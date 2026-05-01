using Photography.Application.Common.Routing;
using Xunit;

namespace Photography.Application.Tests;

public sealed class LegacyRedirectsTests
{
    [Theory]
    [InlineData("diana-oleg-wedding", "/portfolio/diana-oleg-wedding")]
    [InlineData("/familia-caus/", "/portfolio/familia-caus")]
    [InlineData(" wedding-in-tenerife ", "/portfolio/wedding-in-tenerife")]
    public void AlbumDetailsTarget_BuildsCleanPortfolioSlugUrl(string slug, string expected)
    {
        var target = LegacyRedirects.AlbumDetailsTarget(slug);

        Assert.Equal(expected, target);
    }

    [Fact]
    public void AlbumDetailsTarget_RejectsMissingSlug()
    {
        Assert.Throws<ArgumentException>(() => LegacyRedirects.AlbumDetailsTarget(" "));
    }
}
