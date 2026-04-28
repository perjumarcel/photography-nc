# Image / R2 storage model

## Storage abstraction

`Photography.Application.Storage.IStorageService` is the only seam the Application layer uses:

- `UploadAsync(key, stream, contentType, ct)` → returns canonical key
- `DeleteAsync(key, ct)` (idempotent)
- `OpenReadAsync(key, ct)` → caller-owned stream
- `ExistsAsync(key, ct)`
- `GetPublicUrl(key)` — for public portfolio images served via CDN
- `GetSignedUrlAsync(key, ttl, ct)` — short-lived URL for private albums

Two implementations:

| Implementation                  | Use case                                     |
|---------------------------------|----------------------------------------------|
| `LocalFileSystemStorageService` | Development; mirrors R2 layout under `_storage/` |
| `R2StorageService`              | Production; uses AWS S3 SDK against the R2 endpoint |

Selected via `Storage:Provider` (`Local` or `R2`) in `appsettings.json`.

## Key layout

Deterministic — the migrator and the live API both compute keys via `Photography.Application.Storage.StorageKeys.ImageKey`:

```
albums/{albumId-guid}/images/{imageId-guid}{ext}
```

Cover images may optionally use a parallel prefix `albums/{albumId}/covers/{imageId}{ext}` if cover separation is desired in a future iteration. The current implementation marks covers via the `image_type` column instead, keeping a single object per image.

Determinism guarantees:

1. The migrator can compute the new key from legacy `(albumId, imageId, originalName)` tuples without an intermediate lookup.
2. Re-runs of the migrator skip uploads when `IStorageService.ExistsAsync(key)` returns `true`.

## Public vs. private

- **Public portfolio images**: served via the configured `Storage:R2:PublicBaseUrl` (a CDN host backed by the R2 bucket). The API returns ready-to-use URLs in `ImageDto.PublicUrl`.
- **Private albums** (future): omitted from the public CDN. The API returns short-lived signed URLs from `GetSignedUrlAsync`. Signed URL TTLs default to 5–15 minutes and are never logged at `Information` level.

## Secrets

R2 credentials live exclusively in environment / configuration:

```
Storage__Provider=R2
Storage__R2__ServiceUrl=https://<account-id>.r2.cloudflarestorage.com
Storage__R2__Bucket=photography-prod
Storage__R2__AccessKey=...
Storage__R2__SecretKey=...
Storage__R2__PublicBaseUrl=https://images.example.com
```

Never committed. Never logged. Rotated via the runbook in `docs/operations/r2-credentials.md`.
