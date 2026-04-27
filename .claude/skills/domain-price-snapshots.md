# Skill: Domain Price Snapshots

> Prices are snapshotted at creation time and never recomputed.

## When to Apply

- Creating entities that hold pricing (bookings, orders, print orders)
- Applying discounts or promotions to prices

## Rules

1. **Snapshot at creation time** — store price as immutable fields on the entity.
2. **Never recompute** from the current catalog after creation.
3. Client-side price calculation is **UX preview only** — backend always re-validates and snapshots.
4. Monetary values: always `>= 0`, apply `Math.Max(0, total)` when subtracting discounts.
5. Use a **pure domain service** for price calculation (no I/O, no infrastructure dependencies).
6. Currency code: 3-character ISO code (e.g., `MDL`), validated in constructor.

## Template

```csharp
public class Booking : EntityBase
{
    public decimal BasePriceSnapshot { get; }        // Immutable — set at creation
    public decimal DiscountSnapshot { get; }         // Immutable — set at creation
    public decimal FinalPrice => Math.Max(0, BasePriceSnapshot - DiscountSnapshot);
    public string CurrencyCode { get; } = string.Empty;  // 3-char ISO
}
```

## Checklist

- [ ] Price stored as immutable snapshot field
- [ ] Never recomputed from current catalog
- [ ] `Math.Max(0, ...)` when subtracting discounts
- [ ] Currency code validated (3 characters)
- [ ] Pure domain service for calculation (no I/O)
