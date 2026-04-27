using MediatR;
using Photography.Application.Categories.Dtos;
using Photography.Core.Albums;
using Photography.Core.Categories;
using Photography.SharedKernel;

namespace Photography.Application.Categories.Commands;

public sealed record CreateCategoryCommand(CreateCategoryDto Dto) : IRequest<Result<int>>;

public sealed class CreateCategoryHandler(ICategoryRepository repo) : IRequestHandler<CreateCategoryCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreateCategoryCommand request, CancellationToken ct)
    {
        try
        {
            var category = Category.Create(
                request.Dto.Name, request.Dto.Slug, request.Dto.DisplayOrder, request.Dto.ShowAsFilter);
            await repo.AddAsync(category, ct);
            await repo.SaveChangesAsync(ct);
            return Result<int>.Ok(category.Id);
        }
        catch (ArgumentException ex)
        {
            return Result<int>.Fail(ex.Message);
        }
    }
}

public sealed record UpdateCategoryCommand(int Id, UpdateCategoryDto Dto) : IRequest<Result>;

public sealed class UpdateCategoryHandler(ICategoryRepository repo) : IRequestHandler<UpdateCategoryCommand, Result>
{
    public async Task<Result> Handle(UpdateCategoryCommand request, CancellationToken ct)
    {
        var entity = await repo.GetByIdAsync(request.Id, ct);
        if (entity is null) return Result.NotFound("Category not found");
        try
        {
            entity.Rename(request.Dto.Name);
            entity.SetSlug(request.Dto.Slug);
            entity.SetDisplayOrder(request.Dto.DisplayOrder);
            entity.SetShowAsFilter(request.Dto.ShowAsFilter);
            await repo.SaveChangesAsync(ct);
            return Result.Ok();
        }
        catch (ArgumentException ex)
        {
            return Result.Fail(ex.Message);
        }
    }
}

public sealed record DeleteCategoryCommand(int Id) : IRequest<Result>;

/// <summary>
/// Deletes a category, refusing to do so when one or more albums still reference it
/// (we never null out FKs on the album side — the snapshot rule keeps that data stable).
/// </summary>
public sealed class DeleteCategoryHandler(
    ICategoryRepository categories,
    IAlbumQueryRepository albums) : IRequestHandler<DeleteCategoryCommand, Result>
{
    public async Task<Result> Handle(DeleteCategoryCommand request, CancellationToken ct)
    {
        var entity = await categories.GetByIdAsync(request.Id, ct);
        if (entity is null) return Result.NotFound("Category not found");

        if (await albums.AnyInCategoryAsync(request.Id, ct))
            return Result.Conflict("Category is in use by one or more albums");

        categories.Remove(entity);
        await categories.SaveChangesAsync(ct);
        return Result.Ok();
    }
}
