using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Photography.Core.Albums;
using Photography.Core.Categories;
using Photography.Infrastructure.Persistence;

namespace Photography.Migrator;

/// <summary>
/// Reads albums + images from the legacy SQL Server / EF6 schema and writes them into the
/// new PostgreSQL schema, preserving Guid IDs so that R2 keys remain deterministic.
///
/// IMPORTANT: this is a one-shot, idempotent migrator. Re-runs skip rows whose ID already exists.
/// </summary>
public static class DbMigrator
{
    public static async Task<int> RunAsync(IServiceProvider sp, IConfiguration cfg, bool dryRun, ILogger logger)
    {
        var legacyCs = cfg["Migration:LegacyConnectionString"];
        if (string.IsNullOrWhiteSpace(legacyCs))
        {
            logger.LogError("Missing Migration:LegacyConnectionString");
            return 2;
        }

        using var scope = sp.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var ct = CancellationToken.None;

        await using var legacy = new SqlConnection(legacyCs);
        await legacy.OpenAsync(ct);

        var categoryMap = await CopyCategoriesAsync(legacy, db, dryRun, logger, ct);
        await CopyAlbumsAsync(legacy, db, categoryMap, dryRun, logger, ct);

        if (!dryRun) await db.SaveChangesAsync(ct);
        logger.LogInformation("DB migration complete. dry-run={DryRun}", dryRun);
        return 0;
    }

    private static async Task<IDictionary<int, int>> CopyCategoriesAsync(
        SqlConnection legacy, AppDbContext db, bool dryRun, ILogger logger, CancellationToken ct)
    {
        var map = new Dictionary<int, int>();
        await using var cmd = legacy.CreateCommand();
        cmd.CommandText = "SELECT Id, Name FROM Categories";
        await using var rd = await cmd.ExecuteReaderAsync(ct);
        var added = 0;
        while (await rd.ReadAsync(ct))
        {
            var legacyId = rd.GetInt32(0);
            var name = rd.GetString(1);
            if (await db.Categories.AnyAsync(c => c.Name == name, ct))
            {
                var existing = await db.Categories.FirstAsync(c => c.Name == name, ct);
                map[legacyId] = existing.Id;
                continue;
            }
            var cat = Category.Create(name);
            if (!dryRun)
            {
                await db.Categories.AddAsync(cat, ct);
                await db.SaveChangesAsync(ct);
                map[legacyId] = cat.Id;
            }
            added++;
        }
        logger.LogInformation("Categories: copied {Count}", added);
        return map;
    }

    private static async Task CopyAlbumsAsync(
        SqlConnection legacy, AppDbContext db, IDictionary<int, int> categoryMap,
        bool dryRun, ILogger logger, CancellationToken ct)
    {
        await using var cmd = legacy.CreateCommand();
        cmd.CommandText = @"SELECT Id, Title, Description, EventDate, Client, Location,
                                   ShowInPortfolio, ShowInStories, ShowInHome, CategoryId, CreationTime
                            FROM Albums";
        await using var rd = await cmd.ExecuteReaderAsync(ct);

        var albumIds = new List<Guid>();
        var albums = new List<Album>();
        while (await rd.ReadAsync(ct))
        {
            var id = rd.GetGuid(0);
            albumIds.Add(id);
            if (await db.Albums.AnyAsync(a => a.Id == id, ct)) continue;

            var legacyCategoryId = rd.GetInt32(9);
            if (!categoryMap.TryGetValue(legacyCategoryId, out var newCategoryId))
            {
                logger.LogWarning("Album {AlbumId} skipped — unmapped legacy category {Cat}", id, legacyCategoryId);
                continue;
            }

            var album = Album.Create(
                id: id,
                title: rd.GetString(1),
                categoryId: newCategoryId,
                description: rd.IsDBNull(2) ? null : rd.GetString(2),
                eventDate: rd.IsDBNull(3) ? null : DateTime.SpecifyKind(rd.GetDateTime(3), DateTimeKind.Utc),
                client: rd.IsDBNull(4) ? null : rd.GetString(4),
                location: rd.IsDBNull(5) ? null : rd.GetString(5),
                showInPortfolio: rd.GetBoolean(6),
                showInStories: rd.GetBoolean(7),
                showInHome: rd.GetBoolean(8));
            albums.Add(album);
        }
        await rd.CloseAsync();

        if (!dryRun)
        {
            await db.Albums.AddRangeAsync(albums, ct);
            await db.SaveChangesAsync(ct);
        }
        logger.LogInformation("Albums: copied {Count} (legacy total {Total})", albums.Count, albumIds.Count);
    }
}
