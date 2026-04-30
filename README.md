# Photography

Photography portfolio platform — modernized stack.

> **Status**: migration in progress. The new clean-architecture solution lives under `src/`, `client/`, `tools/`, `tests/`, and `docs/`. The legacy ABP / .NET Framework 4.6.1 project lives under [`legacy/`](legacy/) and is kept as reference until feature parity is reached.

## Stack

| Layer       | Technology                                                            |
|-------------|-----------------------------------------------------------------------|
| Backend     | ASP.NET Core API on **.NET 10** + MediatR + EF Core 9 + `Result<T>`   |
| Database    | **PostgreSQL** via Npgsql                                             |
| Storage     | Cloudflare **R2** (S3-compatible) with local-fs fallback              |
| Frontend    | React 19 + TypeScript (strict) + Vite + Redux Toolkit + Tailwind CSS 4 + shadcn/Radix |
| i18n        | i18next + react-i18next (RO default, EN supported)                    |
| Tests       | xUnit + Moq (backend) · Vitest + Cypress (frontend)                   |

## Quick start

### Backend
```bash
dotnet build Photography.slnx
dotnet test  Photography.slnx
dotnet run --project src/Photography.Web
```

### Frontend
```bash
cd client
npm install
npm run dev      # http://localhost:5173
npm run build
npx vitest run
npm run e2e      # start Vite first; Cypress runs against http://127.0.0.1:5173
```

### Migration
```bash
cd tools/Photography.Migrator
dotnet run -- migrate-db     --dry-run
dotnet run -- migrate-images --dry-run
dotnet run -- verify
```

### Docker (local dev)
```bash
docker compose up -d postgres   # Postgres only — run the API on the host with hot reload
docker compose up api           # Or build & run the containerised API
```
The Vite dev server is intentionally not containerised so frontend HMR runs at full speed; it proxies `/api/*` to the API on `:5080` via `client/vite.config.ts`.

### CI
GitHub Actions ([`.github/workflows/ci.yml`](.github/workflows/ci.yml)) builds + tests both the backend and the frontend on every push and pull request.

## Documentation

See [`docs/README.md`](docs/README.md) for the full index — RFC, architecture, runbooks, post-migration verification.

## Repository layout

```
src/
  Photography.SharedKernel/   Result<T>, EntityBase, AggregateRoot
  Photography.Core/           Domain entities + repository interfaces
  Photography.Application/    MediatR commands/queries, DTOs, IStorageService
  Photography.Infrastructure/ EF Core (PostgreSQL), repositories, R2/Local storage
  Photography.Web/            ASP.NET Core API
client/                       React 19 + Vite + Redux Toolkit + Tailwind 4
tools/Photography.Migrator/   One-shot legacy → PostgreSQL/R2 migrator
tests/                        xUnit projects
legacy/                       Legacy ABP / .NET Framework 4.6.1 code (reference)
docs/                         Architecture, RFCs, runbooks
```
