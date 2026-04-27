# Reflect Studio — Custom Copilot Agent Instructions

> **Purpose:** Guide Copilot to produce code that is consistent with the established architecture, design patterns, domain language, and quality standards of the Reflect Studio project.

---

## 1. Project Overview

Reflect Studio is a **multi-studio self-portrait photography booking platform** built with:

- **Backend:** ASP.NET Core (.NET 10) Web API with Clean Architecture
- **Frontend:** React 19 + Redux Toolkit SPA (Vite, TypeScript, Tailwind CSS 4)
- **Database:** Multi-provider (PostgreSQL in production, SQLite for local dev, SQL Server optional) via EF Core with code-first migrations
- **Deployment:** Docker → fly.io

### Domain Context

The platform manages photography studio bookings in Chișinău, Moldova. Core capabilities:

- Public booking wizard (4-step flow)
- Admin dashboard with FullCalendar integration
- Gallery management with password protection
- Gift certificates and promo codes
- Print order management
- Multi-tenant studio support with role-based access

---

## 2. Architecture & Layering

### Backend — Clean Architecture (strictly enforced)

```
src/
├── ReflectStudio.SharedKernel/      # Base classes, value objects, Result<T>, interfaces
├── ReflectStudio.Core/              # Domain entities, enums, specifications, domain events, domain services
│   ├── BookingContext/
│   ├── StudioContext/
│   ├── CustomerContext/
│   ├── PromotionContext/
│   ├── GalleryContext/
│   ├── NotificationContext/
│   ├── PrintingContext/
│   └── AuthContext/
├── ReflectStudio.Application/       # Use-case services, event handlers, DTOs, mapping
├── ReflectStudio.Infrastructure/    # EF Core DbContext, repositories, external auth adapters, migrations
└── ReflectStudio.Web/               # Controllers, MediatR handlers, middleware, filters, background services
```

**Dependency rule — never violate:**

```
Web → Application → Core ← Infrastructure
          ↓
     SharedKernel
```

- `Core` has **zero** dependency on Infrastructure or Web.
- `Application` depends on Core interfaces; never on EF Core or HTTP.
- `Infrastructure` implements Core interfaces (repositories, external providers).
- `Web` wires everything via DI; controllers are thin and delegate to MediatR or Application services.
- **Web-layer handlers (QueryHandlers/CommandHandlers) must NOT access repositories directly.** They must use Application-layer services only.

### Frontend — Feature-Sliced Design

```
client/src/
├── app/             # Redux store, typed hooks, listener middleware, router, providers
├── entities/        # Shared business domain entities
├── features/        # Feature modules (auth, booking, admin, galleries, prints, etc.)
│   └── {feature}/
│       ├── api/     # API call functions
│       ├── model/   # Redux slices, selectors, thunks
│       └── ui/      # React components (Container + Presenter pattern)
├── pages/           # Route-level page components
├── shared/          # Shared utilities, UI components, types, API client
│   ├── api/         # Axios client configuration
│   ├── lib/         # Utility functions (validation, formatting, phone, email, etc.)
│   ├── model/       # Shared models (LoadStatus enum)
│   ├── types/       # Shared TypeScript types
│   └── ui/          # Reusable Radix-based UI components
├── store/           # Additional store slices
└── i18n/            # Internationalization translations
```

**Import boundary rules (enforced by ESLint):**

| Layer | Can import from | Cannot import from |
|-------|----------------|--------------------|
| `shared/` | — | `features/`, `pages/`, `app/` |
| `entities/` | `shared/` | `features/`, `pages/`, `app/` |
| `features/` | `shared/`, `entities/` | `pages/`, `app/` (except hooks.ts via Containers) |
| `pages/` | `shared/`, `entities/`, `features/` | — |

**Container/Presenter rule:** Files in `features/**/ui/**/*.tsx` must NOT import from `app/hooks`. Only `*Container.tsx` files may use `useAppDispatch`/`useAppSelector`. Presenters receive data and callbacks via props only.

---

## 3. Domain-Driven Design (DDD)

### Bounded Contexts

