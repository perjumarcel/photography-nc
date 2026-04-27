using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Photography.Application.Categories.Commands;
using Photography.Application.Categories.Dtos;
using Photography.Application.Categories.Queries;

namespace Photography.Web.Controllers;

[ApiController]
[Authorize(Policy = "AdminOnly")]
[Route("api/admin/categories")]
public sealed class AdminCategoriesController : ControllerBase
{
    private readonly IMediator _mediator;
    public AdminCategoriesController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> List(CancellationToken ct)
    {
        var result = await _mediator.Send(new ListCategoriesQuery(), ct);
        return result.ToActionResult(this);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCategoryDto dto, CancellationToken ct)
    {
        var result = await _mediator.Send(new CreateCategoryCommand(dto), ct);
        return result.ToActionResult(this, id => Created($"/api/admin/categories/{id}", new { id }));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCategoryDto dto, CancellationToken ct)
    {
        var result = await _mediator.Send(new UpdateCategoryCommand(id, dto), ct);
        return result.ToActionResult(this);
    }
}
