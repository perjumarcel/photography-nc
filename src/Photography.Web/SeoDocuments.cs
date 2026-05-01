using System.Net;
using System.Text;
using Photography.Application.Albums.Dtos;

namespace Photography.Web;

public static class SeoDocuments
{
    public static string BuildSitemapXml(string origin, IEnumerable<AlbumDto> albums)
    {
        var paths = new List<string> { "/", "/portfolio", "/stories", "/about", "/contact" };
        paths.AddRange(albums
            .Where(a => a.ShowInPortfolio || a.ShowInStories)
            .Select(a => $"/portfolio/{a.Slug}"));

        var xml = new StringBuilder();
        xml.AppendLine("""<?xml version="1.0" encoding="UTF-8"?>""");
        xml.AppendLine("""<urlset xmlns="http://www.sitemaps.org/schemas/sitemap/0.9">""");
        foreach (var path in paths.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            xml.Append("  <url><loc>")
                .Append(WebUtility.HtmlEncode(origin.TrimEnd('/') + path))
                .AppendLine("</loc></url>");
        }
        xml.AppendLine("</urlset>");
        return xml.ToString();
    }

    public static string BuildRobotsTxt(string origin)
        => $"User-agent: *\nAllow: /\nSitemap: {origin.TrimEnd('/')}/sitemap.xml\n";
}
