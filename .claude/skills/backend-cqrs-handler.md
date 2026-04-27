# Skill: Backend CQRS Handler (MediatR)

> Use this pattern when adding a new command or query handler.

## When to Apply

- Adding a new write operation (create, update, delete, state change)
- Adding a new read operation (get, list, search)
- Any operation that goes through a controller

## Command / Query Record

Define in the **Web layer** (or Application layer for reusable commands):

```csharp
// Command — mutates state
public record CreateBookingCommand(Guid StudioId, CreateBookingDto Dto) : IRequest<Result<Guid>>;

// Query — read-only
public record GetBookingByIdQuery(Guid StudioId, Guid BookingId) : IRequest<Result<BookingDetailDto>>;
```

Rules:
- Use `record` types (immutable by default).
- Always return `Result<T>` — never raw types.
- Include `StudioId` for tenant-scoped operations.

## Handler Implementation

```csharp
public class CreateBookingHandler(
    IBookingCommandRepository bookingRepo,
    IAvailabilityService availability,
    IUnitOfWork unitOfWork)
    : IRequestHandler<CreateBookingCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateBookingCommand request, CancellationToken ct)
    {
        // 1. Validate business rules
        var available = await availability.IsSlotAvailableAsync(request.Dto.StartUtc, ct);
        if (!available)
            return Result<Guid>.Fail("Time slot is not available");

        // 2. Create domain entity (validation in constructor)
        var booking = new Booking(
            request.StudioId,
            request.Dto.ResourceId,
            request.Dto.ServiceId,
            request.Dto.ClientId,
            request.Dto.StartUtc,
            request.Dto.DurationMinutes,
            request.Dto.BasePrice,
            request.Dto.CurrencyCode);

        // 3. Persist
        bookingRepo.Add(booking);
        await unitOfWork.SaveChangesAsync(ct);

        return Result<Guid>.Ok(booking.BookingId);
    }
}
```

## Checklist

- [ ] Record type implements `IRequest<Result<T>>`
- [ ] Handler uses primary constructor for DI
- [ ] `CancellationToken ct` forwarded to every async call
- [ ] Domain validation in entity constructor or service — not in handler
- [ ] Returns `Result.Fail()` for known failures — never throws
- [ ] Max ~9 dependencies — split if exceeded
- [ ] Query handlers use query repositories (`.AsNoTracking()`)
- [ ] Command handlers use command repositories (tracked entities)
- [ ] Multi-step mutations wrapped in explicit transaction

## Anti-Patterns

- Putting business logic in the controller instead of the handler
- Calling `SaveChangesAsync` multiple times without a transaction
- Using `AppDbContext` directly in the handler
- Returning domain entities instead of DTOs
- Missing `CancellationToken` parameter
