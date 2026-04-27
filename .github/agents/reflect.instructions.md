# Reflect Studio Agent Instructions

You are an expert software developer working on the Reflect Studio project - a multi-studio self-portrait photography booking platform. You must produce code that is consistent with the established architecture, design patterns, domain language, and quality standards defined below.

## Project Stack

- **Backend:** ASP.NET Core (.NET 10) Web API with Clean Architecture
- **Frontend:** React 19 + Redux Toolkit SPA (Vite, TypeScript, Tailwind CSS 4)
- **Database:** PostgreSQL (production), SQLite (local dev) via EF Core with code-first migrations
- **Deployment:** Docker → fly.io

## Domain Context

The platform manages photography studio bookings in Chișinău, Moldova with:
- Public booking wizard (4-step flow)
- Admin dashboard with FullCalendar integration
- Gallery management with password protection
- Gift certificates and promo codes
- Print order management
- Multi-tenant studio support with role-based access

---

## Architecture Rules (STRICTLY ENFORCED)

### Backend - Clean Architecture

```
src/
├── ReflectStudio.SharedKernel/      # Base classes, value objects, Result<T>, interfaces
├── ReflectStudio.Core/              # Domain entities, enums, specifications, domain events
├── ReflectStudio.Application/       # Use-case services, event handlers, DTOs, mapping
├── ReflectStudio.Infrastructure/    # EF Core DbContext, repositories, external adapters
└── ReflectStudio.Web/               # Controllers, MediatR handlers, middleware, filters
```

**Dependency rule - NEVER violate:**
- `Core` has ZERO dependency on Infrastructure or Web
- `Application` depends on Core interfaces; never on EF Core or HTTP
- `Infrastructure` implements Core interfaces
- `Web` wires everything via DI; controllers delegate to MediatR or Application services
- Web-layer handlers must NOT access repositories directly - use Application services only

### Frontend - Feature-Sliced Design

```
client/src/
├── app/             # Redux store, hooks, router, providers
├── entities/        # Shared business domain entities
├── features/        # Feature modules with api/, model/, ui/ subdirectories
├── pages/           # Route-level page components
├── shared/          # Shared utilities, UI components, API client
├── store/           # Additional store slices
└── i18n/            # Translations
```

**Import boundary rules:**
- `shared/` cannot import from `features/`, `pages/`, `app/`
- `features/` cannot import from `pages/`, `app/` (except hooks via Containers)
- Only `*Container.tsx` files may use `useAppDispatch`/`useAppSelector`
- Presenters receive data and callbacks via props only

---

## Domain-Driven Design Rules

### Bounded Contexts

| Context | Aggregate Roots |
|---------|----------------|
| BookingContext | `Booking` |
| StudioContext | `Studio`, `Resource`, `Service`, `AddOnGroup`, `AddOn` |
| CustomerContext | `Customer` |
| PromotionContext | `GiftCertificate`, `PromoCode` |
| GalleryContext | `Gallery` |
| NotificationContext | `NotificationOutboxEntry` |
| PrintingContext | `PrintOrder` |

### Domain Rules

1. Aggregates reference other aggregates by ID only - never navigation properties across contexts
2. Entities use private setters with behavioral methods (e.g., `booking.SetStatus()`)
3. Domain events extend `DomainEventBase` and are published after persistence
4. Value objects are immutable records: `Money`, `EmailAddress`, `PhoneNumber`, `Address`, `PromotionCode`
5. Specifications encapsulate query predicates
6. Domain services are pure (no I/O)
7. Price snapshots are immutable records on entities

### Ubiquitous Language

Use these exact terms in code:
- **Booking** - An appointment reservation
- **BookingStatus** - Lifecycle: `Pending` → `Confirmed` → `Completed` / `Cancelled` / `NoShow` / `Expired`
- **Resource** - A physical studio room/set
- **Service** - A photography session type with duration and price
- **Slot Step** - Minutes between available start times
- **Cleanup Minutes** - Buffer time between bookings
- **Session End** - start + duration
- **Blocked End** - session end + cleanup
- **Trusted** - Customer flag for auto-confirm
- **GiftCertificate** - Single-use code for free session
- **PromoCode** - Discount code with percentage and date range
- **Gallery** - Photo gallery with access token and optional password
- **PrintOrder** - Order for physical prints

---

## Design Patterns

### Backend Patterns

| Pattern | Implementation |
|---------|---------------|
| Repository | Split into Query (`.AsNoTracking()`) and Command repos |
| Unit of Work | `IUnitOfWork` wraps `SaveChangesAsync()` |
| MediatR (CQRS) | Commands/Queries as records with handlers in Web layer |
| Domain Events | Published after `SaveChangesAsync`, handled in Application layer |
| Result Object | `Result<T>` from SharedKernel: `Result.Ok()`, `Result.Fail()` |
| Specification | Encapsulated query predicates in Core |

### Frontend Patterns

| Pattern | Implementation |
|---------|---------------|
| Redux Toolkit | `createSlice` + `createAsyncThunk` with `condition` for caching |
| Container/Presenter | Containers use Redux; Presenters receive props |
| Custom Hooks | Complex state in `useXxx` hooks |
| Listener Middleware | Cross-cutting concerns in `app/listeners/` |
| LoadStatus | Enum for async state: `idle`, `loading`, `succeeded`, `failed` |

### Code Style

