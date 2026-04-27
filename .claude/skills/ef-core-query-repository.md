# Skill: EF Core Query Repository

> Read-only repository pattern: AsNoTracking, projections, existence checks.

## When to Apply

- Adding a new read-only query or data retrieval method
- Building projections for API responses

## Template

```csharp
// Interface — in Core layer
public interface IBookingQueryRepository
{
    Task<BookingDetailDto?> GetByIdAsync(Guid bookingId, CancellationToken ct = default);
    Task<List<BookingListDto>> GetByStudioAsync(Guid studioId, CancellationToken ct = default);
    Task<bool> AnyOverlappingAsync(Guid resourceId, DateTime start, DateTime end, CancellationToken ct = default);
    Task<int> CountPendingByClientAsync(Guid clientId, CancellationToken ct = default);
}

// Implementation — in Infrastructure layer
public class BookingQueryRepository(AppDbContext db) : IBookingQueryRepository
{
    public async Task<BookingDetailDto?> GetByIdAsync(Guid bookingId, CancellationToken ct = default)
    {
        return await db.Bookings
            .AsNoTracking()                          // ALWAYS for queries
            .Where(b => b.BookingId == bookingId)
            .Select(b => new BookingDetailDto        // ALWAYS project — never return entity
            {
                BookingId = b.BookingId,
                Status = b.Status.ToString(),
                StartUtc = b.StartUtc,
            })
            .FirstOrDefaultAsync(ct);                // ALWAYS pass ct
    }

    public async Task<bool> AnyOverlappingAsync(
        Guid resourceId, DateTime start, DateTime end, CancellationToken ct = default)
    {
        return await db.Bookings
            .AsNoTracking()
            .AnyAsync(b => b.ResourceId == resourceId
                        && b.StartUtc < end
                        && b.BlockedEndUtc > start, ct);
    }
}
```

## Rules

- `.AsNoTracking()` on **every** query.
- `.Select()` projections — **never** return full domain entities.
- `AnyAsync()` for existence checks — not `FirstOrDefaultAsync()` + null check.
- `CancellationToken ct = default` on every async method.
- Never query inside a loop — batch pre-load all data first.
- Add `CountAsync()` variants for pagination/limits.

## Checklist

- [ ] `.AsNoTracking()` on all queries
- [ ] `.Select()` projections to DTOs — no full entities returned
- [ ] `AnyAsync()` for existence — not `FirstOrDefault` + null check
- [ ] `CancellationToken ct = default` on every method
- [ ] No querying inside loops
