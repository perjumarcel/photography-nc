# Covercenco Migration — Progress Tracker

Living document tracking the migration of the legacy ASP.NET MVC photography
portfolio (under `legacy/`) to the modern .NET 10 + React/Redux stack
(`src/Photography.*` and `client/`).

Update this file whenever a checklist item changes state. Use:

- `[x]` — done and validated
- `[~]` — in progress / partially landed
- `[ ]` — not started

Each section links to the supporting code or doc. Keep entries brief; deep
detail belongs in `docs/architecture/` or `docs/operations/`.

---

## 1. Discovery & planning

- [x] Inspect legacy project structure (`legacy/`)
- [x] Document target architecture — `docs/architecture/solution.md`
- [x] Define target DB schema — `docs/architecture/postgres-schema.md`
- [x] Define storage model — `docs/architecture/storage-r2.md`
- [x] Cutover & rollback plan — `docs/architecture/cutover.md`
- [x] Migration RFC — `docs/rfcs/todo/0001-migrate-to-dotnet-core.md`

## 2. Backend — .NET 10 API

- [x] Solution scaffolded as Clean Architecture
  (`SharedKernel`/`Core`/`Application`/`Infrastructure`/`Web`)
- [x] Domain entities for albums, images, categories
- [x] EF Core PostgreSQL persistence with Fluent configurations
  (Guid PKs use `ValueGeneratedNever()`)
- [x] MediatR command/query handlers returning `Result<T>`
- [x] Public album/category endpoints
  (`src/Photography.Web/Controllers/PublicController.cs`)
- [x] Sitemap + robots endpoints (API + root) — `SeoDocuments`
- [x] Legacy 301 redirects for `/Portfolio/Details/{guid}` (case + trailing
  slash variants) — `LegacyRedirects` + `Program.cs`
- [x] JWT auth + admin controllers for albums, images, categories,
  inquiries, site settings
- [x] Inquiry handling with email notification
  (`IEmailSender` NoOp/SMTP switch)
- [~] Content pages (About) management — entity + admin endpoints landed;
  public `GET /api/pages/about` shape pending review against spec
- [ ] FluentValidation pass over remaining DTOs
- [ ] Structured-data (JSON-LD) endpoint helpers for
  Photographer / BreadcrumbList / ImageObject

## 3. Database & data migration

- [x] EF Core migrations targeting PostgreSQL
- [x] One-shot legacy importer (`tools/Photography.Migrator`)
  with `migrate-db`, `migrate-images`, `verify` commands
- [x] Album `LegacyGuid` preserved for redirect lookup
- [x] Slug generation + uniqueness on `Album`
- [ ] Spot-check production import dry-run output (operator task)

## 4. Image pipeline

- [x] R2 storage provider with local-fs fallback (provider-switch DI)
- [x] Width/height stored on `Image` (prevents CLS)
- [x] Responsive variant URLs (`Placeholder`/`Thumbnail`/`Card`/`Hero`/`Full`)
  exposed in `ImageDto`
- [ ] Server-side WebP/AVIF derivative generation in import pipeline
- [ ] LQIP (blur) placeholders persisted alongside originals

## 5. Frontend — React 19 / Vite / Redux Toolkit

- [x] FSD layout (`app/`, `pages/`, `features/`, `shared/`) with
  Container/Presenter pattern (presenters are props-only)
- [x] Public pages: Home, Portfolio, AlbumDetail, About, Contact, Stories,
  NotFound
- [x] Category filtering on Portfolio
- [x] Auth tokens via `tokenStore` + axios bearer/refresh wiring
- [x] Admin dashboard skeleton (login + albums/images CRUD)
- [ ] Premium editorial visual pass (typography, palette, spacing) per
  problem-statement design direction
- [ ] Album-detail storytelling layout + mid-gallery CTA + related albums
- [ ] Contact page direct-action buttons (Call/WhatsApp/Email/Instagram)
- [ ] SSR or static prerender for public marketing pages
  (currently CSR — flagged as SEO risk)
- [ ] Per-page meta/OG tags + canonical URLs from API site-settings

## 6. SEO

- [x] Clean slug URLs (`/portfolio/{slug}`)
- [x] Sitemap + robots at root and under `/api/public`
- [x] 301 redirects for legacy album URLs
- [ ] Open Graph image per album + global fallback
- [ ] JSON-LD structured data on public pages
- [ ] Multilingual-ready route shape (RO default, EN/RU optional)

## 7. Security

- [x] JWT-based admin auth + refresh
- [x] CORS policy in `Program.cs`
- [x] Rate limiter registered on the API pipeline
- [ ] Honeypot / spam protection on inquiry form
- [ ] Confirm secure-headers middleware (HSTS, X-Content-Type-Options, CSP)

## 8. Testing

- [x] Backend unit/integration tests
  (`tests/Photography.Application.Tests`, `tests/Photography.Infrastructure.Tests`)
- [x] Legacy redirect mapping unit tests (`LegacyRedirectsTests`)
- [x] Frontend Vitest slice/boundary tests (`client/src/**/*.test.ts`)
- [x] Cypress public-gallery E2E spec (`client/cypress/e2e/public-gallery.cy.ts`)
- [ ] Cypress admin login + create/edit album spec
- [ ] Cypress contact-form submission spec
- [ ] CI job that exercises the legacy redirect endpoint end-to-end

## 9. Operations & docs

- [x] `docs/operations/migration-runbook.md`
- [x] `docs/operations/r2-credentials.md`
- [x] `docs/operations/email.md`
- [x] `docs/operations/post-migration-verification.md`
- [ ] Production deploy guide (compose / hosting target)
- [ ] Admin-user creation runbook

---

## Recently landed

- Legacy 301 redirects + root `sitemap.xml` / `robots.txt`
  (`src/Photography.Web/Program.cs`, `SeoDocuments.cs`,
  `Photography.Application/Common/Routing/LegacyRedirects.cs`)
- Cypress album-detail intercept now derives slug from fixture
  (`client/cypress/e2e/public-gallery.cy.ts`)
- This progress tracker (`docs/migration-progress.md`)

## Validation commands

```bash
# Backend
dotnet build Photography.slnx
dotnet test  Photography.slnx --no-build

# Frontend
cd client && npm ci
npm run build
npx vitest run
# Cypress requires cypress binary; sandboxed envs may need
# CYPRESS_INSTALL_BINARY=0 npm ci, then run on dev machine
```
