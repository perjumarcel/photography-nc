# Solution architecture

Clean architecture, dependencies always point inward.

```
+----------------+
|  SharedKernel  |  Result<T>, EntityBase, AggregateRoot, IDomainEvent
+--------+-------+
         |
+--------v-------+
|     Core       |  Album, Image, Category, Tag (private setters + behavior)
|                |  Repository interfaces (IAlbumQueryRepository, IAlbumCommandRepository, ICategoryRepository)
+--------+-------+
         |
+--------v-------+
|  Application   |  MediatR commands/queries, DTOs, IStorageService, LoggingBehavior
+--------+-------+
         |
+--------v-------+
| Infrastructure |  AppDbContext (PostgreSQL via Npgsql), Fluent configurations,
|                |  query/command repositories, R2StorageService, LocalFileSystemStorageService
+--------+-------+
         |
+--------v-------+
|      Web       |  Controllers (thin), JWT, Swagger, CORS, security headers,
|                |  rate limiting, health checks, Serilog
+----------------+
```

## Rules

- `Web` references `Application` + `Infrastructure`. It does **not** access `AppDbContext` directly.
- `Application` references `Core` + `SharedKernel` only.
- `Core` references `SharedKernel` only.
- `Infrastructure` references `Core`, `Application`, `SharedKernel`.
- All public async APIs accept a `CancellationToken` that is forwarded through the chain.
- Handlers return `Result<T>`; controllers use `ResultExtensions.ToActionResult` to map to HTTP responses.
- EF Core configuration is via Fluent API only (`IEntityTypeConfiguration<T>` in `Infrastructure/Persistence/Configurations`).
- Query repositories use `AsNoTracking` + `Include` projections; command repositories load the full tracked aggregate.
- Domain entities have private setters + behavioral methods. No public mutator setters.
