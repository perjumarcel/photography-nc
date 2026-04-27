# Role Skill: Quality Assurance & Tester

> Role perspective for testing strategy, coverage standards, and regression prevention.

## Role Context

The QA/Tester ensures code quality through comprehensive testing at every layer: unit, integration, and E2E.

## Testing Pyramid

```
         ╱ E2E (Cypress) ╲           ← Few: critical user flows
        ╱  Integration     ╲          ← Some: API + component integration
       ╱   Unit (xUnit/Vitest) ╲      ← Many: logic, reducers, utilities
```

## What Must Be Tested

### Backend (Critical → Medium)

| Area | Priority |
|------|----------|
| Entity constructors + validation | Critical |
| Status transitions (valid + invalid) | Critical |
| Booking lifecycle (create → confirm → complete/cancel) | Critical |
| Availability calculation (overlap detection) | Critical |
| Price calculation (discounts, snapshots) | Critical |
| Promotion redemption (concurrent access) | Critical |
| Service methods (success + failure paths) | High |
| MediatR handlers (delegation) | Medium |
| Repository queries (projection correctness) | Medium |
| Background services (batch processing) | Medium |

### Frontend (Critical → Medium)

| Area | Priority |
|------|----------|
| Redux slices — all reducer cases | Critical |
| Async thunks — pending/fulfilled/rejected | Critical |
| Reset actions — return to initial state | Critical |
| Utility functions — edge cases | High |
| Selectors — memoized computation | Medium |
| Component rendering — critical paths | Medium |

## Regression Prevention

### Before Every PR

```bash
cd tests && dotnet test                    # Backend — all projects
cd client && npx vitest run                # Frontend — all tests
cd client && npm run lint                  # Lint — zero warnings
cd src && dotnet build ReflectStudio.slnx  # Backend build
cd client && npm run build                 # Frontend build
```

### Common Regression Risks

| Change | Risk | Mitigation |
|--------|------|-----------|
| Redux state shape change | Persisted state mismatch | Test reset action, check localStorage |
| API contract change | Frontend/backend mismatch | Update DTOs on both sides |
| EF Core migration | Data loss, schema drift | Test migration up/down |
| i18n key rename | Missing translations | Verify both EN + RO files |
| Shared component change | Breaks consumers | Test all 14+ Modal consumers |

## Consult These Skills

| Task | Skill |
|------|-------|
| Backend tests | `testing-backend-xunit.md` |
| Frontend tests | `testing-frontend-vitest.md` |
| E2E tests | `testing-e2e-cypress.md` |
| Security review | `security-checklist.md` |

## Checklist (every PR)

- [ ] All existing tests pass (backend + frontend)
- [ ] New logic has corresponding tests
- [ ] Slice tests cover pending/fulfilled/rejected + reset
- [ ] Backend tests use Arrange-Act-Assert + Moq
- [ ] Test naming follows `MethodName_Condition_Expected`
- [ ] No test imports from other features (FSD boundaries)
- [ ] `data-testid` from centralized `testIds.ts`
- [ ] Build succeeds on both layers
- [ ] Lint passes with zero warnings