| Context | Aggregate Roots | Key Entities |
|---------|----------------|--------------|
| **BookingContext** | `Booking` | `BookingAddOn`, `BookingAuditEntry` |
| **StudioContext** | `Studio`, `Resource`, `Service`, `AddOnGroup`, `AddOn` | `OpeningHoursRule`, `CalendarBlocker`, `TimeBlock`, `StudioMember`, `ServiceAddOnRule` |
| **CustomerContext** | `Customer` | `ClientNote` |
| **PromotionContext** | `GiftCertificate`, `PromoCode` | — |
| **GalleryContext** | `Gallery` | `Album`, `GalleryImage` |
| **NotificationContext** | `NotificationOutboxEntry` | — |
| **PrintingContext** | `PrintOrder` | `PrintOrderItem`, `PrintFormatConfig` |

### Domain Design Rules

1. **Aggregates reference other aggregates by ID only** — never by navigation property across context boundaries.
2. **Entities use private setters** with behavioral methods (e.g., `booking.SetStatus()`, `booking.Reschedule()`).
3. **Domain events** extend `DomainEventBase` (which implements MediatR `INotification`). They are published **after persistence** via `AppDbContext.SaveChangesAsync` override.
4. **Value objects** are immutable records in `SharedKernel/ValueObjects/`: `Money`, `EmailAddress`, `PhoneNumber`, `Address`, `PromotionCode`.
5. **Specifications** implement the Specification pattern for complex query predicates (e.g., `ActiveBookingsOverlappingSpec`, `OverlappingTimeBlocksSpec`).
6. **Domain services** are pure (no I/O): `BookingScheduleValidator`, `BookingPolicyEnforcer`, `BookingPriceCalculator`, `PromotionValidator`.
7. **Price snapshots** are immutable records on `Booking`: `BasePriceSnapshot`, `AddOnsTotalSnapshot`, `TotalPriceSnapshot`.

### Ubiquitous Language

Always use the correct domain terms in code, variables, and comments:

| Term | Meaning |
|------|---------|
| **Booking** | An appointment reservation (primary aggregate) |
| **BookingStatus** | Lifecycle: `Pending` → `Confirmed` → `Completed` / `Cancelled` / `NoShow` / `Expired` |
| **Resource** | A physical studio room/set that can be booked |
| **Service** | A photography session type with duration and price |
| **Slot Step** | Increment (minutes) between available start times |
| **Cleanup Minutes** | Buffer time between bookings for setup/teardown |
| **Session End** | Actual end time (start + duration) |
| **Blocked End** | End of cleanup buffer (session end + cleanup) |
| **Trusted** | Customer flag — auto-confirms bookings |
| **Blocked** | Customer flag — prevents booking creation |
| **GiftCertificate** | Single-use code for a free session on a specific service |
| **PromoCode** | Discount code with percentage, date range, optional service restrictions |
| **RestoreAfterCancellation** | Reverting a used certificate/promo code to Active when booking is cancelled |
| **CalendarBlocker** | Manual block on a resource's calendar |
| **AddOnGroup** | Logical group of add-ons with selection mode (`SingleRequired` / `MultiOptional`) |
| **Gallery** | Photo gallery with access token, optional password protection, albums, and images |
| **PrintOrder** | Order for physical prints from gallery images |

---

## 4. Design Patterns & Conventions

### Backend Patterns

| Pattern | Implementation |
|---------|---------------|
| **Repository** | Split into Query (`IXxxQueryRepository`) and Command (`IXxxCommandRepository`) repos. Queries use `.AsNoTracking()`. |
| **Unit of Work** | `IUnitOfWork` wraps `SaveChangesAsync()`. Coordinated via `UnitOfWorkBehavior` in MediatR pipeline. |
| **MediatR (CQRS)** | Commands as records → CommandHandlers in Web layer. Queries as records → QueryHandlers in Web layer. Pipeline behaviors: `LoggingBehavior` (warns >500ms), `UnitOfWorkBehavior` (auto-save for `ITransactional`). |
| **Domain Events** | Published after `SaveChangesAsync` in `AppDbContext`. Handled by dedicated event handlers in Application layer (one handler per event). |
| **Result Object** | `Result` / `Result<T>` from SharedKernel for service return values. Factory: `Result.Ok()`, `Result.Fail()`, `Result.Ok<T>(data)`, `Result.Fail<T>(message)`. |
| **Specification** | Encapsulated query predicates in `Core/Specifications/`. |
| **Strategy** | OAuth providers via `IExternalAuthProvider` with Google/Facebook/Apple adapters. |
| **Factory Method** | Entity creation via constructors with validation. |
| **Anti-Corruption Layer** | `IExternalAuthProvider` interface isolates third-party OAuth. |
| **ISP Sub-Interfaces** | When a repository interface grows beyond what a single consumer needs, split into focused sub-interfaces. Each consumer should depend only on the methods it actually calls. |
| **Command Objects** | Service methods taking 5+ primitive parameters must be refactored to accept a single typed command record. This improves readability and prevents parameter-order bugs. |
| **Service Size Threshold** | Max ~9 dependencies per service. Beyond that, extract focused collaborators — each owning one concern (e.g., validation, conflict checking, price calculation, promotion application). |
| **Shared Utilities Over Duplication** | Before writing inline logic, check for existing helpers: `ClaimsPrincipalExtensions` for user claims, `TimeFormatHelper` for display formatting, `NotificationComposer` for email composition, value objects for normalization. Create new shared utilities when a pattern repeats 2+ times. |

