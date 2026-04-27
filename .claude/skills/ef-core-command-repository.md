# Skill: EF Core Command Repository

> Read-write repository pattern: tracked entities, sync mutations.

## When to Apply

- Adding a new entity that needs create/update/delete persistence
- Loading an entity for mutation

## Template

```csharp
// Interface — in Core layer
public interface IBookingCommandRepository
{
    void Add(Booking booking);
    void Update(Booking booking);
    void Remove(Booking booking);
    Task<Booking?> GetByIdAsync(Guid bookingId, CancellationToken ct = default);
}

// Implementation — in Infrastructure layer
public class BookingCommandRepository(AppDbContext db) : IBookingCommandRepository
{
    public void Add(Booking booking) => db.Bookings.Add(booking);        // Sync — no Async
    public void Update(Booking booking) => db.Bookings.Update(booking);  // Sync — no Async
    public void Remove(Booking booking) => db.Bookings.Remove(booking);  // Sync — no Async

    public async Task<Booking?> GetByIdAsync(Guid bookingId, CancellationToken ct = default)
    {
        return await db.Bookings
            .FirstOrDefaultAsync(b => b.BookingId == bookingId, ct);  // Tracked — no AsNoTracking
    }
}
```

## Rules

- `Add`, `Update`, `Remove` are **sync** — no `AddAsync`.
- `GetByIdAsync` returns **tracked** entities — no `.AsNoTracking()`.
- `CancellationToken ct = default` on every async method.
- Never use `AppDbContext` directly in Application or Web layers.

## Checklist

- [ ] Sync `Add`/`Update`/`Remove` — no `AddAsync` or `UpdateAsync`
- [ ] Tracked entities (no `.AsNoTracking()`)
- [ ] `CancellationToken ct = default` on async methods
- [ ] Interface in Core layer, implementation in Infrastructure
