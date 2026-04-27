# Skill: E2E Testing (Cypress)

> Cypress conventions, centralized test IDs, and critical flow coverage.

## When to Apply

- Adding or modifying user-facing flows
- Ensuring end-to-end integration works correctly

## Test Organization

```
client/cypress/
  e2e/              ← Test specs
  fixtures/         ← Mock data (JSON)
  support/          ← Commands, setup
```

## Test ID Convention

```typescript
// Use centralized test IDs from shared/test/testIds.ts
cy.get(`[data-cy="${TEST_IDS.loginEmail}"]`).type('test@example.com');
cy.get(`[data-cy="${TEST_IDS.loginSubmit}"]`).click();

// API mocking for isolated E2E
cy.intercept('POST', '/api/auth/login', { fixture: 'login-success.json' }).as('login');
cy.wait('@login');
```

## Critical E2E Flows

| Flow | Priority |
|------|---------|
| Login (email + password) | Critical |
| Registration | Critical |
| Social login (Google, Facebook, Apple) | Critical |
| Booking wizard (Service → DateTime → AddOns → Review) | Critical |
| Admin calendar (day/week views, status changes) | High |
| Gallery viewing (access token, password) | High |
| Certificate purchase | High |
| Print order | Medium |

## Run Commands

```bash
cd client && npx cypress run     # Headless
cd client && npx cypress open    # Interactive
```

## Checklist

- [ ] Test IDs from centralized `testIds.ts` — not hardcoded selectors
- [ ] API calls mocked with `cy.intercept()`
- [ ] Critical flows covered (auth, booking, gallery)
- [ ] `cy.wait('@alias')` for async assertions
