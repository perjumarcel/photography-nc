using MediatR;
using Photography.Application.Categories.Dtos;
using Photography.Core.Categories;
using Photography.SharedKernel;

namespace Photography.Application.Categories.Queries;

public sealed record ListCategoriesQuery() : IRequest<Result<IReadOnlyList<CategoryDto>>>;

public sealed class ListCategoriesHandler : IRequestHandler<ListCategoriesQuery, Result<IReadOnlyList<CategoryDto>>>
{
    private readonly ICategoryRepository _repo;

    public ListCategoriesHandler(ICategoryRepository repo) => _repo = repo;

    public async Task<Result<IReadOnlyList<CategoryDto>>> Handle(ListCategoriesQuery request, CancellationToken ct)
    {
        var categories = await _repo.ListAsync(ct);
        return Result<IReadOnlyList<CategoryDto>>.Ok(
            categories.Select(c => new CategoryDto(c.Id, c.Name, c.Slug, c.DisplayOrder)).ToList());
    }
}
