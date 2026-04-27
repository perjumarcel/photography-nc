# Role Skill: Product Delivery Manager

> Encode feature planning, sprint workflow, and delivery standards for Reflect Studio.

## Role Context

The Product Delivery Manager ensures features are delivered end-to-end across all layers, following the established checklist, maintaining quality gates, and coordinating cross-cutting concerns. This role thinks in terms of completeness, risk, and user value.

---

## New Feature Delivery Checklist

Every feature must work through **all layers in order**. Never skip a layer.

### 1. Domain Layer (Core)

- [ ] Entity defined with private setters and behavioral methods
- [ ] Placed in correct bounded context namespace
- [ ] Domain events for side-effect-producing operations
- [ ] Specifications for complex query predicates
- [ ] Value objects for typed domain concepts
- [ ] Cross-aggregate references by ID only
- [ ] Prices/data snapshotted at creation time

### 2. Application Layer

- [ ] Focused service — max ~9 dependencies
- [ ] Split query and command operations
- [ ] `Result<T>` from all service methods
- [ ] Domain event handlers (one per event)
- [ ] DTOs for API contracts — never expose domain entities

### 3. Infrastructure Layer

- [ ] Query repository (`.AsNoTracking()` + projections)
- [ ] Command repository (tracked entities)
- [ ] `IEntityTypeConfiguration<T>` via Fluent API
- [ ] EF Core migration generated and tested
- [ ] DI registration for new services/repositories

### 4. Web Layer

- [ ] Thin controller — `[ServiceFilter(typeof(StudioScopedActionFilter))]` for admin
- [ ] MediatR command/query records
- [ ] Appropriate `[Authorize(Policy)]`
- [ ] Rate limiting applied
- [ ] OpenAPI/Swagger documentation (if applicable)

### 5. Frontend

- [ ] FSD structure: `features/{name}/api/`, `model/`, `ui/`
- [ ] Redux slice with `LoadStatus` state shape
- [ ] Container/Presenter split
- [ ] All four states handled: idle, loading, success, failed
- [ ] Empty states for empty lists
- [ ] Skeleton loaders for async content
- [ ] i18n keys for EN + RO
- [ ] Vitest tests for slice and utilities
- [ ] `data-testid` from `testIds.ts` for E2E
- [ ] Responsive design (mobile-first)
- [ ] Accessibility (WCAG 2.1 AA)

### 6. Cross-Cutting

- [ ] Security review (see `security-checklist.md`)
- [ ] Error handling on both layers
- [ ] Logout cleanup registered in `app/listeners/logoutListener.ts`
- [ ] Background services for async operations (if applicable)
- [ ] Documentation updated (if significant)

---

## Bounded Context Ownership

| Context | Backend Scope | Frontend Feature |
|---------|-------------|-----------------|
| BookingContext | Booking lifecycle, availability, pricing | `features/booking/` |
| StudioContext | Studio profile, rooms, equipment, hours | `features/studio/`, `features/admin/` |
| ClientContext | Client profiles, contact info | `features/admin/` (client management) |
| PromotionContext | Promo codes, gift certificates | `features/certificates/` |
| GalleryContext | Photo galleries, sharing, expiry | `features/galleries/` |
| NotificationContext | Email outbox, templates, retry | Backend only (no dedicated frontend) |
| PrintingContext | Print formats, orders, ZIP | `features/prints/` |
| IdentityContext | Auth, JWT, social login, MFA | `features/auth/`, `features/profile/` |

---

## Risk Assessment

### High-Risk Changes (require extra review)

- Booking creation/cancellation (transactions, concurrency, promotions)
- Payment-related flows (price snapshots, currency handling)
- Authentication/authorization changes
- Database migrations on production data
- Background service modifications (data loss potential)
- Multi-tenant data access (StudioId scoping)

### Medium-Risk Changes

- New API endpoints (security, rate limiting)
- Redux state shape changes (migration of persisted state)
- i18n key restructuring (missing translations)
- UI component library changes (regression across consumers)

### Low-Risk Changes

- New translations (both languages provided)
- CSS/styling adjustments (visual only)
- Test additions (no production code change)
- Documentation updates

---

## Sprint Workflow

### Feature Development Flow

```
RFC (docs/rfcs/todo/) → In Progress (docs/rfcs/in-progress/) → Done (docs/rfcs/done/)
```

1. **RFC** — Define the feature scope, technical approach, and acceptance criteria
2. **Domain Design** — Entity modeling, status machines, event design
3. **Backend Implementation** — Service → Repository → Controller → Tests
4. **Frontend Implementation** — Slice → API → Container → Presenter → Tests
5. **Integration Testing** — E2E with Cypress
6. **Security Review** — Run through security checklist
7. **Code Review** — Verify all layers, patterns, and conventions
8. **Deployment** — Migration → Backend → Frontend

### Quality Gates

| Gate | Criteria |
|------|---------|
| Code Review | All patterns followed, no anti-patterns |
| Backend Tests | All passing, coverage for new logic |
| Frontend Tests | Vitest passing, slice tests complete |
| Build | Both `dotnet build` and `npm run build` succeed |
| Lint | `npm run lint` passes |
| Security | Security checklist completed |
| i18n | Both EN + RO translations provided |

---

## Documentation Standards

| Document Type | Location | When |
|--------------|----------|------|
| RFC | `docs/rfcs/{status}/` | Before major features |
| ADR | `docs/architecture/` | Architecture decisions |
| Audit | `docs/audits/{domain}/` | Security, UX, backend reviews |
| Sprint Report | `docs/sprints/` | Per sprint |
| Runbook | `docs/operations/` | Operational procedures |

## Checklist (every feature delivery)

- [ ] All 6 layers addressed (Domain → Application → Infrastructure → Web → Frontend → Cross-Cutting)
- [ ] RFC written for significant features
- [ ] Risk assessment completed
- [ ] All quality gates passed
- [ ] Both languages have translations
- [ ] Security checklist reviewed
- [ ] Feature tested end-to-end