### Frontend Patterns

| Pattern | Implementation |
|---------|---------------|
| **Redux Toolkit** | `createSlice` + `createAsyncThunk` for state management. Thunks use `condition` to skip duplicate API calls. |
| **Container/Presenter** | Containers use Redux hooks; Presenters are pure components receiving props. |
| **Custom Hooks** | Complex state logic extracted to `useXxx` hooks (e.g., `useBookingWizard`, `useCalendarHandlers`, `useGalleryViewer`). |
| **Listener Middleware** | Cross-cutting concerns (startup init, workflow orchestration) in `app/listeners/`. |
| **Shared LoadStatus** | `LoadStatus` enum (`idle`, `loading`, `succeeded`, `failed`) for async state tracking. |
| **Error Extraction** | `extractErrorMessage()` utility for consistent error handling from API responses. |
| **i18n** | `react-i18next` with translation keys organized per feature. |

### Frontend Data Fetching Rules

- **App-level data loads once:** Data needed across the app (studio config, services, resources) is fetched once at startup via listener middleware — never re-fetch in individual components.
- **RTK `condition` caching:** Every thunk that fetches reference data must use a dedicated `*Loaded` boolean flag in its `condition` callback. Use booleans (not `array.length > 0`) to distinguish "not fetched" from "fetched but empty".
- **`useEffect` legitimate uses only:** Route-param-based fetches, local state initialization, component-level orchestration. If a side effect reacts to a Redux action or spans multiple slices, it belongs in listener middleware.
- **Never put mutable objects in `useEffect` dependency arrays** — objects that transition from `null → populated` (like store-loaded config) will trigger unnecessary re-runs. Derive stable primitive keys instead.
- **Pure reducers:** No `localStorage`, no API calls, no side effects in Redux reducers. Side effects go in listener middleware + utility files.
- **Component line limits:** >250 lines = must split. 150–250 = should split. <150 = good. Extract hooks, sub-components, or utility functions.
- **Hook size limit:** Hooks >150 lines must be split into focused sub-hooks, each owning one concern.

### Code Style

- **Backend:** C# primary constructors for DI injection. Record types for commands, DTOs, and value objects. Nullable reference types enabled.
- **Frontend:** TypeScript strict mode. Functional components only. Named exports for components, default exports for pages. Prettier for formatting.
- **Naming:**
    - Backend: PascalCase for types/methods, camelCase for parameters/locals, `I` prefix for interfaces, `Async` suffix for async methods.
    - Frontend: PascalCase for components/types, camelCase for variables/functions, `use` prefix for hooks, `select` prefix for selectors.
- **File naming:**
    - Backend: PascalCase matching the primary type name (e.g., `BookingService.cs`).
    - Frontend: PascalCase for components (`BookingWizard.tsx`), camelCase for utilities/hooks (`useBookingWizard.ts`, `validation.ts`).

---

## 5. Data & Persistence

### EF Core Conventions

- `AppDbContext` extends `IdentityDbContext` (ASP.NET Identity for authentication).
- **Fluent API** via `IEntityTypeConfiguration<T>` — no data annotations on entities.
- **Concurrency tokens** on entities that need optimistic concurrency (certificates, promo codes).
- **Domain events** collected from entities and dispatched after `SaveChangesAsync`.
- **Multi-tenancy** via `StudioId` on all studio-scoped entities. Enforced by `StudioScopedActionFilter` on controllers.
- Use **migrations** (`dotnet ef migrations add`) — never `EnsureCreated()`.

