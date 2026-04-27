using MediatR;
using Photography.Application.Storage;
using Photography.Core.Albums;
using Photography.SharedKernel;

namespace Photography.Application.Albums.Commands;

/// <summary>
/// Adds an image to an album. Caller is responsible for providing image dimensions
/// (extracted at the API edge before invoking this command) so the Application layer
/// stays free of image-decoding dependencies.
/// </summary>
public sealed record AddImageCommand(
    Guid AlbumId,
    string OriginalName,
    string ContentType,
    long SizeBytes,
    int Width,
    int Height,
    Stream Content,
    string? Checksum = null) : IRequest<Result<Guid>>;

public sealed class AddImageHandler : IRequestHandler<AddImageCommand, Result<Guid>>
{
    private readonly IAlbumCommandRepository _albums;
    private readonly IStorageService _storage;

    public AddImageHandler(IAlbumCommandRepository albums, IStorageService storage)
    {
        _albums = albums;
        _storage = storage;
    }

    public async Task<Result<Guid>> Handle(AddImageCommand request, CancellationToken ct)
    {
        var album = await _albums.GetForUpdateAsync(request.AlbumId, ct);
        if (album is null) return Result<Guid>.NotFound("Album not found");

        var imageId = Guid.NewGuid();
        var key = StorageKeys.ImageKey(album.Id, imageId, request.OriginalName);

        await _storage.UploadAsync(key, request.Content, request.ContentType, ct);

        try
        {
            album.AddImage(
                imageId: imageId,
                originalName: request.OriginalName,
                storageKey: key,
                contentType: request.ContentType,
                sizeBytes: request.SizeBytes,
                width: request.Width,
                height: request.Height,
                checksum: request.Checksum);
            await _albums.SaveChangesAsync(ct);
            return Result<Guid>.Ok(imageId);
        }
        catch (ArgumentException ex)
        {
            // Roll back the uploaded blob on validation failure to avoid orphans.
            try { await _storage.DeleteAsync(key, ct); } catch { /* ignored */ }
            return Result<Guid>.Fail(ex.Message);
        }
    }
}

public sealed record DeleteImageCommand(Guid AlbumId, Guid ImageId) : IRequest<Result>;

public sealed class DeleteImageHandler : IRequestHandler<DeleteImageCommand, Result>
{
    private readonly IAlbumCommandRepository _albums;
    private readonly IStorageService _storage;

    public DeleteImageHandler(IAlbumCommandRepository albums, IStorageService storage)
    {
        _albums = albums;
        _storage = storage;
    }

    public async Task<Result> Handle(DeleteImageCommand request, CancellationToken ct)
    {
        var album = await _albums.GetForUpdateAsync(request.AlbumId, ct);
        if (album is null) return Result.NotFound("Album not found");

        var image = album.Images.FirstOrDefault(i => i.Id == request.ImageId);
        if (image is null) return Result.NotFound("Image not found");

        var key = image.StorageKey;
        album.RemoveImage(image.Id);
        await _albums.SaveChangesAsync(ct);

        try { await _storage.DeleteAsync(key, ct); } catch { /* ignored */ }
        return Result.Ok();
    }
}

public sealed record SetCoverImageCommand(Guid AlbumId, Guid ImageId) : IRequest<Result>;

public sealed class SetCoverImageHandler : IRequestHandler<SetCoverImageCommand, Result>
{
    private readonly IAlbumCommandRepository _albums;

    public SetCoverImageHandler(IAlbumCommandRepository albums) => _albums = albums;

    public async Task<Result> Handle(SetCoverImageCommand request, CancellationToken ct)
    {
        var album = await _albums.GetForUpdateAsync(request.AlbumId, ct);
        if (album is null) return Result.NotFound("Album not found");

        try
        {
            album.SetCover(request.ImageId);
            await _albums.SaveChangesAsync(ct);
            return Result.Ok();
        }
        catch (InvalidOperationException ex)
        {
            return Result.Fail(ex.Message);
        }
    }
}
