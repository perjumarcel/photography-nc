# Photography

Photography portfolio platform — modernized stack.

## Stack

| Layer       | Technology                                                     |
|-------------|----------------------------------------------------------------|
| Backend     | ASP.NET Core API on **.NET 10** + MediatR + EF Core 9 + Result&lt;T&gt; |
| Database    | **PostgreSQL** (via Npgsql)                                    |
| Storage     | Cloudflare **R2** (S3-compatible) with local-fs fallback       |
| Frontend    | React 19 + TypeScript (strict) + Vite + Redux Toolkit + Tailwind CSS 4 + shadcn/Radix |
| i18n        | i18next + react-i18next (RO default, EN supported)             |
| Tests       | xUnit + Moq (backend) · Vitest + Cypress (frontend)            |

## Repository layout

```
src/
  Photography.SharedKernel/   Result<T>, EntityBase, AggregateRoot
  Photography.Core/           Domain entities + repository interfaces
  Photography.Application/    MediatR commands/queries, DTOs, IStorageService
  Photography.Infrastructure/ EF Core (PostgreSQL), repositories, R2/Local storage
  Photography.Web/            ASP.NET Core API (controllers, JWT, Swagger, CORS, etc.)
client/                       React 19 + Vite + Redux Toolkit + Tailwind 4
tools/Photography.Migrator/   One-shot legacy → PostgreSQL/R2 migrator
tests/                        xUnit projects
legacy/                       Legacy ABP / .NET Framework 4.6.1 code (reference)
docs/                         Architecture, RFCs, runbooks
```

## Quick start

### Backend
```bash
dotnet build Photography.slnx
dotnet test Photography.slnx
dotnet run --project src/Photography.Web
```

PostgreSQL is configured under `ConnectionStrings:Default` in `src/Photography.Web/appsettings.json`.

### Frontend
```bash
cd client
npm install
npm run dev      # http://localhost:5173
npm run build
npx vitest run
```

### Migration utility
```bash
cd tools/Photography.Migrator
dotnet run -- migrate-db        --dry-run
dotnet run -- migrate-images    --dry-run
dotnet run -- verify
```

See [`docs/operations/migration-runbook.md`](docs/operations/migration-runbook.md).

## Documentation

- [`docs/architecture/solution.md`](docs/architecture/solution.md) — clean-architecture project layout
- [`docs/architecture/postgres-schema.md`](docs/architecture/postgres-schema.md) — target DB schema
- [`docs/architecture/storage-r2.md`](docs/architecture/storage-r2.md) — image/R2 storage model
- [`docs/architecture/cutover.md`](docs/architecture/cutover.md) — production cutover & rollback
- [`docs/rfcs/todo/0001-migrate-to-dotnet-core.md`](docs/rfcs/todo/0001-migrate-to-dotnet-core.md) — migration RFC
- [`docs/operations/migration-runbook.md`](docs/operations/migration-runbook.md) — DB + image migration runbook
- [`docs/operations/r2-credentials.md`](docs/operations/r2-credentials.md) — R2 credential setup
- [`docs/operations/email.md`](docs/operations/email.md) — `IEmailSender` configuration (NoOp / SMTP)
- [`docs/migration-progress.md`](docs/migration-progress.md) — living checklist of migration progress
