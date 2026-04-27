# Skill: EF Core Entity Configuration

> Fluent API configuration via IEntityTypeConfiguration — no data annotations.

## When to Apply

- Adding a new entity to the database
- Configuring indexes, relationships, or constraints

## Template

```csharp
// Infrastructure/Data/Config/{Entity}Configuration.cs
public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.HasKey(b => b.BookingId);
        builder.Property(b => b.BookingId).ValueGeneratedNever();

        builder.Property(b => b.StudioId).IsRequired();
        builder.Property(b => b.Status).IsRequired().HasConversion<string>().HasMaxLength(50);
        builder.Property(b => b.CurrencyCode).IsRequired().HasMaxLength(3);

        // Concurrency token for optimistic locking
        builder.Property(b => b.RowVersion).IsRowVersion();

        // Indexes
        builder.HasIndex(b => b.StudioId);
        builder.HasIndex(b => new { b.ResourceId, b.StartUtc });

        // Relationships — one direction, FK only
        builder.HasMany(b => b.AddOns)
            .WithOne()
            .HasForeignKey(a => a.BookingId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
```

## Rules

- **Fluent API only** — never data annotations on entities.
- Enums stored as strings: `.HasConversion<string>().HasMaxLength(50)`.
- `ValueGeneratedNever()` on manually assigned IDs.
- Concurrency tokens on single-use resources (certificates, promo codes).
- Migrations only — never `EnsureCreated()`.
- Register via assembly scan in `OnModelCreating`.

## Checklist

- [ ] `IEntityTypeConfiguration<T>` in `Infrastructure/Data/Config/`
- [ ] No data annotations on entity classes
- [ ] Enums converted to strings with `HasConversion<string>()`
- [ ] `ValueGeneratedNever()` on manual IDs
- [ ] Appropriate indexes on query-filtered columns
- [ ] Concurrency tokens on single-use resources
