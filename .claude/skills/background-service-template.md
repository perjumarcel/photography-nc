# Skill: Background Service Template

> BackgroundService base pattern with scoped DI, batch loading, and graceful shutdown.

## When to Apply

- Adding a new periodic background job
- Processing queued work items

## Existing Services

| Service | Purpose |
|---------|---------|
| `ExpiredDataCleanupService` | Expires pending bookings past confirmation window |
| `NotificationProcessingService` | Processes notification outbox (email dispatch) |
| `BookingReminderService` | Sends booking reminders |
| `GalleryExpiryAndDeletionWorker` | Expires and deletes galleries |
| `ZipGenerationWorker` | Generates ZIP archives |

## Template

```csharp
public class MyProcessingService(
    IServiceScopeFactory scopeFactory,
    ILogger<MyProcessingService> logger) : BackgroundService
{
    private static readonly TimeSpan Interval = TimeSpan.FromMinutes(5);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("MyProcessingService started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessBatchAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;  // Graceful shutdown
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in MyProcessingService");
            }

            await Task.Delay(Interval, stoppingToken);
        }
    }

    private async Task ProcessBatchAsync(CancellationToken ct)
    {
        using var scope = scopeFactory.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IMyRepository>();

        // Batch-load ALL items — never query per-item in a loop
        var items = await repo.GetPendingItemsAsync(ct);
        if (items.Count == 0) return;

        // Pre-load reference data for the entire batch
        var relatedData = await repo.GetRelatedDataAsync(
            items.Select(i => i.RelatedId).Distinct().ToList(), ct);

        foreach (var item in items) { /* process */ }
        await scope.ServiceProvider.GetRequiredService<IUnitOfWork>().SaveChangesAsync(ct);
    }
}
```

## Registration

```csharp
services.AddHostedService<MyProcessingService>();
```

## Rules

1. Extend `BackgroundService` — not raw `IHostedService`.
2. Create new `IServiceScope` per iteration.
3. Handle `CancellationToken` — check `stoppingToken.IsCancellationRequested`.
4. Catch `OperationCanceledException` for graceful shutdown.
5. **Batch-load** all data before iteration — never query per-item.
6. Queue blob/file deletion as background job — never block user requests.

## Checklist

- [ ] Extends `BackgroundService`
- [ ] Creates `IServiceScope` per iteration
- [ ] Handles `CancellationToken` and graceful shutdown
- [ ] Batch-loads data before processing loop
- [ ] Registered via `AddHostedService<T>()`
- [ ] No sensitive data in log messages
