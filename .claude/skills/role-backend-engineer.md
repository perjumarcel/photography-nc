# Role Skill: Backend Engineer

> Role perspective for building and maintaining the ASP.NET Core API.

## Role Context

The Backend Engineer owns domain modeling, data access, API contracts, and backend test coverage, following Clean Architecture with MediatR CQRS.

## Architecture Layer Rule

```
SharedKernel → Core → Application → Infrastructure → Web
```

| Layer | Contains | Depends On |
|-------|----------|-----------|
| SharedKernel | `Result<T>`, EntityBase, DomainEventBase, ValueObjectBase | Nothing |
| Core | Entities, aggregates, domain events, specs, repo interfaces | SharedKernel |
| Application | Services, handlers, DTOs, event handlers, service interfaces | Core + SharedKernel |
| Infrastructure | `AppDbContext`, repo implementations, email, background services | Application + Core |
| Web | Controllers, middleware, DI, request/response mapping | Application + Infrastructure (DI only) |

**Golden rule:** Application and Web layers never touch `AppDbContext` — only via repository interfaces.

## Service Design Rules

- Max **~9 dependencies** per service — split if exceeded.
- Return `Result<T>` from all public methods — never throw for domain errors.
- Pure domain services for calculation (no I/O).
- `IEmailSender` abstraction — never concrete provider.
- Configurable limits via studio settings — no hardcoded constants.

## Consult These Skills

| Task | Skill |
|------|-------|
| New command/query | `backend-cqrs-handler.md` |
| New entity | `domain-entity-template.md` |
| Status lifecycle | `domain-status-machine.md` |
| Value objects | `domain-value-objects.md` |
| Price handling | `domain-price-snapshots.md` |
| Time/scheduling | `domain-time-utc.md` |
| Read-only queries | `ef-core-query-repository.md` |
| Write operations | `ef-core-command-repository.md` |
| EF Core config | `ef-core-entity-configuration.md` |
| Transactions | `ef-core-transactions.md` |
| Error returns | `backend-result-pattern.md` |
| Async methods | `cancellation-token-discipline.md` |
| Multi-tenancy | `multi-tenancy-studio-scope.md` |
| Controller setup | `api-controller-conventions.md` |
| Writing tests | `testing-backend-xunit.md` |
| Background jobs | `background-service-template.md` |
| Reliable messaging | `outbox-pattern.md` |
| Security review | `security-checklist.md` |

## Checklist (every PR)

- [ ] Layer dependency rule respected
- [ ] No `AppDbContext` in Application or Web layers
- [ ] `Result<T>` returned — no exceptions for domain errors
- [ ] `CancellationToken` on every async method
- [ ] Fluent API configuration — no data annotations
- [ ] DTOs for API responses — never domain entities
- [ ] StudioScopedActionFilter on admin routes
- [ ] xUnit tests for new logic