### Transaction Rules

- **Booking creation** must be wrapped in an explicit database transaction.
- **Certificate/promo code redemption** must use concurrency tokens to prevent double-use.
- **Multi-step operations** (cancel + restore promotions) must be in a single transaction.
- Avoid multiple `SaveChangesAsync` calls without a wrapping transaction.

### Projection & Query Performance Rules

- **Decision tree:** Need to mutate entity? → Load full entity (tracked). Display/read-only? → Use `.Select()` projection with `.AsNoTracking()`.
- **N+1 prevention:** Never query inside a loop — pre-load batch data for the entire range before iteration (e.g., availability: load all day conflicts in 3 queries, not 3 per slot × 36 slots).
- **`AsNoTracking()` required** on ALL read-only repository methods.
- **Existence checks:** Use `AnyAsync()` not `ToListAsync()` when only checking existence (overlap checks, duplicate checks).
- **Count queries:** Add `CountAsync()` variants instead of loading full entities just for `.Count`.
- **Dead code:** Remove unused query methods — don't keep dead code in repositories.
- **Coverage target:** ~60–70% of read operations use projections; 100% of write operations use full entities.
- **CQRS-lite:** Dedicated query repositories use projections; command repositories load full entities.

---

## 6. API & Controller Conventions

### Controller Structure

- **Route prefix:** `api/admin/{studioId:guid}/...` for admin endpoints, `api/client/...` for authenticated client endpoints, `api/public/...` for public endpoints.
- **Authorization policies:** `AdminOnly`, `EmployeeOrAdmin` (applied via `[Authorize(Policy = "...")]`).
- **Studio scoping:** All admin controllers use `[ServiceFilter(typeof(StudioScopedActionFilter))]` — verifies the user belongs to the studio.
- **Controllers are thin:** They delegate to MediatR `Send()` or Application services. No business logic in controllers.
- **Primary constructors:** Controllers use `(IMediator mediator)` primary constructor pattern.

### HTTP Conventions

| Operation | Verb | Response |
|-----------|------|----------|
| List/query | `GET` | `200 OK` with array/object |
| Create | `POST` | `201 Created` with created resource |
| Full update | `PUT` | `200 OK` |
| Partial update / state change | `PATCH` | `200 OK` or `204 No Content` |
| Delete | `DELETE` | `204 No Content` |
| Action (confirm, cancel) | `POST` | `200 OK` or `204 No Content` |
| Validation/verification | `POST` | `200 OK` or `204 No Content` |

### Error Responses

- Use `GlobalExceptionHandlerMiddleware` for unhandled exceptions.
- Return `Result.Fail()` from services; controllers map to appropriate HTTP status.
- Input validation via data annotations on request DTOs.
- Rate limiting configured: 10/min auth, 20/min validate, 120/min general.

---

## 7. Security Rules

- **JWT secrets** must come from environment variables — never hardcode.
- **Seed data** gated behind `IsDevelopment()` environment check.
- **Input validation** on all DTOs.
- **CORS** restricted to specific origins, methods, and headers.
- **Security headers** via `SecurityHeadersMiddleware`: `X-Content-Type-Options`, `X-Frame-Options`, `Strict-Transport-Security`.
- **Sensitive tokens** must not appear in logs.
- **PII** minimized in localStorage — only non-sensitive user data.
- **Rate limiting** on authentication and public endpoints.

---

## 8. Testing Standards

### Backend Testing (xUnit + Moq)

- **Location:** `tests/ReflectStudio.Application.Tests/`
- **Pattern:** Arrange-Act-Assert with descriptive test method names.
- **Database:** In-memory EF Core via `TestDbContextFactory.Create()`.
- **Mocking:** Moq for repository interfaces and external dependencies.
- **Coverage areas:** Booking lifecycle, availability calculation, pricing, promotions, domain services, specifications, entity behavior, event dispatch, middleware.
- **Naming convention:** `MethodName_Condition_ExpectedResult` or descriptive sentence style.
- **Run:** `cd tests && dotnet test ReflectStudio.Application.Tests/`

### Frontend Testing (Vitest)

