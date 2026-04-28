using Photography.SharedKernel;

namespace Photography.Core.Albums;

/// <summary>
/// Image entity. Belongs to an <see cref="Album"/>.
/// Binary content is stored externally (R2 / local-fs in development) and referenced via <see cref="StorageKey"/>.
/// </summary>
public class Image : EntityBase<Guid>
{
    public const int MaxOriginalNameLength = 256;
    public const int MaxStorageKeyLength = 512;
    public const int MaxContentTypeLength = 128;
    public const int MaxChecksumLength = 128;

    public Guid AlbumId { get; private set; }
    public string OriginalName { get; private set; } = string.Empty;
    public string StorageKey { get; private set; } = string.Empty;
    public string ContentType { get; private set; } = string.Empty;
    public long SizeBytes { get; private set; }
    public int Width { get; private set; }
    public int Height { get; private set; }
    public ImageOrientation Orientation { get; private set; }
    public ImageType ImageType { get; private set; }
    public string? Checksum { get; private set; }

    private Image() { }

    internal static Image Create(
        Guid id,
        Guid albumId,
        string originalName,
        string storageKey,
        string contentType,
        long sizeBytes,
        int width,
        int height,
        ImageOrientation orientation,
        string? checksum,
        ImageType imageType)
    {
        if (string.IsNullOrWhiteSpace(originalName))
            throw new ArgumentException("Original name required", nameof(originalName));
        if (originalName.Length > MaxOriginalNameLength)
            throw new ArgumentException($"Original name exceeds {MaxOriginalNameLength}", nameof(originalName));
        if (string.IsNullOrWhiteSpace(storageKey))
            throw new ArgumentException("Storage key required", nameof(storageKey));
        if (storageKey.Length > MaxStorageKeyLength)
            throw new ArgumentException($"Storage key exceeds {MaxStorageKeyLength}", nameof(storageKey));
        if (string.IsNullOrWhiteSpace(contentType))
            throw new ArgumentException("Content type required", nameof(contentType));
        if (sizeBytes < 0)
            throw new ArgumentException("Size must be non-negative", nameof(sizeBytes));
        if (width < 0 || height < 0)
            throw new ArgumentException("Width and height must be non-negative");

        return new Image
        {
            Id = id == Guid.Empty ? Guid.NewGuid() : id,
            AlbumId = albumId,
            OriginalName = originalName,
            StorageKey = storageKey,
            ContentType = contentType,
            SizeBytes = sizeBytes,
            Width = width,
            Height = height,
            Orientation = orientation,
            ImageType = imageType,
            Checksum = checksum,
            CreatedAtUtc = DateTime.UtcNow,
        };
    }

    internal void MarkAsCover()
    {
        ImageType = ImageType.Cover;
        Touch();
    }

    internal void MarkAsDefault()
    {
        ImageType = ImageType.Default;
        Touch();
    }
}
