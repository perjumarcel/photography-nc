using Photography.SharedKernel;

namespace Photography.Core.Tags;

public class Tag : EntityBase<int>
{
    public const int MaxNameLength = 64;

    public string Name { get; private set; } = string.Empty;

    private Tag() { }

    public static Tag Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Tag name is required", nameof(name));
        if (name.Length > MaxNameLength)
            throw new ArgumentException($"Tag name exceeds {MaxNameLength} characters", nameof(name));

        return new Tag
        {
            Name = name.Trim(),
            CreatedAtUtc = DateTime.UtcNow,
        };
    }
}
