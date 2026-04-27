# RFC 0001 — Migrate Photography from ABP/.NET Framework 4.6.1 to ASP.NET Core .NET 10

## Status
Proposed — implementation in progress (this PR scaffolds the new stack alongside the legacy projects).

## Context

The current production stack is **ASP.NET Boilerplate (ABP)** on **.NET Framework 4.6.1** with **EF6 + SQL Server** and a server-rendered MVC/WebApi front-end. Image binaries are stored under `Images/Albums/Images/{AlbumId}/{ImageId}.{extension}`.

The framework is end-of-life, slow to deploy, and tightly coupled to Windows hosting. Adding new features (responsive admin, public CDN, signed URLs, etc.) is increasingly painful.

## Goals

1. **Clean architecture** — `SharedKernel → Core → Application → Infrastructure → Web` layers with explicit dependencies.
2. **Modern runtime** — ASP.NET Core API on `net10.0`, MediatR, EF Core, `Result<T>`, cancellation tokens everywhere.
3. **PostgreSQL** as the system of record. EF Core migrations only — no `EnsureCreated`.
4. **Cloudflare R2** for image storage; local FS for development.
5. **React 19 + Vite + Redux Toolkit + Tailwind 4 + shadcn/Radix** front-end with i18next (RO default, EN supported).
6. **One-shot, idempotent migration tool** that copies legacy SQL Server data into PostgreSQL preserving GUID IDs, and uploads existing image folders into R2 with deterministic keys.
7. **Reconciliation reports** (counts, missing files, duplicates, orphans) before and after cutover.

## Non-goals

- Porting ABP-specific abstractions (modules, `IRepository`, dynamic API generation). Replaced by MediatR + explicit query/command repositories.
- Maintaining the legacy MVC views — replaced wholesale by the React client.
- Multi-tenancy. The current product is single-tenant; we do not carry forward ABP's tenant/edition machinery.

## Domain model

| Entity   | Notes                                                                                   |
|----------|-----------------------------------------------------------------------------------------|
| Album    | Aggregate root; owns `Image` collection; references `CategoryId` by ID only             |
| Image    | Belongs to album. Binary lives in storage. Holds metadata + deterministic `StorageKey`. |
| Category | Int identity (preserves legacy IDs)                                                     |
| Tag      | Int identity                                                                            |

GUID IDs are preserved across the migration so that R2 keys can be computed deterministically (`albums/{albumId}/images/{imageId}{ext}`).

## API surface

```
GET    /api/public/albums
GET    /api/public/albums/{id}
GET    /api/public/categories

GET    /api/admin/albums
POST   /api/admin/albums
PUT    /api/admin/albums/{id}
DELETE /api/admin/albums/{id}
POST   /api/admin/albums/{id}/images
DELETE /api/admin/albums/{albumId}/images/{imageId}
PATCH  /api/admin/albums/{albumId}/images/{imageId}/cover

GET    /api/admin/categories
POST   /api/admin/categories
PUT    /api/admin/categories/{id}
```

Controllers stay thin and forward to MediatR. Admin endpoints require the `AdminOnly` policy. Public endpoints expose DTOs only.

## Migration plan (high-level)

1. Scaffold the new solution and projects (this PR).
2. Implement the domain model and EF Core PostgreSQL schema with migrations.
3. Implement album/image/category APIs end-to-end.
4. Build the migrator console:
   - `migrate-db` — copies categories + albums + image metadata from legacy SQL Server.
   - `migrate-images` — walks the legacy folder, builds a manifest, uploads to R2 (idempotent, checksum-skipped).
   - `verify` — counts, duplicates, orphans, storage spot-check.
5. Scaffold the React client and the public gallery; then admin album/image management.
6. Auth (JWT + refresh), security headers, rate limiting, CORS, structured logging.
7. Dry-run cutover in a staging environment against a production-like dump.
8. Production cutover (see `docs/architecture/cutover.md`).

## Risks and mitigations

| Risk                                          | Mitigation                                                                  |
|-----------------------------------------------|-----------------------------------------------------------------------------|
| Legacy IDs collide / are not unique           | Verifier fails the run; cutover blocked until reconciliation passes.        |
| R2 upload partial failure                     | Manifest persists per-file status; re-running the tool resumes idempotently |
| Schema drift between staging and prod         | Migrations are checked into source; staging dump rehearsed before cutover.  |
| Frontend not at parity with legacy MVC views  | Feature parity tracked in the implementation plan; gated by stakeholder UX. |

## Rollback

Both DB and R2 migrations are write-mostly into new resources (new DB, new bucket). Rollback is to repoint DNS to the legacy app — the legacy DB and image folder remain untouched until decommission. See `docs/architecture/cutover.md`.
