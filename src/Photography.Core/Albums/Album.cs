using Photography.SharedKernel;

namespace Photography.Core.Albums;

/// <summary>
/// Album aggregate root. Owns its <see cref="Image"/> collection.
/// References <c>CategoryId</c> by ID only — no navigation property across aggregate boundaries.
/// </summary>
public class Album : AggregateRoot<Guid>
{
    public const int MaxTitleLength = 64;
    public const int MaxSlugLength = 96;
    public const int MaxClientLength = 128;
    public const int MaxLocationLength = 128;
    public const int MaxSeoTitleLength = 128;
    public const int MaxSeoDescriptionLength = 320;
    public const int MaxCoverAltTextLength = 256;

    public string Title { get; private set; } = string.Empty;
    public string Slug { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public DateTime? EventDate { get; private set; }
    public string? Client { get; private set; }
    public string? Location { get; private set; }
    public string? SeoTitle { get; private set; }
    public string? SeoDescription { get; private set; }
    public string? CoverAltText { get; private set; }

    public bool ShowInPortfolio { get; private set; }
    public bool ShowInStories { get; private set; }
    public bool ShowInHome { get; private set; }

    public int CategoryId { get; private set; }

    private readonly List<Image> _images = new();
    public IReadOnlyCollection<Image> Images => _images.AsReadOnly();

    private Album() { }

    public static Album Create(
        Guid id,
        string title,
        int categoryId,
        string? slug = null,
        string? description = null,
        DateTime? eventDate = null,
        string? client = null,
        string? location = null,
        string? seoTitle = null,
        string? seoDescription = null,
        string? coverAltText = null,
        bool showInPortfolio = false,
        bool showInStories = false,
        bool showInHome = false)
    {
        ValidateTitle(title);
        var normalizedSlug = NormalizeSlug(slug ?? title);
        ValidateSlug(normalizedSlug);
        ValidateClient(client);
        ValidateLocation(location);
        ValidateSeo(seoTitle, seoDescription, coverAltText);

        return new Album
        {
            Id = id == Guid.Empty ? Guid.NewGuid() : id,
            Title = title.Trim(),
            Slug = normalizedSlug.Trim(),
            CategoryId = categoryId,
            Description = description,
            EventDate = eventDate,
            Client = client,
            Location = location,
            SeoTitle = NormalizeOptional(seoTitle),
            SeoDescription = NormalizeOptional(seoDescription),
            CoverAltText = NormalizeOptional(coverAltText),
            ShowInPortfolio = showInPortfolio,
            ShowInStories = showInStories,
            ShowInHome = showInHome,
            CreatedAtUtc = DateTime.UtcNow,
        };
    }

    public void UpdateDetails(
        string title,
        string slug,
        int categoryId,
        string? description,
        DateTime? eventDate,
        string? client,
        string? location,
        string? seoTitle,
        string? seoDescription,
        string? coverAltText)
    {
        ValidateTitle(title);
        var normalizedSlug = NormalizeSlug(slug);
        ValidateSlug(normalizedSlug);
        ValidateClient(client);
        ValidateLocation(location);
        ValidateSeo(seoTitle, seoDescription, coverAltText);

        Title = title.Trim();
        Slug = normalizedSlug;
        CategoryId = categoryId;
        Description = description;
        EventDate = eventDate;
        Client = client;
        Location = location;
        SeoTitle = NormalizeOptional(seoTitle);
        SeoDescription = NormalizeOptional(seoDescription);
        CoverAltText = NormalizeOptional(coverAltText);
        Touch();
    }

    public void SetVisibility(bool showInPortfolio, bool showInStories, bool showInHome)
    {
        ShowInPortfolio = showInPortfolio;
        ShowInStories = showInStories;
        ShowInHome = showInHome;
        Touch();
    }

    public Image AddImage(
        Guid imageId,
        string originalName,
        string storageKey,
        string contentType,
        long sizeBytes,
        int width,
        int height,
        string? checksum = null,
        ImageType imageType = ImageType.Default)
    {
        var orientation = width >= height ? ImageOrientation.Horizontal : ImageOrientation.Vertical;
        var image = Image.Create(
            id: imageId,
            albumId: Id,
            originalName: originalName,
            storageKey: storageKey,
            contentType: contentType,
            sizeBytes: sizeBytes,
            width: width,
            height: height,
            orientation: orientation,
            checksum: checksum,
            imageType: imageType);
        _images.Add(image);
        Touch();
        return image;
    }

    public void RemoveImage(Guid imageId)
    {
        var image = _images.FirstOrDefault(i => i.Id == imageId)
                    ?? throw new InvalidOperationException($"Image {imageId} not part of album {Id}");
        _images.Remove(image);
        Touch();
    }

    public void SetCover(Guid imageId)
    {
        var target = _images.FirstOrDefault(i => i.Id == imageId)
                     ?? throw new InvalidOperationException($"Image {imageId} not part of album {Id}");

        foreach (var img in _images)
        {
            if (img.ImageType == ImageType.Cover && img.Id != imageId)
                img.MarkAsDefault();
        }
        target.MarkAsCover();
        Touch();
    }

    private static void ValidateTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Album title is required", nameof(title));
        if (title.Length > MaxTitleLength)
            throw new ArgumentException($"Album title exceeds {MaxTitleLength} characters", nameof(title));
    }

    private static void ValidateSlug(string slug)
    {
        if (string.IsNullOrWhiteSpace(slug))
            throw new ArgumentException("Album slug is required", nameof(slug));
        if (slug.Length > MaxSlugLength)
            throw new ArgumentException($"Album slug exceeds {MaxSlugLength} characters", nameof(slug));
        if (slug.Any(ch => !char.IsLetterOrDigit(ch) && ch != '-'))
            throw new ArgumentException("Album slug may contain only letters, numbers, and hyphens", nameof(slug));
    }

    private static void ValidateClient(string? client)
    {
        if (client is { Length: > MaxClientLength })
            throw new ArgumentException($"Client exceeds {MaxClientLength} characters", nameof(client));
    }

    private static void ValidateLocation(string? location)
    {
        if (location is { Length: > MaxLocationLength })
            throw new ArgumentException($"Location exceeds {MaxLocationLength} characters", nameof(location));
    }

    private static void ValidateSeo(string? seoTitle, string? seoDescription, string? coverAltText)
    {
        if (seoTitle is { Length: > MaxSeoTitleLength })
            throw new ArgumentException($"SEO title exceeds {MaxSeoTitleLength} characters", nameof(seoTitle));
        if (seoDescription is { Length: > MaxSeoDescriptionLength })
            throw new ArgumentException($"SEO description exceeds {MaxSeoDescriptionLength} characters", nameof(seoDescription));
        if (coverAltText is { Length: > MaxCoverAltTextLength })
            throw new ArgumentException($"Cover alt text exceeds {MaxCoverAltTextLength} characters", nameof(coverAltText));
    }

    private static string? NormalizeOptional(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static string NormalizeSlug(string value)
    {
        var slug = new string(value.Trim().ToLowerInvariant()
            .Select(ch => char.IsLetterOrDigit(ch) ? ch : '-')
            .ToArray());
        while (slug.Contains("--", StringComparison.Ordinal))
            slug = slug.Replace("--", "-", StringComparison.Ordinal);
        slug = slug.Trim('-');
        if (string.IsNullOrWhiteSpace(slug)) slug = "album";
        return slug.Length <= MaxSlugLength ? slug : slug[..MaxSlugLength].Trim('-');
    }
}
