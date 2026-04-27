# Skill: Domain Entity Template

> Entity base pattern with constructors, private setters, and behavioral methods.

## When to Apply

- Creating a new domain entity or aggregate root

## Template

```csharp
using ReflectStudio.SharedKernel;

namespace ReflectStudio.Core.{Context}Context;

public class Order : EntityBase
{
    // Private parameterless ctor for EF Core
    private Order() { }

    // Public ctor — validates ALL invariants upfront
    public Order(Guid studioId, Guid clientId, decimal priceSnapshot, string currencyCode)
    {
        if (studioId == Guid.Empty) throw new ArgumentException("StudioId is required");
        if (clientId == Guid.Empty) throw new ArgumentException("ClientId is required");
        if (priceSnapshot < 0) throw new ArgumentException("Price must be >= 0");
        if (string.IsNullOrWhiteSpace(currencyCode) || currencyCode.Length != 3)
            throw new ArgumentException("Currency code must be 3 characters");

        OrderId = Guid.NewGuid();
        StudioId = studioId;
        ClientId = clientId;
        PriceSnapshot = priceSnapshot;
        CurrencyCode = currencyCode;
        CreatedAtUtc = DateTime.UtcNow;
    }

    // Properties — private setters, mutation via behavioral methods only
    public Guid OrderId { get; }
    public Guid StudioId { get; }
    public Guid ClientId { get; }
    public decimal PriceSnapshot { get; }
    public string CurrencyCode { get; } = string.Empty;
    public DateTime CreatedAtUtc { get; }
}
```

## Rules

- Extend `EntityBase` from SharedKernel.
- Private parameterless constructor for EF Core hydration.
- Public constructor validates **all** required fields — fail fast.
- Properties use **private setters** — state changes only through behavioral methods.
- Generate `Guid.NewGuid()` in constructor — never let callers provide IDs.
- Place in correct bounded context namespace: `ReflectStudio.Core.{Context}Context`.

## Checklist

- [ ] Extends `EntityBase`
- [ ] Private parameterless constructor for EF Core
- [ ] Public constructor validates all required fields
- [ ] Properties have private setters
- [ ] ID generated internally via `Guid.NewGuid()`
- [ ] Placed in correct bounded context namespace
