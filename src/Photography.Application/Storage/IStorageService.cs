namespace Photography.Application.Storage;

/// <summary>
/// Storage abstraction for image binary content.
/// Implementations: Cloudflare R2 (production) and LocalFileSystem (development).
/// Never log signed URLs, secrets, or full keys at Information level — they are sensitive.
/// </summary>
public interface IStorageService
{
    Task<string> UploadAsync(string storageKey, Stream content, string contentType, CancellationToken ct = default);
    Task DeleteAsync(string storageKey, CancellationToken ct = default);
    Task<Stream> OpenReadAsync(string storageKey, CancellationToken ct = default);
    Task<bool> ExistsAsync(string storageKey, CancellationToken ct = default);
    string GetPublicUrl(string storageKey);
    Task<string> GetSignedUrlAsync(string storageKey, TimeSpan ttl, CancellationToken ct = default);
}
