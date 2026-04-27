# Reflect Studio — GitHub Copilot Instructions

> Auto-loaded by GitHub Copilot for every interaction in this repository.

## Technology Stack

| Layer | Technology |
|-------|-----------|
| Backend | ASP.NET Core .NET 10 · EF Core · PostgreSQL · MediatR |
| Frontend | React 19 · TypeScript (strict) · Redux Toolkit · Vite · Tailwind CSS 4 |
| UI | shadcn/ui (Radix UI) — owned copies in `shared/ui/` |
| Auth | ASP.NET Identity + JWT + refresh tokens |
| i18n | i18next + react-i18next (EN + RO, default RO) |
| Testing | xUnit + Moq (backend) · Vitest + Cypress (frontend) |

## Architecture

### Backend — Clean Architecture

```
SharedKernel → Core → Application → Infrastructure → Web
```

- **Controllers** delegate to `_mediator.Send(command, ct)` — zero business logic.
- **Services/handlers** return `Result<T>` — never throw for domain validation.
- **Query repos** use `.AsNoTracking()` + `.Select()` projections.
- **Command repos** load full tracked entities.
- **EF Core** configured via Fluent API only (`IEntityTypeConfiguration<T>`).
- **CancellationToken** on every async method, forwarded through the entire chain.

### Frontend — Feature-Sliced Design

```
src/
  app/          ← Providers, router, store setup
  pages/        ← Thin page assemblies
  features/{name}/api/ model/ ui/
  shared/api/ ui/ lib/ test/
```

**Import rules (never violate):**
- `shared/` cannot import from `features/` or `pages/`.
- `features/` cannot import from other `features/`.
- Cross-feature reactions go in `app/listeners/`.

### Container / Presenter Pattern

- **Container**: reads Redux state, dispatches actions, passes props to Presenter.
- **Presenter**: pure props-only, no Redux hooks, renders UI.
- File names: `BookingWizardContainer.tsx` wraps `BookingWizard.tsx`.

## Coding Conventions

### Backend (C#)

```csharp
// Controller — primary constructor, MediatR delegation
[ApiController]
[Route("api/admin/{studioId:guid}/bookings")]
[Authorize(Policy = "AdminOnly")]
[ServiceFilter(typeof(StudioScopedActionFilter))]
public class BookingsController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(Guid studioId, CreateBookingDto dto)
    {
        var result = await mediator.Send(new CreateBookingCommand(studioId, dto), HttpContext.RequestAborted);
        return result.Success ? Created() : BadRequest(result.Error);
    }
}

// Handler — IRequestHandler<TRequest, Result<T>>
public class CreateBookingHandler(IBookingCommandRepository repo)
    : IRequestHandler<CreateBookingCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateBookingCommand req, CancellationToken ct)
    {
        // domain logic here
        return Result<Guid>.Ok(booking.BookingId);
    }
}

// Entity — private setters, behavioral methods, status machine
public class Booking : EntityBase
{
    public bool CanTransitionTo(BookingStatus newStatus)
        => _allowedTransitions[Status].Contains(newStatus);
}
```

### Frontend (TypeScript)

```typescript
// Async thunk — err: unknown + extractErrorMessage
export const fetchBookings = createAsyncThunk(
  'bookings/fetch',
  async (studioId: string, { rejectWithValue }) => {
    try {
      const { data } = await api.get<BookingDto[]>(`/admin/${studioId}/bookings`);
      return data;
    } catch (err: unknown) {
      return rejectWithValue(extractErrorMessage(err, 'Failed to load bookings'));
    }
  }
);

// Slice state shape — always include LoadStatus
interface BookingsState {
  data: BookingDto[];
  status: LoadStatus;
  error: string | null;
}

// Container component — uses Redux hooks
export default function BookingsContainer() {
  const dispatch = useAppDispatch();
  const { data, status, error } = useAppSelector(s => s.bookings);
  return <BookingsList bookings={data} status={status} error={error} />;
}

// Presenter component — pure props only, NO useAppDispatch/useAppSelector
function BookingsList({ bookings, status, error }: BookingsListProps) {
  // render only
}
```

## Domain Rules

