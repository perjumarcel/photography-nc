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
                return new AlbumDto(
                    a.Id, a.Title, a.Description, a.EventDate, a.Client, a.Location,
                    a.ShowInPortfolio, a.ShowInStories, a.ShowInHome,
                    a.CategoryId, a.Images.Count, cover?.Id);
            })
            .ToList();
        return Result<IReadOnlyList<AlbumDto>>.Ok(result);
    }
}

public sealed record GetAlbumByIdQuery(Guid Id) : IRequest<Result<AlbumDetailsDto>>;

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

        var images = album.Images
            .Select(i => new ImageDto(
                i.Id, i.AlbumId, i.OriginalName, i.StorageKey,
                _storage.GetPublicUrl(i.StorageKey),
                i.Width, i.Height, i.Orientation, i.ImageType, i.SizeBytes))
            .ToList();

        var dto = new AlbumDetailsDto(
            album.Id, album.Title, album.Description, album.EventDate, album.Client, album.Location,
            album.ShowInPortfolio, album.ShowInStories, album.ShowInHome,
            album.CategoryId, images);
        return Result<AlbumDetailsDto>.Ok(dto);
    }
}
