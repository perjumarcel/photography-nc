using Moq;
using Photography.Application.Albums.Queries;
using Photography.Application.Storage;
using Photography.Core.Albums;
using Xunit;

namespace Photography.Application.Tests.Albums;

public class AlbumQueryHandlerTests
{
    [Fact]
    public async Task ListAlbums_IncludesCoverUrlAndResponsiveVariants()
    {
        var album = Album.Create(Guid.NewGuid(), "Album", 1, showInPortfolio: true);
        var first = album.AddImage(Guid.NewGuid(), "first.jpg", "albums/a/images/first.jpg", "image/jpeg", 1, 800, 600);
        var cover = album.AddImage(Guid.NewGuid(), "cover.jpg", "albums/a/images/cover.jpg", "image/jpeg", 1, 1200, 800);
        album.SetCover(cover.Id);

        var repo = new Mock<IAlbumQueryRepository>();
        repo.Setup(x => x.ListAsync(true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { album });

        var storage = new Mock<IStorageService>();
        storage.Setup(x => x.GetPublicUrl(first.StorageKey)).Returns("https://cdn.test/first.jpg");
        storage.Setup(x => x.GetPublicUrl(cover.StorageKey)).Returns("https://cdn.test/cover.jpg");

        var result = await new ListAlbumsHandler(repo.Object, storage.Object)
            .Handle(new ListAlbumsQuery(PublicOnly: true), CancellationToken.None);

        Assert.True(result.IsSuccess);
        var dto = Assert.Single(result.Value!);
        Assert.Equal(cover.Id, dto.CoverImageId);
        Assert.Equal("https://cdn.test/cover.jpg", dto.CoverPublicUrl);
        Assert.Equal("album", dto.Slug);
        Assert.Equal(1200, dto.CoverWidth);
        Assert.Equal(800, dto.CoverHeight);
        Assert.Equal("https://cdn.test/cover.jpg?width=640", dto.CoverVariants?.Card);
        Assert.Equal("https://cdn.test/cover.jpg", dto.CoverVariants?.Full);
    }

    [Fact]
    public async Task GetAlbumBySlugOrId_ResolvesReadableSlug()
    {
        var album = Album.Create(Guid.NewGuid(), "Family Session", categoryId: 1, slug: "family-session");
        var cover = album.AddImage(Guid.NewGuid(), "cover.jpg", "albums/a/images/cover.jpg", "image/jpeg", 1, 1200, 800);
        album.SetCover(cover.Id);

        var repo = new Mock<IAlbumQueryRepository>();
        repo.Setup(x => x.GetBySlugAsync("family-session", It.IsAny<CancellationToken>()))
            .ReturnsAsync(album);

        var storage = new Mock<IStorageService>();
        storage.Setup(x => x.GetPublicUrl(cover.StorageKey)).Returns("https://cdn.test/cover.jpg");

        var result = await new GetAlbumBySlugOrIdHandler(repo.Object, storage.Object)
            .Handle(new GetAlbumBySlugOrIdQuery("family-session"), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(album.Id, result.Value?.Id);
        Assert.Equal("family-session", result.Value?.Slug);
        Assert.Equal("https://cdn.test/cover.jpg?width=1600", result.Value?.CoverVariants?.Hero);
    }
}
