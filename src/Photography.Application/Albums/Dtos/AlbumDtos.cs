using Photography.Core.Albums;

namespace Photography.Application.Albums.Dtos;

public sealed record AlbumDto(
    Guid Id,
    string Title,
    string? Description,
    DateTime? EventDate,
    string? Client,
    string? Location,
    bool ShowInPortfolio,
    bool ShowInStories,
    bool ShowInHome,
    int CategoryId,
    int ImageCount,
    Guid? CoverImageId);

public sealed record AlbumDetailsDto(
    Guid Id,
    string Title,
    string? Description,
    DateTime? EventDate,
    string? Client,
    string? Location,
    bool ShowInPortfolio,
    bool ShowInStories,
    bool ShowInHome,
    int CategoryId,
    IReadOnlyList<ImageDto> Images);

public sealed record ImageDto(
    Guid Id,
    Guid AlbumId,
    string OriginalName,
    string StorageKey,
    string PublicUrl,
    int Width,
    int Height,
    ImageOrientation Orientation,
    ImageType ImageType,
    long SizeBytes);

public sealed record CreateAlbumDto(
    string Title,
    int CategoryId,
    string? Description,
    DateTime? EventDate,
    string? Client,
    string? Location,
    bool ShowInPortfolio = false,
    bool ShowInStories = false,
    bool ShowInHome = false);

public sealed record UpdateAlbumDto(
    string Title,
    int CategoryId,
    string? Description,
    DateTime? EventDate,
    string? Client,
    string? Location,
    bool ShowInPortfolio,
    bool ShowInStories,
    bool ShowInHome);