- **Location:** Tests colocated with source or in `__tests__/` directories.
- **Config:** `vitest.config.ts` with jsdom environment, globals enabled.
- **Pattern:** Test Redux slices, selectors, utility functions, component rendering.
- **Run:** `cd client && npx vitest run`

### E2E Testing (Cypress)

- **Location:** `client/cypress/e2e/` (E2E specs) and `client/cypress/component/` (component tests).
- **Coverage:** Auth flows, booking flows, appointment management, social login.
- **Run:** `cd client && npx cypress run`

### Build Commands

```bash
# Backend build
cd src && dotnet build ReflectStudio.slnx

# Backend tests
cd tests && dotnet test ReflectStudio.Application.Tests/

# Frontend build
cd client && npm run build

# Frontend tests
cd client && npx vitest run

# Frontend lint
cd client && npm run lint
```

---

## 9. When Adding New Features

Follow this checklist for every new feature:

### Domain Layer (Core)

- [ ] Define entities with private setters and behavioral methods
- [ ] Place in the correct bounded context namespace
- [ ] Add domain events if the operation has side effects
- [ ] Create specifications for complex query predicates
- [ ] Use value objects for typed domain concepts
- [ ] Reference other aggregates by ID only

### Application Layer

- [ ] Create focused service(s) — max ~9 dependencies per service
- [ ] Split query and command operations into separate methods or services
- [ ] Use `Result<T>` for return values
- [ ] Create event handlers for domain events (one handler per event)
- [ ] Add DTOs for API contracts — never expose domain entities directly

### Infrastructure Layer

- [ ] Create query repository (with `.AsNoTracking()`) and command repository separately
- [ ] Add `IEntityTypeConfiguration<T>` for new entities — use Fluent API only
- [ ] Create and apply EF Core migration

### Web Layer

- [ ] Create thin controller using `[ServiceFilter(typeof(StudioScopedActionFilter))]` for admin endpoints
- [ ] Use MediatR command/query records for complex flows
- [ ] Apply appropriate `[Authorize(Policy = "...")]`
- [ ] Register new services in DI

### Frontend

- [ ] Create feature directory: `features/{name}/api/`, `model/`, `ui/`
- [ ] Redux slice with `createSlice` + `createAsyncThunk`
- [ ] Use `LoadStatus` for async state
- [ ] Container/Presenter split for components using Redux
- [ ] Respect import boundaries — no importing from upper layers
- [ ] Add i18n translation keys
- [ ] Use existing shared UI components (`shared/ui/`)

### Testing

- [ ] Backend: Unit tests for service logic, entity behavior, and specifications
- [ ] Frontend: Tests for Redux slices and utility functions
- [ ] Verify no regressions with existing test suites

---

## 10. Common Pitfalls to Avoid

1. **Never put business logic in controllers** — controllers delegate only.
2. **Never access `AppDbContext` from Application or Web layers** — use repository interfaces.
3. **Never use navigation properties across aggregate boundaries** — use IDs and separate queries.
4. **Never skip transactions for multi-step mutations** — especially booking + promotion operations.
5. **Never import from `app/hooks` in Presenter components** — only Containers may use Redux hooks.
6. **Never import from `features/` in `shared/`** — respect the layer hierarchy.
7. **Never call `SaveChangesAsync` multiple times** without a wrapping transaction — use Unit of Work.
8. **Never expose domain entities in API responses** — always map to DTOs.
9. **Never hardcode secrets** — use environment variables and configuration.
10. **Never use `EnsureCreated()`** — always use EF Core migrations.
11. **Never create a "god service"** — split services when they exceed ~9 dependencies.
12. **Never use string literals for booking status transitions** — use enums and `booking.CanTransitionTo()`.
13. **Never query inside a loop** — pre-load batch data before iteration (availability, reminders, notifications).
14. **Never exceed ~250 lines per component or ~150 lines per hook** — extract sub-components, hooks, or utilities.
15. **Never use raw `User.FindFirst(ClaimTypes.Email)?.Value`** — use `ClaimsPrincipalExtensions.GetEmail()` / `.GetRequiredEmail()`.
16. **Never use `err: any` in thunk error handling** — use `err: unknown` with `extractErrorMessage()`.
17. **Never use `GetRequiredService<T>()` service locator** inside services — use constructor injection.
18. **Never name sync operations `*Async`** — only methods with genuine async I/O get the `Async` suffix. Sync tracking operations (`Add`/`Update`/`Remove`) must not be `*Async`.
19. **Never use raw `code.ToUpperInvariant()` for promotion codes** — use `new PromotionCode(code).Value`.

