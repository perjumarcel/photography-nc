# Migration runbook

## Prerequisites

- .NET 10 SDK installed.
- PostgreSQL 14+ provisioned, with a database and a writeable user.
- R2 bucket provisioned, with API token and access/secret key.
- Read-only access to the legacy SQL Server database.
- Read access to the legacy image folder (`Images/Albums/Images/{albumId}/{imageId}.{ext}`).

## Configuration

`appsettings.json` (or environment variables) for the migrator:

```json
{
  "ConnectionStrings": {
    "Default": "Host=...;Port=5432;Database=photography;Username=...;Password=..."
  },
  "Migration": {
    "LegacyConnectionString": "Server=...;Database=Photography;User Id=...;Password=...;Encrypt=False",
    "LegacyImagesRoot": "/var/www/photography/Images/Albums/Images"
  },
  "Storage": {
    "Provider": "R2",
    "R2": {
      "ServiceUrl": "https://<account-id>.r2.cloudflarestorage.com",
      "Bucket": "photography-prod",
      "AccessKey": "...",
      "SecretKey": "...",
      "PublicBaseUrl": "https://images.example.com",
      "Region": "auto"
    }
  }
}
```

> Never commit secrets. Use environment variables prefixed `PHOTOGRAPHY_` (e.g. `PHOTOGRAPHY_Storage__R2__SecretKey`).

## Steps

### 1. Apply the EF Core schema
```bash
dotnet ef database update \
  --project src/Photography.Infrastructure \
  --startup-project src/Photography.Web
```

### 2. Dry-run the DB migration
```bash
cd tools/Photography.Migrator
dotnet run -- migrate-db --dry-run
```
Inspect the log for warnings about unmapped categories or skipped albums.

### 3. Run the DB migration for real
```bash
dotnet run -- migrate-db
```

### 4. Dry-run the image migration
```bash
dotnet run -- migrate-images --dry-run
```
Inspect `migration-manifest.json` for the planned uploads, sizes, and checksums.

### 5. Run the image migration
```bash
dotnet run -- migrate-images
```
Re-runnable. Already-uploaded objects are skipped (verified via `ExistsAsync`).

### 6. Verify
```bash
dotnet run -- verify
```
This reports:
- Album count, image count
- Duplicate `storage_key` values
- Orphan images (no parent album)
- Storage spot-check for the first 50 images

Any non-zero finding **blocks the cutover**.

## Failure modes and recovery

| Symptom                            | Recovery                                                         |
|------------------------------------|------------------------------------------------------------------|
| Image upload throws `503` from R2  | Re-run `migrate-images` — idempotent, only retries failed entries |
| Manifest entry status is `failed`  | Inspect log, fix root cause, delete entry from manifest, re-run  |
| Verifier reports orphan images     | Investigate which legacy album was excluded; re-run `migrate-db` |
| Verifier reports duplicate keys    | Manual: deduplicate at source, re-run                            |

## Rollback

The migration reads from the legacy DB / folder; it never writes. To undo everything on the new side, drop the PostgreSQL DB and empty the R2 bucket (keep one bucket per environment so that a rollback is bounded).
