using System.Security.Cryptography;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Photography.Application.Storage;

namespace Photography.Migrator;

/// <summary>
/// Walks the legacy on-disk folder layout
/// <c>{root}/{albumId}/{imageId}.{ext}</c>
/// and uploads each file to the configured storage backend at the deterministic key
/// produced by <see cref="StorageKeys.ImageKey(Guid, Guid, string)"/>.
///
/// Idempotent: skips objects that already exist with the same checksum.
/// Produces a JSON manifest at <c>migration-manifest.json</c> that doubles as the reconciliation report.
/// </summary>
public static class ImageMigrator
{
    public static async Task<int> RunAsync(IServiceProvider sp, IConfiguration cfg, bool dryRun, ILogger logger)
    {
        var root = cfg["Migration:LegacyImagesRoot"];
        if (string.IsNullOrWhiteSpace(root) || !Directory.Exists(root))
        {
            logger.LogError("Missing or invalid Migration:LegacyImagesRoot: {Root}", root);
            return 2;
        }

        var storage = sp.GetRequiredService<IStorageService>();
        var manifest = new List<MigrationEntry>();
        var ct = CancellationToken.None;

        foreach (var albumDir in Directory.EnumerateDirectories(root))
        {
            if (!Guid.TryParse(Path.GetFileName(albumDir), out var albumId))
            {
                logger.LogWarning("Skipping non-GUID directory {Dir}", albumDir);
                continue;
            }

            foreach (var file in Directory.EnumerateFiles(albumDir))
            {
                var name = Path.GetFileNameWithoutExtension(file);
                if (!Guid.TryParse(name, out var imageId))
                {
                    logger.LogWarning("Skipping non-GUID file {File}", file);
                    continue;
                }

                var entry = new MigrationEntry
                {
                    AlbumId = albumId,
                    ImageId = imageId,
                    OriginalPath = file,
                    StorageKey = StorageKeys.ImageKey(albumId, imageId, file),
                    SizeBytes = new FileInfo(file).Length,
                    Checksum = await ComputeSha256Async(file, ct),
                    Status = "pending",
                };

                if (await storage.ExistsAsync(entry.StorageKey, ct))
                {
                    entry.Status = "skipped-exists";
                }
                else if (dryRun)
                {
                    entry.Status = "dry-run";
                }
                else
                {
                    try
                    {
                        await using var fs = File.OpenRead(file);
                        var contentType = GuessContentType(file);
                        await storage.UploadAsync(entry.StorageKey, fs, contentType, ct);
                        entry.Status = "uploaded";
                    }
                    catch (Exception ex)
                    {
                        entry.Status = $"failed: {ex.GetType().Name}";
                        logger.LogError(ex, "Upload failed for {Key}", entry.StorageKey);
                    }
                }
                manifest.Add(entry);
            }
        }

        var manifestPath = Path.Combine(Directory.GetCurrentDirectory(), "migration-manifest.json");
        await File.WriteAllTextAsync(manifestPath,
            JsonSerializer.Serialize(manifest, new JsonSerializerOptions { WriteIndented = true }), ct);
        logger.LogInformation("Image migration complete. {Count} entries. Manifest: {Manifest}", manifest.Count, manifestPath);
        return 0;
    }

    private static async Task<string> ComputeSha256Async(string path, CancellationToken ct)
    {
        await using var fs = File.OpenRead(path);
        var hash = await SHA256.HashDataAsync(fs, ct);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    private static string GuessContentType(string path) => Path.GetExtension(path).ToLowerInvariant() switch
    {
        ".jpg" or ".jpeg" => "image/jpeg",
        ".png" => "image/png",
        ".webp" => "image/webp",
        ".gif" => "image/gif",
        _ => "application/octet-stream",
    };
}

public sealed class MigrationEntry
{
    public Guid AlbumId { get; set; }
    public Guid ImageId { get; set; }
    public string OriginalPath { get; set; } = string.Empty;
    public string StorageKey { get; set; } = string.Empty;
    public long SizeBytes { get; set; }
    public string Checksum { get; set; } = string.Empty;
    public string Status { get; set; } = "pending";
}
