using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Photography.Application.Contact.Commands;
using Photography.Application.Contact.Dtos;

namespace Photography.Web.Controllers;

/// <summary>
/// Public contact form endpoint. Replaces the legacy <c>ContactController</c> which
/// only rendered a Razor view; the React client now POSTs JSON here.
/// </summary>
[ApiController]
[Route("api/public/contact")]
[EnableRateLimiting("general")]
public sealed class ContactController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Send([FromBody] ContactMessageDto dto, CancellationToken ct)
    {
        var result = await mediator.Send(new SendContactMessageCommand(dto), ct);
        return result.ToActionResult(this);
    }
}
