using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Photography.Application.Storage;

namespace Photography.Infrastructure.Storage;

/// <summary>
/// Local-filesystem storage for development. Mirrors the planned R2 key layout under <see cref="LocalStorageOptions.RootPath"/>.
/// </summary>
public sealed class LocalFileSystemStorageService : IStorageService
{
    private readonly LocalStorageOptions _opts;
    private readonly ILogger<LocalFileSystemStorageService> _logger;

    public LocalFileSystemStorageService(IOptions<LocalStorageOptions> opts, ILogger<LocalFileSystemStorageService> logger)
    {
        _opts = opts.Value;
        _logger = logger;
        Directory.CreateDirectory(_opts.RootPath);
    }

    public async Task<string> UploadAsync(string storageKey, Stream content, string contentType, CancellationToken ct = default)
    {
        var path = ResolvePath(storageKey);
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        await using var fs = File.Create(path);
        await content.CopyToAsync(fs, ct);
        _logger.LogDebug("Uploaded {Key} ({ContentType})", storageKey, contentType);
        return storageKey;
    }

    public Task DeleteAsync(string storageKey, CancellationToken ct = default)
    {
        var path = ResolvePath(storageKey);
        if (File.Exists(path)) File.Delete(path);
        return Task.CompletedTask;
    }

    public Task<Stream> OpenReadAsync(string storageKey, CancellationToken ct = default)
    {
        var path = ResolvePath(storageKey);
        Stream s = File.OpenRead(path);
        return Task.FromResult(s);
    }

    public Task<bool> ExistsAsync(string storageKey, CancellationToken ct = default) =>
        Task.FromResult(File.Exists(ResolvePath(storageKey)));

    public string GetPublicUrl(string storageKey) =>
        $"{_opts.PublicBaseUrl.TrimEnd('/')}/{storageKey.TrimStart('/')}";

    public Task<string> GetSignedUrlAsync(string storageKey, TimeSpan ttl, CancellationToken ct = default) =>
        Task.FromResult(GetPublicUrl(storageKey));

    private string ResolvePath(string storageKey)
    {
        var safe = storageKey.Replace("..", string.Empty).TrimStart('/');
        return Path.GetFullPath(Path.Combine(_opts.RootPath, safe));
    }
}
