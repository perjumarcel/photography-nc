using Photography.SharedKernel;

namespace Photography.Core.Categories;

public class Category : EntityBase<int>
{
    public const int MaxNameLength = 64;

    public string Name { get; private set; } = string.Empty;
    public string? Slug { get; private set; }
    public int DisplayOrder { get; private set; }

    /// <summary>
    /// When <c>true</c> the category is offered as a filter chip on the public
    /// portfolio / stories pages. Mirrors the legacy <c>Category.ShowAsFilter</c>
    /// flag that powered the same UX.
    /// </summary>
    public bool ShowAsFilter { get; private set; } = true;

    private Category() { }

    public static Category Create(string name, string? slug = null, int displayOrder = 0, bool showAsFilter = true)
    {
        ValidateName(name);

        return new Category
        {
            Name = name.Trim(),
            Slug = NormalizeSlug(slug),
            DisplayOrder = displayOrder,
            ShowAsFilter = showAsFilter,
            CreatedAtUtc = DateTime.UtcNow,
        };
    }

    public void Rename(string name)
    {
        ValidateName(name);
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
        Slug = NormalizeSlug(slug);
        Touch();
    }

    public void SetShowAsFilter(bool value)
    {
        ShowAsFilter = value;
        Touch();
    }

    private static void ValidateName(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        if (name.Length > MaxNameLength)
            throw new ArgumentException($"Category name exceeds {MaxNameLength} characters", nameof(name));
    }

    private static string? NormalizeSlug(string? slug)
        => string.IsNullOrWhiteSpace(slug) ? null : slug.Trim().ToLowerInvariant();
}
