# Skill: Security Checklist

> Run through this checklist for every code change that touches auth, data access, or API endpoints.

## When to Apply

- Adding or modifying API endpoints
- Changing authentication or authorization logic
- Handling user input or file uploads
- Modifying data access patterns
- Adding new environment configuration

---

## Authentication & Authorization

- [ ] Admin routes: `[Authorize(Policy = "AdminOnly")]` + `StudioScopedActionFilter`
- [ ] Client routes: `[Authorize(Policy = "EmployeeOrAdmin")]`
- [ ] Public routes: no `[Authorize]` — verify this is intentional
- [ ] Frontend role guards are UX only — backend policy is the real boundary
- [ ] MFA enforced for admin accounts (`AdminMfaRequiredPolicy`)
- [ ] Never use `User.FindFirst(ClaimTypes.Email)?.Value` — use `ClaimsPrincipalExtensions`

## Input Validation

- [ ] All DTOs have validation attributes or are validated in the handler
- [ ] String inputs: `MaxLength`, `Required` where appropriate
- [ ] Numeric inputs: range validation (especially `>= 0` for prices)
- [ ] GUID inputs: validated as route constraints (`{id:guid}`)
- [ ] Email inputs: validated format before use
- [ ] No SQL injection vectors (EF Core parameterizes, but verify raw SQL)
- [ ] File uploads: validated type, size, and content

## Secrets & Configuration

- [ ] JWT secrets from environment variables — never hardcoded
- [ ] Connection strings from configuration — never in code
- [ ] API keys, tokens, passwords: never in source code
- [ ] Seed data gated behind `IsDevelopment()`
- [ ] Sensitive tokens never appear in logs (Serilog configured)

## Data Access

- [ ] Multi-tenancy enforced: `StudioId` on all studio-scoped queries
- [ ] `StudioScopedActionFilter` on admin controllers
- [ ] Users can only access their own data (verified by policy or filter)
- [ ] No direct `AppDbContext` access from Application or Web layers

## API Security

- [ ] CORS restricted to specific origins — not `*`
- [ ] Rate limiting applied: 10/min auth, 20/min validate, 120/min general
- [ ] Security headers middleware active: `X-Content-Type-Options`, `X-Frame-Options`, `HSTS`
- [ ] No email enumeration: verification/password-reset return 200 regardless

## Frontend Security

- [ ] Bearer token stored in `localStorage` (acceptable for this app)
- [ ] PII minimized in localStorage — only non-sensitive user data
- [ ] Axios interceptor adds `Authorization: Bearer` header
- [ ] 401 response triggers token refresh (single retry)
- [ ] Failed refresh redirects to login
- [ ] Public endpoints skip auth headers (`isPublicEndpoint()` check)
- [ ] Correlation IDs for tracing — no sensitive data in headers

## Concurrency & Data Integrity

- [ ] Single-use resources (certificates, promo codes) have concurrency tokens
- [ ] Redemption wrapped in explicit transaction
- [ ] Multi-step mutations in single transaction — no partial commits
- [ ] Optimistic locking for concurrent access patterns

## Background Services

- [ ] Service handles cancellation tokens properly
- [ ] Outbox pattern for email/notification dispatch
- [ ] Failures saved with `Status=Pending` for retry — never lost
- [ ] No sensitive data in outbox message payloads
