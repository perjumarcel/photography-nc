namespace Photography.Application.Categories.Dtos;

public sealed record CategoryDto(int Id, string Name, string? Slug, int DisplayOrder, bool ShowAsFilter);

public sealed record CreateCategoryDto(string Name, string? Slug, int DisplayOrder, bool ShowAsFilter = true);

public sealed record UpdateCategoryDto(string Name, string? Slug, int DisplayOrder, bool ShowAsFilter);
