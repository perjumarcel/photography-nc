# Skill: Domain Status Machine

> Enum-based status transitions with guard methods on the aggregate root.

## When to Apply

- Adding lifecycle statuses to an entity (booking, order, gallery)
- Implementing state transitions with validation

## Template

```csharp
public class Booking : EntityBase
{
    private static readonly Dictionary<BookingStatus, BookingStatus[]> _allowedTransitions = new()
    {
        [BookingStatus.Pending]   = [BookingStatus.Confirmed, BookingStatus.Cancelled, BookingStatus.Expired],
        [BookingStatus.Confirmed] = [BookingStatus.Completed, BookingStatus.Cancelled, BookingStatus.NoShow],
        [BookingStatus.Completed] = [BookingStatus.Confirmed],
        [BookingStatus.Cancelled] = [],
        [BookingStatus.NoShow]    = [BookingStatus.Confirmed],
        [BookingStatus.Expired]   = [],
    };

    public BookingStatus Status { get; private set; }

    public bool CanTransitionTo(BookingStatus newStatus)
        => _allowedTransitions.TryGetValue(Status, out var allowed) && allowed.Contains(newStatus);

    public void TransitionTo(BookingStatus newStatus)
    {
        if (!CanTransitionTo(newStatus))
            throw new InvalidOperationException($"Cannot transition from {Status} to {newStatus}");

        Status = newStatus;
        AddDomainEvent(new BookingStatusChangedEvent(BookingId, Status));
    }
}
```

## Rules

1. All statuses in an **enum** — never strings.
2. Transition map as `static readonly Dictionary` on the aggregate.
3. `CanTransitionTo()` — query guard, called by controllers/services before attempting.
4. `TransitionTo()` — performs the transition + raises domain event.
5. Orthogonal concerns (payment) as separate methods: `MarkPaid()` independent of booking status.
6. Never use raw string literals for status — always enum + `CanTransitionTo()`.

## Checklist

- [ ] Status is an enum — not a string
- [ ] `_allowedTransitions` dictionary defined as `static readonly`
- [ ] `CanTransitionTo()` guard method exists
- [ ] `TransitionTo()` raises domain event
- [ ] Terminal states have empty transition arrays
