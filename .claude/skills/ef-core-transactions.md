# Skill: EF Core Transactions

> Explicit transactions for multi-step mutations and concurrency control.

## When to Apply

- Booking creation (serializable isolation to prevent double-booking)
- Certificate or promo code redemption (concurrency token + transaction)
- Any operation with multiple `SaveChangesAsync` calls

## Template

```csharp
// Multi-step mutation — explicit transaction
await using var tx = await db.Database.BeginTransactionAsync(
    IsolationLevel.Serializable, ct);

try
{
    bookingRepo.Add(booking);
    promoCode.Redeem(booking.BookingId);
    await unitOfWork.SaveChangesAsync(ct);
    await tx.CommitAsync(ct);
}
catch
{
    await tx.RollbackAsync(ct);
    throw;
}
```

```csharp
// Concurrency token — optimistic locking
try
{
    certificate.Redeem(bookingId);
    await unitOfWork.SaveChangesAsync(ct);  // Concurrency token check
    await tx.CommitAsync(ct);
}
catch (DbUpdateConcurrencyException)
{
    await tx.RollbackAsync(ct);
    return Result.Fail("Certificate was already redeemed");
}
```

## Rules

1. **Never call `SaveChangesAsync` multiple times** without a wrapping transaction.
2. Booking creation: **serializable isolation level**.
3. Certificate/promo code redemption: **concurrency token + transaction**.
4. Cancel + restore promotions: **single transaction**.
5. `DbUpdateConcurrencyException` → return `Result.Fail()` — don't rethrow.

## Checklist

- [ ] Multi-step mutations wrapped in explicit transaction
- [ ] Appropriate isolation level (Serializable for bookings)
- [ ] Concurrency tokens on single-use resources
- [ ] `DbUpdateConcurrencyException` handled with `Result.Fail()`
- [ ] `RollbackAsync` in catch block
