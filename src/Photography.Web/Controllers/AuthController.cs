using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Photography.Application.Auth.Commands;
using Photography.Application.Auth.Dtos;

namespace Photography.Web.Controllers;

/// <summary>
/// Authentication endpoints. Mirrors the legacy <c>AccountController</c> (login/logout)
/// but exposes a JSON API consumed by the React client. Refresh-token rotation is
/// performed server-side: each successful login or refresh invalidates the previous
/// refresh token by overwriting its hash.
/// </summary>
[ApiController]
[Route("api/auth")]
[EnableRateLimiting("auth")]
public sealed class AuthController(IMediator mediator) : ControllerBase
{
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(TokensDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto dto, CancellationToken ct)
    {
        var result = await mediator.Send(new LoginCommand(dto.Email, dto.Password), ct);
        return result.ToActionResult(this);
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(TokensDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequestDto dto, CancellationToken ct)
    {
        var result = await mediator.Send(new RefreshCommand(dto.RefreshToken), ct);
        return result.ToActionResult(this);
    }

    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Logout(CancellationToken ct)
    {
        var sub = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(sub, out var userId)) return NoContent();
        var result = await mediator.Send(new LogoutCommand(userId), ct);
        return result.ToActionResult(this);
    }
}
