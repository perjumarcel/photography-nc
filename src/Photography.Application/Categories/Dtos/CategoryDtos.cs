namespace Photography.Application.Categories.Dtos;

public sealed record CategoryDto(int Id, string Name, string? Slug, int DisplayOrder);

public sealed record CreateCategoryDto(string Name, string? Slug, int DisplayOrder);

public sealed record UpdateCategoryDto(string Name, string? Slug, int DisplayOrder);
