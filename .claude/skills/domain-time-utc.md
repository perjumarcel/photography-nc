# Skill: Domain Time & UTC

> All times stored in UTC. Convert to local only at the display boundary.

## When to Apply

- Adding time fields to entities
- Calculating availability, scheduling, or expiration
- Displaying times in the UI

## Rules

1. Store all times as **UTC**: `DateTime.UtcNow`, `DateTimeKind.Utc`.
2. Validate in constructors: `if (date.Kind != DateTimeKind.Utc) throw`.
3. Convert to local **only at display boundary** — never in domain/application layer.
4. Use `TimeZoneInfo.ConvertTimeFromUtc(date, tz).Date` before `DayOfWeek` comparisons.
5. Studio timezone: `Europe/Chisinau` (UTC+2 / UTC+3 DST).

## Scheduling Model

```csharp
// Session timing
SessionEndUtc  = StartUtc + TimeSpan.FromMinutes(DurationMinutes);
BlockedEndUtc  = SessionEndUtc + TimeSpan.FromMinutes(BufferMinutes);

// Overlap check — pre-load all conflicts for the range, then filter in memory
var conflicts = await repo.GetBookingsInRangeAsync(resourceId, rangeStart, rangeEnd, ct);
bool overlaps = conflicts.Any(b => b.StartUtc < newEnd && b.BlockedEndUtc > newStart);
```

## Availability Calculation

- **Pre-load** all conflicts for the entire date range first.
- **Filter in memory** — never query per-slot in a loop.
- Compare using `BlockedEndUtc` (includes buffer), not `SessionEndUtc`.

## Checklist

- [ ] All times stored as UTC
- [ ] Constructor validates `DateTimeKind.Utc`
- [ ] `TimeZoneInfo.ConvertTimeFromUtc()` for display
- [ ] Availability pre-loads range — no per-slot queries
- [ ] `BlockedEndUtc` used for overlap checks (includes buffer)
