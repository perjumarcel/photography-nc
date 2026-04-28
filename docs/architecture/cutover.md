# Production cutover and rollback

## Pre-cutover (T-7 days)

1. Take a fresh dump of the legacy SQL Server DB and copy of the `Images/Albums/Images` folder to a staging machine.
2. Provision the production PostgreSQL instance and the R2 bucket.
3. Provision DNS records for the new API + client (kept un-pointed until cutover).
4. Run the full migration in **staging** with `--dry-run` first, then for real:
   - `dotnet run --project tools/Photography.Migrator -- migrate-db --dry-run`
   - `dotnet run --project tools/Photography.Migrator -- migrate-db`
   - `dotnet run --project tools/Photography.Migrator -- migrate-images --dry-run`
   - `dotnet run --project tools/Photography.Migrator -- migrate-images`
   - `dotnet run --project tools/Photography.Migrator -- verify`
5. Smoke-test the React client against the staging API.

## Cutover (T-0)

1. **Freeze writes** on the legacy app (read-only banner + admin lockout).
2. Re-export the legacy DB and the image folder.
3. Re-run the migrator against production targets (idempotent — only deltas are copied).
4. Run `verify`. Block on any non-zero `missing` / duplicate / orphan reports.
5. Flip DNS to the new API + client.
6. Watch logs and `/health` for 1 hour.

## Rollback

Both the legacy DB and the legacy image folder are **untouched** by the migration (we only read from them). To roll back:

1. Flip DNS back to the legacy app.
2. Take note of any writes that happened against the new system during the cutover window — they will need to be reconciled manually if cutover succeeded longer than a few minutes before rollback.

Decommission of the legacy stack happens only after at least 14 days of stable operation on the new stack.