---

## 11. Logging & Observability

- **Backend logging:** Serilog with `RenderedCompactJsonFormatter` for structured JSON output.
- **Bootstrap logger** configured before host build for early startup errors.
- **Request logging** via Serilog request logging middleware.
- **MediatR `LoggingBehavior`** logs execution time; warns if >500ms.
- **Health checks** registered and exposed.
- **Never log sensitive tokens or PII.**

---

## 12. Background Services

The application runs several hosted background services:

| Service | Purpose |
|---------|---------|
| `ExpiredDataCleanupService` | Expires pending bookings past the confirmation window |
| `NotificationProcessingService` | Processes the notification outbox (email dispatch) |
| `BookingReminderService` | Sends booking reminders based on configured schedules |
| `GalleryExpiryAndDeletionWorker` | Expires and deletes galleries past their expiry date |
| `ZipGenerationWorker` | Generates ZIP archives for gallery downloads |

Each service is single-purpose (SRP). New background tasks should follow the same pattern: implement `BackgroundService`, inject only the required services, and handle cancellation tokens.

**When adding new background services:**
- Follow the outbox pattern: write records to a queue table, let the processing service handle them. This ensures no work is lost if the app restarts.
- Background services that process collections must batch-load all required reference data upfront — never query per-item in a loop.
- All externally-dispatched messages (emails, webhooks) should be recorded in the outbox for audit trail, even on immediate success.

---

## 13. Concurrency & Data Integrity

- **Optimistic concurrency:** Row version tokens on `GiftCertificate` and `PromoCode` to prevent double-use.
- **Pessimistic locking:** Per-resource locks via `IResourceLocker` with provider-specific implementations: `pg_advisory_lock` (PostgreSQL), `sp_getapplock` (SQL Server), in-memory semaphore (SQLite).
- **Resource locks:** `ResourceLocks` table ensures lock row existence before acquiring database-level locks.
- **Booking creation** uses serializable isolation level transactions.
- Always propagate `CancellationToken` through async call chains.

### CancellationToken Propagation Rules (mandatory)

- **Every async interface method** MUST include `CancellationToken ct = default` as its final parameter.
- **Controllers** pass `HttpContext.RequestAborted` to `_mediator.Send(..., ct)` or service calls.
- **MediatR handlers** forward their `CancellationToken` to all downstream service calls.
- **Services** forward `ct` to every repository method call.
- **Repositories** forward `ct` to ALL EF Core async calls: `.ToListAsync(ct)`, `.FirstOrDefaultAsync(ct)`, `.SaveChangesAsync(ct)`, `.AnyAsync(ct)`, `.CountAsync(ct)`.
- **`SemaphoreSlim.WaitAsync()`** on `IResourceLocker` must accept `ct`.
- **Transaction APIs** (`BeginTransactionAsync`, `CommitAsync`) must receive `ct`.
- **Sync tracking operations** (`Add`/`Update`/`Remove`) must NOT have `Async` suffix — only genuinely async I/O methods use `Async`.

---

## 14. Domain Modeling Rules for New Features

### Time-Based Scheduling & Availability

When building any feature that involves scheduling or time slots:

- Model time as `SessionEnd = Start + Duration` and `BlockedEnd = SessionEnd + BufferMinutes`. Keep buffer/cleanup time separate from session duration.
- For availability calculation, **always pre-load all conflicts for the entire query range first**, then filter in memory. Never check conflicts per-slot in a loop.
- Store all times in UTC. Convert to local timezone only at the boundary: use `TimeZoneInfo.ConvertTimeFromUtc(date, tz).Date` before extracting `DayOfWeek` or comparing local dates.

### Price & Snapshot Rules

When any feature involves pricing:

- **Snapshot prices at creation time** — record the price at the moment of the transaction as immutable fields on the entity. Never recompute from the current catalog later.
- Price calculation client-side is for UX preview only. **Backend must always re-validate and snapshot** (single source of truth).
- All monetary snapshots must validate `>= 0`. Apply floor (`Math.Max(0, ...)`) when subtracting discounts.
- Use a **pure domain service** for price calculation (no I/O, no infrastructure dependencies).

