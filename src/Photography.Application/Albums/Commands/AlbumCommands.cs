using MediatR;
using Photography.Application.Albums.Dtos;
using Photography.Core.Albums;
using Photography.Core.Categories;
using Photography.SharedKernel;
using System.Globalization;
using System.Text;

namespace Photography.Application.Albums.Commands;

public sealed record CreateAlbumCommand(CreateAlbumDto Dto) : IRequest<Result<Guid>>;

public sealed class CreateAlbumHandler : IRequestHandler<CreateAlbumCommand, Result<Guid>>
{
    private readonly IAlbumCommandRepository _albums;
    private readonly ICategoryRepository _categories;

    public CreateAlbumHandler(IAlbumCommandRepository albums, ICategoryRepository categories)
    {
        _albums = albums;
        _categories = categories;
    }

    public async Task<Result<Guid>> Handle(CreateAlbumCommand request, CancellationToken ct)
    {
        var dto = request.Dto;
        if (!await _categories.ExistsAsync(dto.CategoryId, ct))
            return Result<Guid>.Fail("Category does not exist");

        try
        {
            var slug = await AlbumSlugGenerator.CreateUniqueAsync(
                dto.Slug ?? dto.Title,
                candidate => _albums.SlugExistsAsync(candidate, excludingAlbumId: null, ct),
                ct);

            var album = Album.Create(
                id: Guid.NewGuid(),
                title: dto.Title,
                categoryId: dto.CategoryId,
                slug: slug,
                description: dto.Description,
                eventDate: dto.EventDate,
                client: dto.Client,
                location: dto.Location,
                seoTitle: dto.SeoTitle,
                seoDescription: dto.SeoDescription,
                coverAltText: dto.CoverAltText,
                showInPortfolio: dto.ShowInPortfolio,
                showInStories: dto.ShowInStories,
                showInHome: dto.ShowInHome);

            await _albums.AddAsync(album, ct);
            await _albums.SaveChangesAsync(ct);
            return Result<Guid>.Ok(album.Id);
        }
        catch (ArgumentException ex)
        {
            return Result<Guid>.Fail(ex.Message);
        }
    }
}

public sealed record UpdateAlbumCommand(Guid Id, UpdateAlbumDto Dto) : IRequest<Result>;

public sealed class UpdateAlbumHandler : IRequestHandler<UpdateAlbumCommand, Result>
{
    private readonly IAlbumCommandRepository _albums;
    private readonly ICategoryRepository _categories;

    public UpdateAlbumHandler(IAlbumCommandRepository albums, ICategoryRepository categories)
    {
        _albums = albums;
        _categories = categories;
    }

    public async Task<Result> Handle(UpdateAlbumCommand request, CancellationToken ct)
    {
        var album = await _albums.GetForUpdateAsync(request.Id, ct);
        if (album is null) return Result.NotFound("Album not found");

        if (!await _categories.ExistsAsync(request.Dto.CategoryId, ct))
            return Result.Fail("Category does not exist");

        try
        {
            var slug = await AlbumSlugGenerator.CreateUniqueAsync(
                request.Dto.Slug ?? request.Dto.Title,
                candidate => _albums.SlugExistsAsync(candidate, excludingAlbumId: album.Id, ct),
                ct);

            album.UpdateDetails(
                request.Dto.Title,
                slug,
                request.Dto.CategoryId,
                request.Dto.Description,
                request.Dto.EventDate,
                request.Dto.Client,
                request.Dto.Location,
                request.Dto.SeoTitle,
                request.Dto.SeoDescription,
                request.Dto.CoverAltText);
            album.SetVisibility(request.Dto.ShowInPortfolio, request.Dto.ShowInStories, request.Dto.ShowInHome);
            await _albums.SaveChangesAsync(ct);
            return Result.Ok();
        }
        catch (ArgumentException ex)
        {
            return Result.Fail(ex.Message);
        }
    }
}

internal static class AlbumSlugGenerator
{
    public static async Task<string> CreateUniqueAsync(
        string source,
        Func<string, Task<bool>> existsAsync,
        CancellationToken ct)
    {
        var baseSlug = Normalize(source);
        var candidate = baseSlug;
        var suffix = 2;
        while (await existsAsync(candidate))
        {
            ct.ThrowIfCancellationRequested();
            var suffixText = $"-{suffix++}";
            var maxBaseLength = Math.Max(1, Album.MaxSlugLength - suffixText.Length);
            candidate = $"{baseSlug[..Math.Min(baseSlug.Length, maxBaseLength)].Trim('-')}{suffixText}";
        }
        return candidate;
    }

    public static string Normalize(string source)
    {
        var normalized = (string.IsNullOrWhiteSpace(source) ? "album" : source.Trim())
            .Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder(normalized.Length);
        var previousWasDash = false;

        foreach (var ch in normalized)
        {
            var category = CharUnicodeInfo.GetUnicodeCategory(ch);
            if (category == UnicodeCategory.NonSpacingMark) continue;

            if (char.IsLetterOrDigit(ch))
            {
                builder.Append(char.ToLowerInvariant(ch));
                previousWasDash = false;
            }
            else if (!previousWasDash)
            {
                builder.Append('-');
                previousWasDash = true;
            }
        }

        var slug = builder.ToString().Trim('-');
        if (string.IsNullOrWhiteSpace(slug)) slug = "album";
        return slug.Length <= Album.MaxSlugLength ? slug : slug[..Album.MaxSlugLength].Trim('-');
    }
}

public sealed record DeleteAlbumCommand(Guid Id) : IRequest<Result>;

public sealed class DeleteAlbumHandler : IRequestHandler<DeleteAlbumCommand, Result>
{
    private readonly IAlbumCommandRepository _albums;
    private readonly Storage.IStorageService _storage;

    public DeleteAlbumHandler(IAlbumCommandRepository albums, Storage.IStorageService storage)
    {
        _albums = albums;
        _storage = storage;
    }

    public async Task<Result> Handle(DeleteAlbumCommand request, CancellationToken ct)
    {
        var album = await _albums.GetForUpdateAsync(request.Id, ct);
        if (album is null) return Result.NotFound("Album not found");

        // Snapshot keys before mutating the aggregate.
        var keys = album.Images.Select(i => i.StorageKey).ToList();
        _albums.Remove(album);
        await _albums.SaveChangesAsync(ct);

        // Best-effort storage cleanup. Failures are not fatal — they are logged
        // by the storage implementation and reconciled by a background sweep.
        foreach (var key in keys)
        {
            try { await _storage.DeleteAsync(key, ct); }
            catch { /* swallow — recorded by storage implementation */ }
        }
        return Result.Ok();
    }
}