- **Status machines:** enum-based with `CanTransitionTo()` guard on aggregate root.
- **Prices:** snapshot at booking creation — never recompute from catalog.
- **Time:** store UTC, convert at display boundary. Use `TimeZoneInfo.ConvertTimeFromUtc()`.
- **Single-use resources:** concurrency tokens + explicit transactions.
- **Promo codes:** normalize via `new PromotionCode(code).Value` — never raw `ToUpperInvariant()`.
- **Cross-context references:** ID-only soft foreign keys + snapshot data at creation.

## UI Standards

- **Design tokens:** `--color-brand: #ffc347` for CTAs only — never for large fills.
- **Components:** `rounded-xl p-5`, `border border-zinc-100 dark:border-zinc-700`.
- **Buttons:** `rounded-lg py-2.5 font-semibold` with loading spinner + disabled state.
- **Icons:** SVG line icons, `strokeWidth="1.5"`, `w-6 h-6`.
- **Class merging:** `cn()` (tailwind-merge + clsx). CVA for variants.
- **WCAG 2.1 AA:** focus rings, aria labels, heading hierarchy, keyboard navigation.
- **Motion:** respect `prefers-reduced-motion: reduce`.
- **Max component size:** 250 lines. Max hook size: 150 lines.

## Security

- JWT secrets from environment variables — never hardcode.
- Input validation on all DTOs.
- CORS restricted to specific origins.
- No email enumeration — return 200 regardless.
- Rate limiting: 10/min auth, 20/min validate, 120/min general.
- Frontend role guards are UX only — backend `[Authorize(Policy)]` is the real boundary.

## Common Pitfalls (never do these)

1. Business logic in controllers
2. `AppDbContext` access from Application or Web layers
3. Navigation properties across aggregate boundaries
4. Skipping transactions for multi-step mutations
5. Importing `app/hooks` in Presenter components
6. Importing `features/` from `shared/`
7. Multiple `SaveChangesAsync` without wrapping transaction
8. Exposing domain entities in API responses
9. Hardcoded secrets
10. `err: any` in thunk error handling
11. Raw `User.FindFirst(ClaimTypes.Email)` — use `ClaimsPrincipalExtensions.GetEmail()`
12. String literals for booking status — use enums + `CanTransitionTo()`
13. Querying inside loops — pre-load batch data
14. Components over 250 lines or hooks over 150 lines

## Skill Files

Granular pattern and role-based skills live in `.claude/skills/`. Each covers one focused topic:

**Domain:** `domain-entity-template.md`, `domain-status-machine.md`, `domain-value-objects.md`, `domain-price-snapshots.md`, `domain-time-utc.md`

**Backend:** `backend-cqrs-handler.md`, `backend-result-pattern.md`, `api-controller-conventions.md`, `cancellation-token-discipline.md`, `multi-tenancy-studio-scope.md`

**EF Core:** `ef-core-query-repository.md`, `ef-core-command-repository.md`, `ef-core-entity-configuration.md`, `ef-core-transactions.md`, `ef-core-avoid-contains.md`

**Frontend:** `frontend-feature-slice.md`, `redux-async-state.md`, `frontend-error-handling.md`, `cross-feature-state-reset.md`

**UI:** `ui-design-tokens.md`, `ui-component-patterns.md`, `ui-accessibility-wcag.md`, `i18n-conventions.md`

**Infrastructure:** `background-service-template.md`, `outbox-pattern.md`, `security-checklist.md`

**Testing:** `testing-backend-xunit.md`, `testing-frontend-vitest.md`, `testing-e2e-cypress.md`

**Roles:** `role-backend-engineer.md`, `role-frontend-engineer.md`, `role-ux-designer.md`, `role-quality-assurance.md`, `role-product-delivery-manager.md`, `role-devops.md`, `role-studio-manager.md`

## Build & Test Commands

```bash
cd src && dotnet build ReflectStudio.slnx          # Backend build
cd tests && dotnet test ReflectStudio.Application.Tests/  # Backend tests
cd client && npm run build                          # Frontend build
cd client && npx vitest run                         # Frontend tests
cd client && npm run lint                           # Frontend lint
```
