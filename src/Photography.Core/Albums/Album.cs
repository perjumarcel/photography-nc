using Photography.SharedKernel;

namespace Photography.Core.Albums;

/// <summary>
/// Album aggregate root. Owns its <see cref="Image"/> collection.
/// References <c>CategoryId</c> by ID only — no navigation property across aggregate boundaries.
/// </summary>
public class Album : AggregateRoot<Guid>
{
    public const int MaxTitleLength = 64;
    public const int MaxClientLength = 128;
    public const int MaxLocationLength = 128;

    public string Title { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public DateTime? EventDate { get; private set; }
    public string? Client { get; private set; }
    public string? Location { get; private set; }

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
        string? description = null,
        DateTime? eventDate = null,
        string? client = null,
        string? location = null,
        bool showInPortfolio = false,
        bool showInStories = false,
        bool showInHome = false)
    {
        ValidateTitle(title);
        ValidateClient(client);
        ValidateLocation(location);

        return new Album
        {
            Id = id == Guid.Empty ? Guid.NewGuid() : id,
            Title = title.Trim(),
            CategoryId = categoryId,
            Description = description,
            EventDate = eventDate,
            Client = client,
            Location = location,
            ShowInPortfolio = showInPortfolio,
            ShowInStories = showInStories,
            ShowInHome = showInHome,
            CreatedAtUtc = DateTime.UtcNow,
        };
    }

    public void UpdateDetails(
        string title,
        int categoryId,
        string? description,
        DateTime? eventDate,
        string? client,
        string? location)
    {
        ValidateTitle(title);
        ValidateClient(client);
        ValidateLocation(location);

        Title = title.Trim();
        CategoryId = categoryId;
        Description = description;
        EventDate = eventDate;
        Client = client;
        Location = location;
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
}
