namespace Photography.Application.Common.Routing;

public static class LegacyRedirects
{
    public static string AlbumDetailsTarget(string slug)
    {
        if (string.IsNullOrWhiteSpace(slug))
            throw new ArgumentException("Album slug is required.", nameof(slug));

        return $"/portfolio/{slug.Trim().Trim('/')}";
    }
}