### Status Machine Pattern

When any entity has a lifecycle with status transitions:

- Define all statuses as an **enum**, never strings.
- Implement a `CanTransitionTo(newStatus)` guard method on the aggregate root. The aggregate enforces which transitions are valid.
- Controllers and services must call `CanTransitionTo()` — never duplicate transition validation logic outside the aggregate.
- If a concern (like payment) is **orthogonal** to the main status flow, model it as a separate flag/method (e.g., `MarkPaid()` independent of status).

### Soft References Across Contexts

When an entity in one context references data in another:

- Use **ID-only references** (soft FKs) — the referenced entity may be deleted later.
- **Snapshot any data the entity needs to survive independently** (e.g., image URLs, names, prices) at creation time.

### Single-Use Resource Rules

When building features with single-use resources (certificates, vouchers, codes):

- Use **concurrency tokens** (row version) to prevent double-use in concurrent requests.
- Wrap redemption in an explicit **transaction** together with the consuming operation.
- On cancellation of the consuming operation, call `RestoreAfterCancellation()` — restoration logic must exist on both admin and client code paths.
- Normalize user-provided codes via **value objects** — never use raw string manipulation.

### Configurable Limits & Expiration

When adding features with per-user limits or auto-expiration:

- Make limits **configurable** via studio settings (not hardcoded constants).
- Implement auto-expiration via a **background service** that runs periodically — don't rely on request-time checks alone.
- Check limits at creation time and enforce them in the domain/service layer, not in controllers.

---

## 15. UI & Accessibility Standards

### UI Library

- **shadcn/ui** (Radix primitives + Tailwind CSS 4 + copy-paste ownership in `shared/ui/`).
- Components are owned copies — modify freely, no upstream dependency lock.
- Use `cn()` utility (tailwind-merge + clsx) for class merging. Use `class-variance-authority` (CVA) for component variants.
- When adding a new interactive component, check if a shadcn/ui primitive exists first before building from scratch.

### WCAG 2.1 AA Compliance (mandatory for all new UI)

- **Modals/Dialogs:** Must have focus trapping, `role="dialog"` or `role="alertdialog"`, escape key dismissal.
- **Interactive elements:** Must have visible `focus:ring-2 focus:ring-brand` focus rings.
- **Expandable content:** Must use `aria-expanded` and support keyboard navigation (arrow keys).
- **Heading hierarchy:** Must be semantic (`h1 → h2 → h3`), never skip levels.
- **Images:** Must have `alt` text. Icon-only buttons must have `aria-label`.
- **Auxiliary form controls** (e.g., password visibility toggle): Use `tabIndex={-1}` to avoid disrupting the main form tab order.
- **Multi-step flows:** Use `role="progressbar"` with `aria-valuenow`/`aria-valuemax`.
- **Frontend role guards are UX only** — backend `[Authorize(Policy)]` is the real security boundary.

### Design Tokens

Always use the project's CSS custom properties — never hardcode color/spacing values:

| Token | Value | Usage |
|-------|-------|-------|
| `--color-brand` | `#ffc347` | CTAs, highlights, badges |
| `--color-brand-dark` | `#e6b800` | Hover/active states |
| `--color-brand-light` | `#fff8e1` | Focus rings, subtle backgrounds |
| `--color-brand-text` | `#1a1a1a` | Text on brand-colored backgrounds |
| `--color-surface-900` | `#0a0a0a` | Darkest background |
| `--color-surface-800` | `#141414` | Card backgrounds |
| `--color-surface-700` | `#1e1e1e` | Elevated surfaces |
| `--color-surface-600` | `#2a2a2a` | Borders |

**Brand color rule:** Use for CTAs, highlights, badges. **Never** for large background fills, body text, or decorative elements.

### Typography Scale

Use these established scales — never invent custom sizes:

| Element | Classes |
|---------|---------|
| Hero H1 | `text-4xl sm:text-5xl lg:text-6xl font-bold leading-[1.1] tracking-tight` |
| Section H2 | `text-3xl sm:text-4xl font-bold` |
| Card H3 | `text-xl font-bold` |
| Body | `text-base leading-relaxed` |
| Caption | `text-sm` |

