# Post-migration verification

Run after every cutover (staging or production).

## 1. Database counts

```bash
dotnet run --project tools/Photography.Migrator -- verify
```

Expected output:
- `Albums: <N>` matches the legacy SQL Server count.
- `Images: <M>` matches the legacy count (modulo any files reported as missing on disk).
- No duplicate `storage_key` warnings.
- No orphan-image warnings.
- Storage spot-check reports `0 missing of 50`.

## 2. Functional smoke tests

| Endpoint                         | Expected                                       |
|----------------------------------|------------------------------------------------|
| `GET /health`                    | `200 OK`                                       |
| `GET /api/public/categories`     | Non-empty list                                 |
| `GET /api/public/albums`         | Non-empty list with cover URLs that load       |
| `GET /api/public/albums/{id}`    | Album details with all images visible          |
| Front-end portfolio page         | Loads albums, no 404s on cover images          |
| Admin login → upload → cover     | Upload returns 201, cover update returns 204   |

## 3. Storage spot-checks

Pick 10 random images from the manifest and verify in a browser that the public URL resolves with the expected MIME type and dimensions.

## 4. Logs

Check the API logs for:
- No 5xx responses during the smoke tests
- No warnings about slow handlers (>500ms) on cold start beyond the first request
- No CORS errors from the React client
