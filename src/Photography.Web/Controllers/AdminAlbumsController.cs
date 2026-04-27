using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Photography.Application.Albums.Commands;
using Photography.Application.Albums.Dtos;
using Photography.Application.Albums.Queries;

namespace Photography.Web.Controllers;

[ApiController]
[Authorize(Policy = "AdminOnly")]
[Route("api/admin/albums")]
public sealed class AdminAlbumsController : ControllerBase
{
    private readonly IMediator _mediator;
    public AdminAlbumsController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> List(CancellationToken ct)
    {
        var result = await _mediator.Send(new ListAlbumsQuery(PublicOnly: false), ct);
        return result.ToActionResult(this);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetAlbumByIdQuery(id), ct);
        return result.ToActionResult(this);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAlbumDto dto, CancellationToken ct)
    {
        var result = await _mediator.Send(new CreateAlbumCommand(dto), ct);
        return result.ToActionResult(this, id => Created($"/api/admin/albums/{id}", new { id }));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAlbumDto dto, CancellationToken ct)
    {
        var result = await _mediator.Send(new UpdateAlbumCommand(id, dto), ct);
        return result.ToActionResult(this);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new DeleteAlbumCommand(id), ct);
        return result.ToActionResult(this);
    }

    [HttpPost("{id:guid}/images")]
    [RequestSizeLimit(50_000_000)]
    public async Task<IActionResult> UploadImage(Guid id, [FromForm] IFormFile file, CancellationToken ct)
    {
        if (file is null || file.Length == 0) return BadRequest(new { error = "File is required" });

        // The handler decodes width/height with ImageSharp; we just hand it a seekable
        // stream by buffering the form file into memory (capped by RequestSizeLimit).
        await using var seekable = new MemoryStream();
        await file.CopyToAsync(seekable, ct);
        seekable.Position = 0;

        var result = await _mediator.Send(new AddImageCommand(
            AlbumId: id,
            OriginalName: file.FileName,
            ContentType: file.ContentType,
            SizeBytes: file.Length,
            Content: seekable,
            Checksum: null), ct);
        return result.ToActionResult(this, imgId => Created($"/api/admin/albums/{id}/images/{imgId}", new { id = imgId }));
    }

    [HttpDelete("{albumId:guid}/images/{imageId:guid}")]
    public async Task<IActionResult> DeleteImage(Guid albumId, Guid imageId, CancellationToken ct)
    {
        var result = await _mediator.Send(new DeleteImageCommand(albumId, imageId), ct);
        return result.ToActionResult(this);
    }

    [HttpPatch("{albumId:guid}/images/{imageId:guid}/cover")]
    public async Task<IActionResult> SetCover(Guid albumId, Guid imageId, CancellationToken ct)
    {
        var result = await _mediator.Send(new SetCoverImageCommand(albumId, imageId), ct);
        return result.ToActionResult(this);
    }
}
