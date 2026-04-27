using MediatR;
using Photography.Application.Categories.Dtos;
using Photography.Core.Categories;
using Photography.SharedKernel;

namespace Photography.Application.Categories.Commands;

public sealed record CreateCategoryCommand(CreateCategoryDto Dto) : IRequest<Result<int>>;

public sealed class CreateCategoryHandler : IRequestHandler<CreateCategoryCommand, Result<int>>
{
    private readonly ICategoryRepository _repo;

    public CreateCategoryHandler(ICategoryRepository repo) => _repo = repo;

    public async Task<Result<int>> Handle(CreateCategoryCommand request, CancellationToken ct)
    {
        try
        {
            var category = Category.Create(request.Dto.Name, request.Dto.Slug, request.Dto.DisplayOrder);
            await _repo.AddAsync(category, ct);
            await _repo.SaveChangesAsync(ct);
            return Result<int>.Ok(category.Id);
        }
        catch (ArgumentException ex)
        {
            return Result<int>.Fail(ex.Message);
        }
    }
}

public sealed record UpdateCategoryCommand(int Id, UpdateCategoryDto Dto) : IRequest<Result>;

public sealed class UpdateCategoryHandler : IRequestHandler<UpdateCategoryCommand, Result>
{
    private readonly ICategoryRepository _repo;

    public UpdateCategoryHandler(ICategoryRepository repo) => _repo = repo;

    public async Task<Result> Handle(UpdateCategoryCommand request, CancellationToken ct)
    {
        var entity = await _repo.GetByIdAsync(request.Id, ct);
        if (entity is null) return Result.NotFound("Category not found");
        try
        {
            entity.Rename(request.Dto.Name);
            entity.SetSlug(request.Dto.Slug);
            entity.SetDisplayOrder(request.Dto.DisplayOrder);
            await _repo.SaveChangesAsync(ct);
            return Result.Ok();
        }
        catch (ArgumentException ex)
        {
            return Result.Fail(ex.Message);
        }
    }
}
