# Skill: API Controller Conventions

> Use this pattern when adding or modifying ASP.NET Core controllers.

## When to Apply

- Adding a new API endpoint
- Modifying an existing controller
- Mapping Result<T> to HTTP responses

## Route Structure

```
api/admin/{studioId:guid}/...   → [Authorize(Policy = "AdminOnly")] + StudioScopedActionFilter
api/client/...                  → [Authorize(Policy = "EmployeeOrAdmin")]
api/public/...                  → No authorization
api/booking-actions/...         → Public (token-based auth in request body)
```

## Controller Template

```csharp
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace ReflectStudio.Web.Controllers;

[ApiController]
[Route("api/admin/{studioId:guid}/items")]
[Authorize(Policy = "AdminOnly")]
[ServiceFilter(typeof(StudioScopedActionFilter))]
[EnableRateLimiting("general")]
public class ItemsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(Guid studioId)
    {
        var result = await mediator.Send(
            new GetItemsQuery(studioId), HttpContext.RequestAborted);
        return result.Success ? Ok(result.Data) : NotFound(result.Error);
    }

    [HttpGet("{itemId:guid}")]
    public async Task<IActionResult> GetById(Guid studioId, Guid itemId)
    {
        var result = await mediator.Send(
            new GetItemByIdQuery(studioId, itemId), HttpContext.RequestAborted);
        return result.Success ? Ok(result.Data) : NotFound(result.Error);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Guid studioId, [FromBody] CreateItemDto dto)
    {
        var result = await mediator.Send(
            new CreateItemCommand(studioId, dto), HttpContext.RequestAborted);
        return result.Success ? Created() : BadRequest(result.Error);
    }

    [HttpPut("{itemId:guid}")]
    public async Task<IActionResult> Update(Guid studioId, Guid itemId, [FromBody] UpdateItemDto dto)
    {
        var result = await mediator.Send(
            new UpdateItemCommand(studioId, itemId, dto), HttpContext.RequestAborted);
        return result.Success ? Ok(result.Data) : BadRequest(result.Error);
    }

    [HttpDelete("{itemId:guid}")]
    public async Task<IActionResult> Delete(Guid studioId, Guid itemId)
    {
        var result = await mediator.Send(
            new DeleteItemCommand(studioId, itemId), HttpContext.RequestAborted);
        return result.Success ? NoContent() : NotFound(result.Error);
    }
}
```

## Result<T> → HTTP Response Mapping

| Result | HTTP Response | When |
|--------|--------------|------|
| `result.Success` (GET) | `200 OK` | Successful read |
| `result.Success` (POST create) | `201 Created` | Resource created |
| `result.Success` (PUT) | `200 OK` | Full update succeeded |
| `result.Success` (DELETE) | `204 No Content` | Resource deleted |
| `result.Success` (action) | `200 OK` / `204 No Content` | State change succeeded |
| `Result.Fail("not found")` | `404 NotFound` | Entity doesn't exist |
| `Result.Fail("validation")` | `400 BadRequest` | Domain validation failed |
| `Result.Fail("conflict")` | `409 Conflict` | Concurrency / duplicate |

## Controller Rules

1. **Zero business logic** — only `mediator.Send()` + response mapping.
2. **Primary constructor** for DI: `ItemsController(IMediator mediator)`.
3. **Always pass `HttpContext.RequestAborted`** as CancellationToken.
4. **Never use `User.FindFirst(ClaimTypes.Email)?.Value`** — use `ClaimsPrincipalExtensions.GetEmail()` / `.GetRequiredEmail()`.
5. **StudioScopedActionFilter** on all admin routes — validates `studioId` belongs to the authenticated user.
6. **Rate limiting** via `[EnableRateLimiting("general")]` or specific policy.

## Checklist

- [ ] Controller is `[ApiController]` + `ControllerBase`
- [ ] Route follows `api/{scope}/{studioId?}/resource` pattern
- [ ] Appropriate `[Authorize(Policy)]` attribute
- [ ] `[ServiceFilter(typeof(StudioScopedActionFilter))]` for admin routes
- [ ] `[EnableRateLimiting]` attribute applied
- [ ] Primary constructor with `IMediator mediator`
- [ ] All actions pass `HttpContext.RequestAborted` to mediator
- [ ] Result<T> mapped to correct HTTP status codes
- [ ] No business logic — only delegation
- [ ] DTOs for request/response — never domain entities