**Backend:**
- C# primary constructors for DI
- Record types for commands, DTOs, value objects
- Nullable reference types enabled
- PascalCase types/methods, camelCase parameters
- `Async` suffix only for async methods

**Frontend:**
- TypeScript strict mode
- Functional components only
- Named exports for components, default for pages
- PascalCase components, camelCase variables
- `use` prefix for hooks, `select` prefix for selectors

---

## Data & Persistence Rules

### EF Core

- Fluent API via `IEntityTypeConfiguration<T>` - no data annotations
- Concurrency tokens on certificates and promo codes
- Multi-tenancy via `StudioId` enforced by `StudioScopedActionFilter`
- Always use migrations - never `EnsureCreated()`

### Query Performance

- Read-only queries: `.Select()` projection with `.AsNoTracking()`
- N+1 prevention: Pre-load batch data before iteration
- Existence checks: Use `AnyAsync()` not `ToListAsync()`
- Count queries: Use `CountAsync()` variants

### Transaction Rules

- Booking creation wrapped in explicit transaction
- Certificate/promo redemption uses concurrency tokens
- Multi-step operations in single transaction
- Avoid multiple `SaveChangesAsync` without transaction

---

## API Conventions

### Routes

- Admin: `api/admin/{studioId:guid}/...`
- Client: `api/client/...`
- Public: `api/public/...`

### HTTP Verbs

| Operation | Verb | Response |
|-----------|------|----------|
| List/query | GET | 200 OK |
| Create | POST | 201 Created |
| Full update | PUT | 200 OK |
| Partial update | PATCH | 200/204 |
| Delete | DELETE | 204 |
| Action | POST | 200/204 |

---

## Security Rules

- JWT secrets from environment variables only
- Seed data gated behind `IsDevelopment()`
- Input validation on all DTOs
- CORS restricted to specific origins
- Security headers via middleware
- No sensitive tokens in logs
- Rate limiting on auth endpoints

---

## Common Pitfalls to AVOID

1. **Never** put business logic in controllers
2. **Never** access `AppDbContext` from Application or Web layers
3. **Never** use navigation properties across aggregate boundaries
4. **Never** skip transactions for multi-step mutations
5. **Never** import from `app/hooks` in Presenter components
6. **Never** import from `features/` in `shared/`
7. **Never** call `SaveChangesAsync` multiple times without transaction
8. **Never** expose domain entities in API responses
9. **Never** hardcode secrets
10. **Never** use `EnsureCreated()`
11. **Never** create services with >9 dependencies
12. **Never** use strings for status transitions - use enums
13. **Never** query inside a loop - pre-load batch data
14. **Never** exceed 250 lines per component or 150 lines per hook
15. **Never** use `err: any` - use `err: unknown` with `extractErrorMessage()`
16. **Never** name sync operations `*Async`
17. **Never** use raw string manipulation for promo codes - use value objects

---

## CancellationToken Rules (MANDATORY)

- Every async interface method includes `CancellationToken ct = default`
- Controllers pass `HttpContext.RequestAborted`
- MediatR handlers forward to all service calls
- Services forward to repository calls
- Repositories forward to ALL EF Core async calls

---

## New Feature Checklist

### Domain Layer (Core)
- [ ] Entities with private setters and behavioral methods
- [ ] Correct bounded context namespace
- [ ] Domain events for side effects
- [ ] Specifications for complex predicates
- [ ] Value objects for typed concepts
- [ ] ID-only references to other aggregates

### Application Layer
- [ ] Focused services (max ~9 dependencies)
- [ ] Split query/command operations
- [ ] `Result<T>` return values
- [ ] Event handlers (one per event)
- [ ] DTOs for API contracts

### Infrastructure Layer
- [ ] Query repo with `.AsNoTracking()`
- [ ] Command repo separate
- [ ] `IEntityTypeConfiguration<T>` with Fluent API
- [ ] EF Core migration

### Web Layer
- [ ] Thin controller with `StudioScopedActionFilter`
- [ ] MediatR commands/queries
- [ ] Appropriate `[Authorize(Policy)]`
- [ ] DI registration

### Frontend
- [ ] Feature directory: `api/`, `model/`, `ui/`
- [ ] Redux slice with `createAsyncThunk`
- [ ] `LoadStatus` for async state
- [ ] Container/Presenter split
- [ ] Import boundary compliance
- [ ] i18n translation keys

---

## UI Standards

### Component Library
- shadcn/ui with Radix primitives + Tailwind CSS 4
- Use `cn()` utility for class merging
- CVA for component variants

### Accessibility (WCAG 2.1 AA)
- Modals: focus trapping, `role="dialog"`, escape key
- Interactive elements: visible focus rings
- Heading hierarchy: semantic, no skipped levels
- Images: `alt` text required
- Icon buttons: `aria-label` required

### Design Tokens
Always use CSS custom properties:
- `--color-brand` (#ffc347) - CTAs, highlights
- `--color-brand-dark` (#e6b800) - Hover states
- `--color-surface-*` - Background shades

### Motion
- Respect `prefers-reduced-motion: reduce`
- Allowed: fade-up, scroll-reveal, hover effects
- Keep `animate-spin` for loading feedback

---

## Build Commands

```bash
# Backend
cd src && dotnet build ReflectStudio.slnx
cd tests && dotnet test ReflectStudio.Application.Tests/

# Frontend
cd client && npm run build
cd client && npx vitest run
cd client && npm run lint
```

