# Skill: Avoid .Contains() in EF Core Queries

> Prefer correlated subqueries or joins over `.Contains(list)` to avoid SQL `IN (...)` anti-patterns.

## When to Apply

- Fetching related data for a set of entities already queried
- Resolving lookup values (names, colors, metadata) for query results
- Any scenario where you would collect IDs into a list then filter with `.Contains()`

## Problem

```csharp
// ❌ BAD — materialises an in-memory list then sends it as SQL IN (...)
List<Guid> bookingIds = rawBookings.Select(b => b.BookingId).ToList();
var colors = await db.BookingAddOns.AsNoTracking()
    .Where(ba => bookingIds.Contains(ba.BookingId))   // IN (...) — unbounded, unparameterised
    .Join(db.AddOns, ba => ba.AddOnId, a => a.AddOnId, (ba, a) => new { ba.BookingId, a.Color })
    .ToDictionaryAsync(x => x.BookingId, x => x.Color, ct);
```

**Why this is harmful:**
- EF Core translates `.Contains(list)` to SQL `IN (@p0, @p1, …, @pN)` with one parameter per item.
- Large lists produce enormous SQL strings that bypass query plan caching.
- PostgreSQL query planner may not use indexes efficiently for large `IN` lists.
- Creates a two-step process (fetch IDs → re-query) instead of a single SQL round-trip.

## Solution: Correlated Subquery Inside `.Select()`

```csharp
// ✅ GOOD — single SQL query with a correlated subquery
var bookings = await db.Bookings.AsNoTracking()
    .Where(b => b.StudioId == studioId && b.StartLocalDate == todayDate)
    .Select(b => new
    {
        b.BookingId,
        b.ClientId,
        BackdropColor = db.BookingAddOns
            .Where(ba => ba.BookingId == b.BookingId)
            .Join(db.AddOns, ba => ba.AddOnId, a => a.AddOnId, (ba, a) => a.Color)
            .FirstOrDefault(c => c != null)
    })
    .ToListAsync(ct);
```

**Why this is better:**
- Produces a single SQL query with a `LATERAL JOIN` or correlated subquery.
- The database optimiser can use indexes on the foreign key columns.
- Query plan is stable and cacheable regardless of result set size.
- No in-memory ID list allocation.

## Alternative: Explicit Join

```csharp
// ✅ ALSO GOOD — explicit join when you need data from a related table
var bookingsWithService = await db.Bookings.AsNoTracking()
    .Where(b => b.StudioId == studioId)
    .Join(db.Services, b => b.ServiceId, s => s.ServiceId, (b, s) => new
    {
        b.BookingId,
        ServiceTitle = s.Title
    })
    .ToListAsync(ct);
```

## Rules

1. **Never** collect IDs into a `List<Guid>` then use `.Contains()` to re-query related data.
2. **Prefer** correlated subqueries inside `.Select()` for lookup values (names, colors, counts).
3. **Prefer** explicit `.Join()` when the relationship is 1:1 or N:1 and you need multiple fields.
4. For server-side filtering, use `IQueryable<T>.Contains()` which translates to `WHERE x IN (SELECT ...)` — this runs entirely in SQL and is acceptable.

## Checklist

- [ ] No in-memory `List<T>.Contains()` in new queries
- [ ] Related data resolved via correlated subquery or join inside `.Select()`
- [ ] Server-side filtering uses `IQueryable<T>.Contains()` (not in-memory lists)
- [ ] Single SQL round-trip where possible
- [ ] `CancellationToken` forwarded on all async calls
