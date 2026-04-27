using MediatR;
using Photography.Application.Common.Imaging;
using Photography.Application.Storage;
using Photography.Core.Albums;
using Photography.SharedKernel;

namespace Photography.Application.Albums.Commands;

/// <summary>
/// Adds an image to an album. The handler decodes pixel dimensions from the supplied
/// stream via <see cref="IImageMetadataReader"/> so callers don't need to pre-process.
/// The original image is uploaded to <see cref="IStorageService"/> using the
/// canonical <c>StorageKeys.ImageKey</c> layout; on validation failure the uploaded
/// blob is removed so the storage doesn't accumulate orphans.
/// </summary>
public sealed record AddImageCommand(
    Guid AlbumId,
    string OriginalName,
    string ContentType,
    long SizeBytes,
    Stream Content,
    string? Checksum = null) : IRequest<Result<Guid>>;

public sealed class AddImageHandler(
    IAlbumCommandRepository albums,
    IStorageService storage,
    IImageMetadataReader metadataReader) : IRequestHandler<AddImageCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(AddImageCommand request, CancellationToken ct)
    {
        var album = await albums.GetForUpdateAsync(request.AlbumId, ct);
        if (album is null) return Result<Guid>.NotFound("Album not found");

        var dimensions = await metadataReader.ReadAsync(request.Content, ct);
        if (dimensions is null)
            return Result<Guid>.Fail("Uploaded file is not a recognised image");

        var imageId = Guid.NewGuid();
        var key = StorageKeys.ImageKey(album.Id, imageId, request.OriginalName);

        await storage.UploadAsync(key, request.Content, request.ContentType, ct);

        try
        {
            album.AddImage(
                imageId: imageId,
                originalName: request.OriginalName,
                storageKey: key,
                contentType: request.ContentType,
                sizeBytes: request.SizeBytes,
                width: dimensions.Width,
                height: dimensions.Height,
                checksum: request.Checksum);
            await albums.SaveChangesAsync(ct);
            return Result<Guid>.Ok(imageId);
        }
        catch (ArgumentException ex)
        {
            try { await storage.DeleteAsync(key, ct); } catch { /* ignored */ }
            return Result<Guid>.Fail(ex.Message);
        }
    }
}

public sealed record DeleteImageCommand(Guid AlbumId, Guid ImageId) : IRequest<Result>;

public sealed class DeleteImageHandler(
    IAlbumCommandRepository albums,
    IStorageService storage) : IRequestHandler<DeleteImageCommand, Result>
{
    public async Task<Result> Handle(DeleteImageCommand request, CancellationToken ct)
    {
        var album = await albums.GetForUpdateAsync(request.AlbumId, ct);
        if (album is null) return Result.NotFound("Album not found");

        var image = album.Images.FirstOrDefault(i => i.Id == request.ImageId);
        if (image is null) return Result.NotFound("Image not found");

        var key = image.StorageKey;
        album.RemoveImage(image.Id);
        await albums.SaveChangesAsync(ct);

        try { await storage.DeleteAsync(key, ct); } catch { /* ignored */ }
        return Result.Ok();
    }
}

public sealed record SetCoverImageCommand(Guid AlbumId, Guid ImageId) : IRequest<Result>;

public sealed class SetCoverImageHandler(IAlbumCommandRepository albums)
    : IRequestHandler<SetCoverImageCommand, Result>
{
    public async Task<Result> Handle(SetCoverImageCommand request, CancellationToken ct)
    {
        var album = await albums.GetForUpdateAsync(request.AlbumId, ct);
        if (album is null) return Result.NotFound("Album not found");

        try
        {
            album.SetCover(request.ImageId);
            await albums.SaveChangesAsync(ct);
            return Result.Ok();
        }
        catch (InvalidOperationException ex)
        {
            return Result.Fail(ex.Message);
        }
    }
}
