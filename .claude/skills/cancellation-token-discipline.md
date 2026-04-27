# Skill: CancellationToken Discipline

> CancellationToken on every async method, forwarded through the entire chain.

## When to Apply

- Writing any async method (interface, implementation, or caller)
- Adding a new API endpoint, service, or repository method

## Rules

1. Every async interface method: `CancellationToken ct = default` as final parameter.
2. Controllers pass `HttpContext.RequestAborted` to mediator.
3. Every downstream async call forwards `ct`.
4. Sync tracking operations (`Add`, `Update`, `Remove`) must NOT have `Async` suffix.

## Chain Example

```csharp
// Controller → Mediator
await mediator.Send(command, HttpContext.RequestAborted);

// Handler → Service
public async Task<Result<T>> Handle(TRequest request, CancellationToken ct)
{
    var entity = await repo.GetByIdAsync(request.Id, ct);
    await unitOfWork.SaveChangesAsync(ct);
}

// Interface
Task<Entity?> GetByIdAsync(Guid id, CancellationToken ct = default);

// Implementation
public async Task<Entity?> GetByIdAsync(Guid id, CancellationToken ct = default)
    => await db.Set<Entity>().FirstOrDefaultAsync(e => e.Id == id, ct);
```

## EF Core Async Calls (always pass ct)

- `.ToListAsync(ct)`
- `.FirstOrDefaultAsync(ct)`
- `.AnyAsync(ct)`
- `.CountAsync(ct)`
- `.SaveChangesAsync(ct)`

## Checklist

- [ ] `CancellationToken ct = default` on every async interface method
- [ ] `HttpContext.RequestAborted` passed in controllers
- [ ] `ct` forwarded to every downstream async call
- [ ] Sync operations (`Add`, `Update`, `Remove`) — no `Async` suffix
