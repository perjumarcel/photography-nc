using MediatR;
using Microsoft.AspNetCore.Mvc;
using Photography.Application.Albums.Queries;
using Photography.Application.Categories.Queries;

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
}