### Component Standards

All new components must follow these conventions:

- **Icons:** SVG line icons, `strokeWidth="1.5"`, consistent `w-6 h-6` or `w-7 h-7`. No emoji anywhere in UI.
- **Cards:** `rounded-xl p-5` or `p-6`, always with border (`border border-zinc-100 dark:border-zinc-700`). Hover: `hover:border-brand/20` or `hover:shadow-md`.
- **Buttons:** `rounded-lg` (never `rounded-full`), consistent `py-2.5 font-semibold`. Every submit/action button must show a spinner + be disabled during loading.
- **Loading states:** Use skeleton loaders for data-dependent content. Never show blank white space during fetch.
- **Empty states:** Always provide a clear empty state with an action prompt when a list/collection has no items.

### Motion Rules

- **Allowed animations:** Fade-up on load, IntersectionObserver scroll-reveal, button hover lift, gallery hover zoom, button active press.
- **`prefers-reduced-motion: reduce` must be respected:** Disable all transforms/transitions. Keep only `animate-spin` (essential loading feedback). Scroll-reveal becomes instant `opacity:1` with no transform.

---

## 16. Notification & Email Patterns

### Rules for Adding New Notifications

1. **Choose the right path:**
   - **Instant** (fire-and-forget + outbox fallback): For user-triggered events where the user expects immediate confirmation.
   - **Scheduled** (outbox-only): For automated, time-based notifications (reminders, expiry warnings).

2. **Use the existing composition helpers** — never duplicate the render+queue pattern. Call `InstantNotificationComposer` for instant emails.

3. **Every email must be recorded in the outbox** — even on immediate success (`Status=Sent`). This provides an audit trail and enables admin visibility.

4. **Failure must never lose an email.** On send failure, save the outbox entry as `Status=Pending` for background retry by `NotificationProcessingService`.

5. **Keep the `IEmailSender` abstraction.** Swap providers (Resend, SMTP) via conditional DI — consumers must never depend on a concrete provider.

### Email Security Rules

- **No email enumeration:** Verification and password-reset endpoints must return 200 regardless of whether the email exists.
- **Rate limit re-send endpoints** (1 per 60s per email for verification resend).
- **Social login users are auto-confirmed** — skip verification email. Only form-registered users require email verification.

### When Adding "Share via Email" Features

- Admin must explicitly trigger sharing — never auto-send on entity creation.
- Track `LastSharedAtUtc` on the entity. If sensitive data (e.g., passwords) changed since last share, show a warning and offer to resend.

---

## 17. File Upload, Streaming & Protected Content Rules

### File Upload Pattern

When building any feature with file uploads:

- **Register each file individually** as soon as its upload completes — don't wait for a batch confirmation step.
- **Show optimistic previews** via `URL.createObjectURL(file)` immediately in the UI, then replace with the server URL after registration succeeds.
- **Handle unsupported preview formats gracefully** — if the browser can't generate a thumbnail (e.g., RAW camera files), send `width: 0, height: 0` to the backend for server-side extraction and show a placeholder icon.

### Streaming Downloads

When building bulk download features:

- **Stream on-demand** (e.g., `ZipArchive` over `Response.Body`) — don't pre-generate archives unless the collection is very large (500+ items).
- For large collections, check for a cached pre-generated archive first; redirect to CDN if valid.

### Password-Protected Content

When building features with access-controlled content:

- Use a **one-time verification endpoint** that grants an HttpOnly cookie on success. Subsequent requests are validated by cookie — never send passwords per-request in headers.
- When password protection is toggled off and back on, **always generate a fresh password** — never reuse the old one.

### Entity Design Config Pattern

When an entity has user-customizable display settings (themes, layouts, fonts):

- Store config as flat properties on the entity — not as a serialized JSON blob.
- **Always provide defaults for backward compatibility** — use nullish coalescing so existing records render correctly without migration.
- Inject config as CSS custom properties on the public-facing page.

### SEO for SPA Public Pages

- Use server-side **middleware** to intercept public routes and inject `<meta og:*>` tags into the cached `index.html` template before `</head>`. No SSR framework needed.

### Cleanup & Deletion

- **Synchronous:** Remove DB records immediately.
- **Asynchronous:** Queue blob/file storage cleanup as a background job — don't block the user request on external storage deletion.
