using Photography.SharedKernel;

namespace Photography.Core.Users;

/// <summary>
/// Application user. The site has a single Admin role today (mirroring the legacy
/// ABP back-office), but the role string is preserved so we can grow into editor /
/// viewer scopes without a schema change.
///
/// Passwords are stored as PBKDF2-HMACSHA256 hashes (see <c>IPasswordHasher</c> in
/// the Application layer); the entity only stores the opaque string and a separate
/// hashed refresh-token used by the rotation flow.
/// </summary>
public sealed class User : EntityBase<Guid>
{
    public const int MaxEmailLength = 256;
    public const int MaxRoleLength = 32;

    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string Role { get; private set; } = "Admin";
    public string? RefreshTokenHash { get; private set; }
    public DateTime? RefreshTokenExpiresAtUtc { get; private set; }
    public DateTime? LastLoginAtUtc { get; private set; }

    private User() { }

    public static User Create(Guid id, string email, string passwordHash, string role = "Admin")
    {
        ValidateEmail(email);
        ArgumentException.ThrowIfNullOrWhiteSpace(passwordHash);
        ArgumentException.ThrowIfNullOrWhiteSpace(role);
        if (role.Length > MaxRoleLength)
            throw new ArgumentException($"Role exceeds {MaxRoleLength} characters", nameof(role));

        return new User
        {
            Id = id == Guid.Empty ? Guid.NewGuid() : id,
            Email = email.Trim().ToLowerInvariant(),
            PasswordHash = passwordHash,
            Role = role,
            CreatedAtUtc = DateTime.UtcNow,
        };
    }

    public void SetPasswordHash(string passwordHash)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(passwordHash);
        PasswordHash = passwordHash;
        Touch();
    }

    public void IssueRefreshToken(string refreshTokenHash, DateTime expiresAtUtc)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(refreshTokenHash);
        RefreshTokenHash = refreshTokenHash;
        RefreshTokenExpiresAtUtc = expiresAtUtc;
        LastLoginAtUtc = DateTime.UtcNow;
        Touch();
    }

    public void RevokeRefreshToken()
    {
        RefreshTokenHash = null;
        RefreshTokenExpiresAtUtc = null;
        Touch();
    }

    /// <summary>
    /// Returns <c>true</c> when the supplied refresh-token hash matches the stored
    /// hash and the token has not expired (compared against <paramref name="nowUtc"/>).
    /// </summary>
    public bool RefreshTokenMatches(string candidateHash, DateTime nowUtc)
    {
        if (RefreshTokenHash is null || RefreshTokenExpiresAtUtc is null) return false;
        if (RefreshTokenExpiresAtUtc <= nowUtc) return false;
        return System.Security.Cryptography.CryptographicOperations.FixedTimeEquals(
            System.Text.Encoding.UTF8.GetBytes(RefreshTokenHash),
            System.Text.Encoding.UTF8.GetBytes(candidateHash));
    }

    private static void ValidateEmail(string email)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        if (email.Length > MaxEmailLength)
            throw new ArgumentException($"Email exceeds {MaxEmailLength} characters", nameof(email));
        if (!email.Contains('@'))
            throw new ArgumentException("Email must contain '@'", nameof(email));
    }
}
