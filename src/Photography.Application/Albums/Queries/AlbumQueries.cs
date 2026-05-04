using MediatR;
using Photography.Application.Albums.Dtos;
using Photography.Application.Storage;
using Photography.Core.Albums;
using Photography.SharedKernel;

namespace Photography.Application.Albums.Queries;

public sealed record ListAlbumsQuery(bool PublicOnly) : IRequest<Result<IReadOnlyList<AlbumDto>>>;

public sealed class ListAlbumsHandler : IRequestHandler<ListAlbumsQuery, Result<IReadOnlyList<AlbumDto>>>
{
    private readonly IAlbumQueryRepository _repo;
    private readonly IStorageService _storage;

    public ListAlbumsHandler(IAlbumQueryRepository repo, IStorageService storage)
    {
        _repo = repo;
        _storage = storage;
    }

    public async Task<Result<IReadOnlyList<AlbumDto>>> Handle(ListAlbumsQuery request, CancellationToken ct)
    {
        var albums = await _repo.ListAsync(request.PublicOnly, ct);
        var result = albums
            .Select(a =>
            {
                var cover = a.Images.FirstOrDefault(i => i.ImageType == ImageType.Cover) ?? a.Images.FirstOrDefault();
                var coverUrl = cover is null ? null : _storage.GetPublicUrl(cover.StorageKey);
                var coverVariants = coverUrl is null ? null : BuildVariants(coverUrl);
                return new AlbumDto(
                    a.Id, a.Title, a.Slug, a.Description, a.EventDate, a.Client, a.Location,
                    a.SeoTitle, a.SeoDescription, a.CoverAltText,
                    a.ShowInPortfolio, a.ShowInStories, a.ShowInHome,
                    a.CategoryId, a.Images.Count, cover?.Id, coverUrl, cover?.Width, cover?.Height, coverVariants);
            })
            .ToList();
        return Result<IReadOnlyList<AlbumDto>>.Ok(result);
    }

    private static ImageVariantsDto BuildVariants(string publicUrl) => ImageVariantFactory.Build(publicUrl);
}

public sealed record GetAlbumByIdQuery(Guid Id) : IRequest<Result<AlbumDetailsDto>>;
public sealed record GetAlbumBySlugOrIdQuery(string SlugOrId) : IRequest<Result<AlbumDetailsDto>>;

public sealed class GetAlbumByIdHandler : IRequestHandler<GetAlbumByIdQuery, Result<AlbumDetailsDto>>
{
    private readonly IAlbumQueryRepository _repo;
    private readonly IStorageService _storage;

    public GetAlbumByIdHandler(IAlbumQueryRepository repo, IStorageService storage)
    {
        _repo = repo;
        _storage = storage;
    }

    public async Task<Result<AlbumDetailsDto>> Handle(GetAlbumByIdQuery request, CancellationToken ct)
    {
        var album = await _repo.GetByIdAsync(request.Id, ct);
        if (album is null) return Result<AlbumDetailsDto>.NotFound("Album not found");

        return Result<AlbumDetailsDto>.Ok(AlbumDtoMapper.ToDetails(album, _storage));
    }
}

public sealed class GetAlbumBySlugOrIdHandler : IRequestHandler<GetAlbumBySlugOrIdQuery, Result<AlbumDetailsDto>>
{
    private readonly IAlbumQueryRepository _repo;
    private readonly IStorageService _storage;

    public GetAlbumBySlugOrIdHandler(IAlbumQueryRepository repo, IStorageService storage)
    {
        _repo = repo;
        _storage = storage;
    }

    public async Task<Result<AlbumDetailsDto>> Handle(GetAlbumBySlugOrIdQuery request, CancellationToken ct)
    {
        var album = Guid.TryParse(request.SlugOrId, out var id)
            ? await _repo.GetByIdAsync(id, ct)
            : await _repo.GetBySlugAsync(request.SlugOrId, ct);
        if (album is null) return Result<AlbumDetailsDto>.NotFound("Album not found");
        return Result<AlbumDetailsDto>.Ok(AlbumDtoMapper.ToDetails(album, _storage));
    }
}

internal static class AlbumDtoMapper
{
    public static AlbumDetailsDto ToDetails(Album album, IStorageService storage)
    {
        var images = album.Images
            .Select(i =>
            {
                var publicUrl = storage.GetPublicUrl(i.StorageKey);
                return new ImageDto(
                    i.Id, i.AlbumId, i.OriginalName, i.StorageKey,
                    publicUrl, ImageVariantFactory.Build(publicUrl),
                    i.Width, i.Height, i.Orientation, i.ImageType, i.SizeBytes);
            })
            .ToList();

        var cover = album.Images.FirstOrDefault(i => i.ImageType == ImageType.Cover) ?? album.Images.FirstOrDefault();
        var coverUrl = cover is null ? null : storage.GetPublicUrl(cover.StorageKey);
        var coverVariants = coverUrl is null ? null : ImageVariantFactory.Build(coverUrl);
        return new AlbumDetailsDto(
            album.Id, album.Title, album.Slug, album.Description, album.EventDate, album.Client, album.Location,
            album.SeoTitle, album.SeoDescription, album.CoverAltText,
            album.ShowInPortfolio, album.ShowInStories, album.ShowInHome,
            album.CategoryId, cover?.Id, coverUrl, cover?.Width, cover?.Height, coverVariants, images);
    }
}

internal static class ImageVariantFactory
{
    public static ImageVariantsDto Build(string publicUrl) => new(
        Placeholder: WithWidth(publicUrl, 40),
        Thumbnail: WithWidth(publicUrl, 240),
        Card: WithWidth(publicUrl, 640),
        Hero: WithWidth(publicUrl, 1600),
        Full: publicUrl);

    private static string WithWidth(string publicUrl, int width)
    {
        var separator = publicUrl.Contains('?', StringComparison.Ordinal) ? '&' : '?';
        return $"{publicUrl}{separator}width={width}";
    }
}
