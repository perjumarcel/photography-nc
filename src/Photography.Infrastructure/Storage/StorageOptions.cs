namespace Photography.Infrastructure.Storage;

/// <summary>
/// Cloudflare R2 storage options. R2 is S3-compatible — set <see cref="ServiceUrl"/>
/// to your R2 endpoint, e.g. <c>https://&lt;account-id&gt;.r2.cloudflarestorage.com</c>.
/// </summary>
public sealed class R2StorageOptions
{
    public const string SectionName = "Storage:R2";

    public string ServiceUrl { get; set; } = string.Empty;
    public string Bucket { get; set; } = string.Empty;
    public string AccessKey { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;

    /// <summary>Public CDN base, e.g. <c>https://images.example.com</c>. Used to build public URLs.</summary>
    public string? PublicBaseUrl { get; set; }

    /// <summary>R2 region. Cloudflare requires <c>auto</c>.</summary>
    public string Region { get; set; } = "auto";
}

public sealed class LocalStorageOptions
{
    public const string SectionName = "Storage:Local";
    public string RootPath { get; set; } = "./_storage";
    public string PublicBaseUrl { get; set; } = "/local-storage";
}
