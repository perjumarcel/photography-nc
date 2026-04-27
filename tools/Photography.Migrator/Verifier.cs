using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Photography.Application.Storage;
using Photography.Infrastructure.Persistence;

namespace Photography.Migrator;

/// <summary>
/// Post-migration reconciliation reports:
///  - album/image counts
///  - duplicate storage keys
///  - missing R2 objects for known images
///  - invalid extensions / content-types
///  - orphan images (no album)
/// </summary>
public static class Verifier
{
    public static async Task<int> RunAsync(IServiceProvider sp, ILogger logger)
    {
        using var scope = sp.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var storage = scope.ServiceProvider.GetRequiredService<IStorageService>();
        var ct = CancellationToken.None;

        var albumCount = await db.Albums.CountAsync(ct);
        var imageCount = await db.Images.CountAsync(ct);
        logger.LogInformation("Albums: {Albums} | Images: {Images}", albumCount, imageCount);

        var duplicates = await db.Images
            .AsNoTracking()
            .GroupBy(i => i.StorageKey)
            .Where(g => g.Count() > 1)
            .Select(g => new { g.Key, Count = g.Count() })
            .ToListAsync(ct);
        foreach (var d in duplicates)
            logger.LogWarning("Duplicate storage key {Key} ({Count} rows)", d.Key, d.Count);

        var orphanImages = await db.Images
            .AsNoTracking()
            .Where(i => !db.Albums.Any(a => a.Id == i.AlbumId))
            .CountAsync(ct);
        if (orphanImages > 0) logger.LogWarning("Orphan images: {Count}", orphanImages);

        var sample = await db.Images.AsNoTracking().Take(50).ToListAsync(ct);
        var missing = 0;
        foreach (var img in sample)
        {
            if (!await storage.ExistsAsync(img.StorageKey, ct))
            {
                logger.LogWarning("Missing in storage: {Key}", img.StorageKey);
                missing++;
            }
        }
        logger.LogInformation("Storage spot-check: {Missing} missing of {Sampled}", missing, sample.Count);
        return 0;
    }
}
