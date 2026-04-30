using MediatR;
using Microsoft.AspNetCore.Mvc;
using Photography.Application.Albums.Queries;
using Photography.Application.Categories.Queries;
using System.Net;
using System.Text;

namespace Photography.Web.Controllers;

[ApiController]
[Route("api/public")]
public sealed class PublicController : ControllerBase
{
    private readonly IMediator _mediator;
    public PublicController(IMediator mediator) => _mediator = mediator;

    /// <summary>List public albums (visible in portfolio/home/stories).</summary>
    [HttpGet("albums")]
    public async Task<IActionResult> ListAlbums(CancellationToken ct)
    {
        var result = await _mediator.Send(new ListAlbumsQuery(PublicOnly: true), ct);
        return result.ToActionResult(this);
    }

    /// <summary>Get a single public album with images.</summary>
    [HttpGet("albums/{id:guid}")]
    public async Task<IActionResult> GetAlbum(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetAlbumByIdQuery(id), ct);
        return result.ToActionResult(this);
    }

    [HttpGet("categories")]
    public async Task<IActionResult> ListCategories(CancellationToken ct)
    {
        var result = await _mediator.Send(new ListCategoriesQuery(), ct);
        return result.ToActionResult(this);
    }

    [HttpGet("sitemap.xml")]
    [Produces("application/xml")]
    public async Task<IActionResult> Sitemap(CancellationToken ct)
    {
        var result = await _mediator.Send(new ListAlbumsQuery(PublicOnly: true), ct);
        if (result.IsFailed) return result.ToActionResult(this);

        var origin = $"{Request.Scheme}://{Request.Host}";
        var paths = new List<string> { "/", "/portfolio", "/stories", "/about", "/contact" };
        paths.AddRange((result.Value ?? []).Where(a => a.ShowInPortfolio || a.ShowInStories)
            .Select(a => $"/portfolio/{a.Id}"));

        var xml = new StringBuilder();
        xml.AppendLine("""<?xml version="1.0" encoding="UTF-8"?>""");
        xml.AppendLine("""<urlset xmlns="http://www.sitemaps.org/schemas/sitemap/0.9">""");
        foreach (var path in paths.Distinct())
        {
            xml.Append("  <url><loc>")
                .Append(WebUtility.HtmlEncode(origin + path))
                .AppendLine("</loc></url>");
        }
        xml.AppendLine("</urlset>");
        return Content(xml.ToString(), "application/xml", Encoding.UTF8);
    }

    [HttpGet("robots.txt")]
    [Produces("text/plain")]
    public IActionResult Robots()
    {
        var origin = $"{Request.Scheme}://{Request.Host}";
        return Content($"User-agent: *\nAllow: /\nSitemap: {origin}/api/public/sitemap.xml\n", "text/plain", Encoding.UTF8);
    }
}
