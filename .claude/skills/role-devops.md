# Role Skill: DevOps Engineer

> Encode CI/CD, deployment, infrastructure, and operational conventions for Reflect Studio.

## Role Context

The DevOps Engineer manages the build pipeline, deployment targets, database migrations, monitoring, and operational procedures. This role ensures reliable, repeatable deployments and infrastructure hygiene.

---

## Deployment Topology

| Component | Platform | Details |
|-----------|----------|---------|
| Backend API | Fly.io | Docker container, ASP.NET Core .NET 10 |
| Frontend SPA | Vercel | Static build, React 19 + Vite |
| Database | Neon | Managed PostgreSQL (serverless) |
| Blob Storage | Cloudflare R2 | S3-compatible object storage |
| Email | Resend | Production email delivery |
| Email (dev) | Gmail SMTP | Development fallback |

---

## CI/CD Pipeline

### GitHub Actions Workflows

| Workflow | Trigger | Steps |
|----------|---------|-------|
| `ci.yml` | Push / PR | Restore → Build → Test → Lint |
| `e2e.yml` | PR / Manual | Frontend E2E with Cypress |

### Build Commands (verified)

```bash
# Backend
cd src && dotnet build ReflectStudio.slnx

# Backend tests
cd tests && dotnet test ReflectStudio.Application.Tests/
cd tests && dotnet test ReflectStudio.Core.Tests/
cd tests && dotnet test ReflectStudio.Infrastructure.Tests/
cd tests && dotnet test ReflectStudio.Web.Tests/

# Frontend (use CYPRESS_INSTALL_BINARY=0 to skip Cypress in CI build step)
cd client && CYPRESS_INSTALL_BINARY=0 npm install
cd client && npm run build
cd client && npx vitest run
cd client && npm run lint

# E2E
cd client && npx cypress run
```

### CI Environment Variables

| Variable | Purpose | Where |
|----------|---------|-------|
| `VITE_API_URL` | Backend API base URL | Frontend build |
| `ConnectionStrings__DefaultConnection` | PostgreSQL connection | Backend |
| `JwtSettings__Secret` | JWT signing key | Backend (never hardcode) |
| `Resend__ApiKey` | Email provider API key | Backend |
| `R2__*` | Cloudflare R2 credentials | Backend |
| `CYPRESS_INSTALL_BINARY` | Set to `0` to skip Cypress binary in non-E2E jobs | CI |

---

## Database Migrations

### Creating a Migration

```bash
cd src && dotnet ef migrations add <MigrationName> \
  --project ReflectStudio.Infrastructure \
  --startup-project ReflectStudio.Web
```

### Migration Rules

1. **Never use `EnsureCreated()`** — always migrations.
2. **Review generated SQL** before applying to production.
3. **Zero-downtime migrations**: add columns as nullable first, backfill, then add constraints.
4. **Never drop columns** without a deprecation period.
5. **Test migration up AND down** in development.
6. Migrations run automatically on application startup (in dev) or explicitly in production.

### Dangerous Migration Patterns

| Pattern | Risk | Mitigation |
|---------|------|-----------|
| Drop column | Data loss | Add deprecation period, backup first |
| Rename column | Breaks running instances | Add new → migrate → drop old |
| Add NOT NULL without default | Fails on existing data | Add with default, then remove default |
| Large table ALTER | Lock contention | Schedule maintenance window |

---

## Docker Configuration

### Backend Dockerfile Expectations

```dockerfile
# Multi-stage build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish ReflectStudio.Web -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app .
EXPOSE 8080
ENTRYPOINT ["dotnet", "ReflectStudio.Web.dll"]
```

### Environment Configuration

- **Development**: `ASPNETCORE_ENVIRONMENT=Development` — enables seed data, detailed errors
- **Production**: `ASPNETCORE_ENVIRONMENT=Production` — security headers, minimal logging
- Seed data gated behind `IsDevelopment()` — never runs in production

---

## Monitoring & Observability

### Logging

- **Serilog** with `RenderedCompactJsonFormatter` for structured JSON
- **Bootstrap logger** configured before host build
- **LoggingBehavior** warns if MediatR handler exceeds 500ms
- **Never log**: JWT tokens, passwords, PII, API keys

### Health Checks

- Health check endpoints registered and exposed
- Database connectivity check
- External service connectivity (email, storage)

### Error Tracking

- **Backend**: Sentry v6.1.0 at 20% trace sampling
- **Frontend**: No Sentry yet (planned)

### Correlation IDs

End-to-end tracing:
1. Frontend axios interceptor generates 12-char hex ID
2. `X-Correlation-Id` header sent to backend
3. `CorrelationIdMiddleware` reads/generates ID
4. Serilog enriches all log entries
5. ID included in response headers
6. ProblemDetails includes correlation ID

---

## Security Headers

Configured via `SecurityHeadersMiddleware`:

```
X-Content-Type-Options: nosniff
X-Frame-Options: DENY
Strict-Transport-Security: max-age=31536000; includeSubDomains
```

### CORS Configuration

- Restricted to specific origins (not `*`)
- Allowed headers include `X-Correlation-Id`
- Exposed headers include `X-Correlation-Id`

### Rate Limiting

| Policy | Limit | Target |
|--------|-------|--------|
| `auth` | 10/min | Login, register, password reset |
| `validate` | 20/min | Email verification, code validation |
| `general` | 120/min | All other endpoints |

---

## Apple Sign-In Key Rotation

Documented in `docs/operations/APPLE_KEY_ROTATION.md`. Follow the runbook for key rotation procedures.

---

## Operational Runbooks

| Runbook | Location |
|---------|----------|
| Apple key rotation | `docs/operations/APPLE_KEY_ROTATION.md` |
| Agent prompts | `docs/operations/` |
| Database backup/restore | Follow Neon platform procedures |
| Incident response | Correlation ID → Serilog → Sentry |

## Checklist (every deployment)

- [ ] All tests pass (backend + frontend)
- [ ] Build succeeds on both layers
- [ ] Database migration reviewed (if any)
- [ ] Environment variables configured
- [ ] Security headers active
- [ ] Rate limiting configured
- [ ] Health checks responsive
- [ ] No secrets in source code or logs
- [ ] Seed data only in Development environment
- [ ] CORS origins correct for target environment
