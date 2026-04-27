# R2 credential setup

1. Sign in to the Cloudflare dashboard → **R2** → **Manage R2 API Tokens**.
2. Create an API token with **Object Read & Write** permission scoped to the project bucket.
3. Capture the **Access Key ID**, **Secret Access Key**, and the **endpoint URL** (`https://<account-id>.r2.cloudflarestorage.com`).
4. Store them in your secret manager (Azure Key Vault, AWS Secrets Manager, GitHub Actions secrets, …). Never commit them to source control.

## Environment variables consumed by the API and the migrator

```
PHOTOGRAPHY_Storage__Provider=R2
PHOTOGRAPHY_Storage__R2__ServiceUrl=https://<account-id>.r2.cloudflarestorage.com
PHOTOGRAPHY_Storage__R2__Bucket=photography-prod
PHOTOGRAPHY_Storage__R2__AccessKey=<from secret manager>
PHOTOGRAPHY_Storage__R2__SecretKey=<from secret manager>
PHOTOGRAPHY_Storage__R2__PublicBaseUrl=https://images.example.com
PHOTOGRAPHY_Storage__R2__Region=auto
```

## CDN fronting

For public portfolio images, set up a Cloudflare custom domain in front of the bucket and use it as `PublicBaseUrl`. The bucket itself stays private — only the CDN can reach the underlying objects (configure via R2 bucket policy).

## Rotation

1. Create a new API token in the Cloudflare dashboard.
2. Update the secret in your secret manager.
3. Restart the API and re-run any in-flight migrator job.
4. Revoke the old token after at least 24 hours of clean operation.

## Logging hygiene

The R2 storage implementation never logs:
- access keys / secret keys
- full storage keys at `Information` level (only the first 32 chars, at `Debug`)
- signed URLs

This is enforced by `R2StorageService` — see `src/Photography.Infrastructure/Storage/R2StorageService.cs`.
