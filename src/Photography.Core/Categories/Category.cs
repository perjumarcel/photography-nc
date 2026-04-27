using Photography.SharedKernel;

namespace Photography.Core.Categories;

public class Category : EntityBase<int>
{
    public const int MaxNameLength = 64;

    public string Name { get; private set; } = string.Empty;
    public string? Slug { get; private set; }
    public int DisplayOrder { get; private set; }

    private Category() { }

    public static Category Create(string name, string? slug = null, int displayOrder = 0)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Category name is required", nameof(name));
        if (name.Length > MaxNameLength)
            throw new ArgumentException($"Category name exceeds {MaxNameLength} characters", nameof(name));

        return new Category
        {
            Name = name.Trim(),
            Slug = string.IsNullOrWhiteSpace(slug) ? null : slug.Trim().ToLowerInvariant(),
            DisplayOrder = displayOrder,
            CreatedAtUtc = DateTime.UtcNow,
        };
    }

    public void Rename(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Category name is required", nameof(name));
        if (name.Length > MaxNameLength)
            throw new ArgumentException($"Category name exceeds {MaxNameLength} characters", nameof(name));
        Name = name.Trim();
        Touch();
    }

    public void SetDisplayOrder(int order)
    {
        DisplayOrder = order;
        Touch();
    }

    public void SetSlug(string? slug)
    {
        Slug = string.IsNullOrWhiteSpace(slug) ? null : slug.Trim().ToLowerInvariant();
        Touch();
    }
}
