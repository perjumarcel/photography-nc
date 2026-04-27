# Skill: Domain Value Objects

> Typed domain concepts that ensure consistency through encapsulation.

## When to Apply

- Wrapping a primitive that has domain-specific validation or normalization
- Ensuring consistent formatting (codes, emails, phone numbers)

## Template

```csharp
// Typed domain concept — ensures consistency
public record PromotionCode
{
    public string Value { get; }

    public PromotionCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Promotion code cannot be empty");
        Value = code.Trim().ToUpperInvariant();
    }
}
```

## Usage

```csharp
// CORRECT — always use the value object
var code = new PromotionCode(userInput);
await repo.FindByCodeAsync(code.Value, ct);

// WRONG — never raw string manipulation
var normalized = userInput.Trim().ToUpperInvariant();
```

## Rules

- Use `record` type for built-in equality and immutability.
- Validate in constructor — fail fast on invalid input.
- Normalize in constructor (trim, case conversion).
- Never use raw `code.ToUpperInvariant()` — always `new PromotionCode(code).Value`.

## Checklist

- [ ] Implemented as `record` type
- [ ] Constructor validates input
- [ ] Constructor normalizes value
- [ ] Callers use `.Value` — never raw string manipulation
