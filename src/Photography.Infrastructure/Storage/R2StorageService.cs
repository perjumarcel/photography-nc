using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Photography.Application.Storage;

namespace Photography.Infrastructure.Storage;

/// <summary>
/// Cloudflare R2 storage backed by the AWS S3 SDK (R2 is S3-compatible).
/// Credentials and endpoint come from <see cref="R2StorageOptions"/> — never hardcoded.
/// </summary>
public sealed class R2StorageService : IStorageService, IDisposable
{
    private readonly IAmazonS3 _s3;
    private readonly R2StorageOptions _opts;
    private readonly ILogger<R2StorageService> _logger;

    public R2StorageService(IOptions<R2StorageOptions> opts, ILogger<R2StorageService> logger)
    {
        _opts = opts.Value;
        _logger = logger;

        var config = new AmazonS3Config
        {
            ServiceURL = _opts.ServiceUrl,
            ForcePathStyle = true,
            AuthenticationRegion = _opts.Region,
        };
        _s3 = new AmazonS3Client(_opts.AccessKey, _opts.SecretKey, config);
    }

    public async Task<string> UploadAsync(string storageKey, Stream content, string contentType, CancellationToken ct = default)
    {
        var req = new PutObjectRequest
        {
            BucketName = _opts.Bucket,
            Key = storageKey,
            InputStream = content,
            ContentType = contentType,
            DisablePayloadSigning = true,
        };
        await _s3.PutObjectAsync(req, ct);
        _logger.LogDebug("Uploaded object key={KeyPrefix}…", storageKey.Length > 32 ? storageKey[..32] : storageKey);
        return storageKey;
    }

    public async Task DeleteAsync(string storageKey, CancellationToken ct = default)
    {
        try
        {
            await _s3.DeleteObjectAsync(_opts.Bucket, storageKey, ct);
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            // Idempotent.
        }
    }

    public async Task<Stream> OpenReadAsync(string storageKey, CancellationToken ct = default)
    {
        var resp = await _s3.GetObjectAsync(_opts.Bucket, storageKey, ct);
        return resp.ResponseStream;
    }

    public async Task<bool> ExistsAsync(string storageKey, CancellationToken ct = default)
    {
        try
        {
            await _s3.GetObjectMetadataAsync(_opts.Bucket, storageKey, ct);
            return true;
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return false;
        }
    }

    public string GetPublicUrl(string storageKey)
    {
        if (string.IsNullOrEmpty(_opts.PublicBaseUrl))
            return $"{_opts.ServiceUrl.TrimEnd('/')}/{_opts.Bucket}/{storageKey}";
        return $"{_opts.PublicBaseUrl.TrimEnd('/')}/{storageKey}";
    }

    public Task<string> GetSignedUrlAsync(string storageKey, TimeSpan ttl, CancellationToken ct = default)
    {
        var req = new GetPreSignedUrlRequest
        {
            BucketName = _opts.Bucket,
            Key = storageKey,
            Expires = DateTime.UtcNow.Add(ttl),
            Verb = HttpVerb.GET,
        };
        return Task.FromResult(_s3.GetPreSignedURL(req));
    }

    public void Dispose() => _s3.Dispose();
}
