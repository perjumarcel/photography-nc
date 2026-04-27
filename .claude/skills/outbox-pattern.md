# Skill: Outbox Pattern

> Reliable message delivery via outbox table with background retry.

## When to Apply

- Sending emails, notifications, or webhooks that must not be lost
- Any external dispatch that needs audit trail and retry

## Write to Outbox

```csharp
// In a domain event handler or service
public async Task Handle(BookingConfirmedEvent notification, CancellationToken ct)
{
    var entry = new NotificationOutbox(
        recipientEmail: booking.ClientEmail,
        templateKey: "booking-confirmed",
        payload: JsonSerializer.Serialize(new { booking.BookingId, booking.StartUtc }),
        status: NotificationStatus.Pending);

    outboxRepo.Add(entry);
    await unitOfWork.SaveChangesAsync(ct);

    // Optionally attempt immediate send (fire-and-forget)
    try
    {
        await emailSender.SendAsync(entry, ct);
        entry.MarkSent();
        await unitOfWork.SaveChangesAsync(ct);
    }
    catch
    {
        // Left as Pending — background service will retry
    }
}
```

## Process Outbox (Background Service)

```csharp
var pending = await outboxRepo.GetPendingAsync(batchSize: 50, ct);

foreach (var notification in pending)
{
    try
    {
        await emailSender.SendAsync(notification, ct);
        notification.MarkSent();
    }
    catch (Exception ex)
    {
        notification.IncrementRetry(ex.Message);
        if (notification.RetryCount >= MaxRetries)
            notification.MarkFailed();
    }
}

await unitOfWork.SaveChangesAsync(ct);
```

## Rules

1. **Every email** recorded in outbox — even on immediate success (`Status=Sent`).
2. **Failure must never lose a message** — save as `Pending` for retry.
3. **Max retry count** with backoff — mark as `Failed` after limit.
4. Use `IEmailSender` abstraction — never depend on concrete provider (Resend, SMTP).
5. Admin must **explicitly trigger** sharing — never auto-send on entity creation.
6. Rate limit resend: 1 per 60s per email.

## Checklist

- [ ] All external dispatches recorded in outbox
- [ ] Failures saved as Pending for retry — never lost
- [ ] Max retry count enforced
- [ ] `IEmailSender` abstraction used
- [ ] No sensitive data in outbox payload
