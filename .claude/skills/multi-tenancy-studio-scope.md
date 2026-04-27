# Skill: Multi-Tenancy & Studio Scope

> StudioId on all studio-scoped entities, enforced by filter and repository.

## When to Apply

- Adding any entity that belongs to a studio
- Adding admin API endpoints
- Writing repository queries for studio-scoped data

## How It Works

Every studio-scoped entity has a `StudioId` property. Enforced at three levels:

### 1. Controller Level

```csharp
[ApiController]
[Route("api/admin/{studioId:guid}/items")]
[Authorize(Policy = "AdminOnly")]
[ServiceFilter(typeof(StudioScopedActionFilter))]  // Validates studioId ownership
public class ItemsController(IMediator mediator) : ControllerBase
```

`StudioScopedActionFilter` verifies the authenticated user owns the `studioId` from the route.

### 2. Repository Level

```csharp
// Query repository always filters by StudioId
public async Task<List<ItemDto>> GetByStudioAsync(Guid studioId, CancellationToken ct = default)
{
    return await db.Items
        .AsNoTracking()
        .Where(i => i.StudioId == studioId)   // Always filter
        .Select(i => new ItemDto { ... })
        .ToListAsync(ct);
}
```

### 3. Entity Level

```csharp
public class Item : EntityBase
{
    public Guid StudioId { get; }  // Set in constructor, never changes

    public Item(Guid studioId, ...)
    {
        if (studioId == Guid.Empty) throw new ArgumentException("StudioId is required");
        StudioId = studioId;
    }
}
```

## Route Convention

```
api/admin/{studioId:guid}/...   → [Authorize(Policy = "AdminOnly")] + StudioScopedActionFilter
api/client/...                  → [Authorize(Policy = "EmployeeOrAdmin")]
api/public/...                  → No authorization
```

## Rules

- Never trust client-provided `StudioId` without `StudioScopedActionFilter` validation.
- Every admin controller must have `[ServiceFilter(typeof(StudioScopedActionFilter))]`.
- Repository queries always filter by `StudioId` — no cross-tenant data leaks.
- Use `ClaimsPrincipalExtensions.GetEmail()` — never raw `User.FindFirst(ClaimTypes.Email)?.Value`.

## Checklist

- [ ] Entity has `StudioId` property set in constructor
- [ ] Admin controller has `StudioScopedActionFilter`
- [ ] Repository queries filter by `StudioId`
- [ ] Route includes `{studioId:guid}` parameter
- [ ] Claims accessed via `ClaimsPrincipalExtensions`
