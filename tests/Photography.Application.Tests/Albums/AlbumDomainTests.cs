using Photography.Core.Albums;
using Xunit;

namespace Photography.Application.Tests.Albums;

public class AlbumDomainTests
{
    [Fact]
    public void Create_WithValidData_Succeeds()
    {
        var album = Album.Create(Guid.NewGuid(), "Beach Wedding", categoryId: 1);
        Assert.Equal("Beach Wedding", album.Title);
        Assert.Equal("beach-wedding", album.Slug);
        Assert.Equal(1, album.CategoryId);
        Assert.Empty(album.Images);
    }

    [Fact]
    public void UpdateDetails_NormalizesSlugAndSeoMetadata()
    {
        var album = Album.Create(Guid.NewGuid(), "Beach Wedding", categoryId: 1);

        album.UpdateDetails(
            "New Title",
            "SEO Friendly Slug",
            categoryId: 2,
            description: "Description",
            eventDate: null,
            client: null,
            location: null,
            seoTitle: "  SEO title  ",
            seoDescription: "  SEO description  ",
            coverAltText: "  Cover alt  ");

        Assert.Equal("seo-friendly-slug", album.Slug);
        Assert.Equal("SEO title", album.SeoTitle);
        Assert.Equal("SEO description", album.SeoDescription);
        Assert.Equal("Cover alt", album.CoverAltText);
    }

    [Fact]
    public void Create_WithEmptyTitle_Throws()
    {
        Assert.Throws<ArgumentException>(() => Album.Create(Guid.NewGuid(), "", 1));
    }

    [Fact]
    public void AddImage_DerivesOrientationFromDimensions()
    {
        var album = Album.Create(Guid.NewGuid(), "T", 1);
        var horizontal = album.AddImage(Guid.NewGuid(), "h.jpg", "albums/x/images/h.jpg", "image/jpeg", 1024, 800, 600);
        var vertical = album.AddImage(Guid.NewGuid(), "v.jpg", "albums/x/images/v.jpg", "image/jpeg", 1024, 600, 800);
        Assert.Equal(ImageOrientation.Horizontal, horizontal.Orientation);
        Assert.Equal(ImageOrientation.Vertical, vertical.Orientation);
    }

    [Fact]
    public void SetCover_DemotesPreviousCover()
    {
        var album = Album.Create(Guid.NewGuid(), "T", 1);
        var a = album.AddImage(Guid.NewGuid(), "a.jpg", "k/a", "image/jpeg", 1, 100, 100);
        var b = album.AddImage(Guid.NewGuid(), "b.jpg", "k/b", "image/jpeg", 1, 100, 100);

        album.SetCover(a.Id);
        album.SetCover(b.Id);

        Assert.Equal(ImageType.Default, a.ImageType);
        Assert.Equal(ImageType.Cover, b.ImageType);
    }
}
