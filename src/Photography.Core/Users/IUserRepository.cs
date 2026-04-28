namespace Photography.Core.Users;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email, CancellationToken ct = default);
    Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default);

    /// <summary>
    /// Looks up a user by the (already hashed) refresh token they presented.
    /// We store the hash of the refresh token in the database — never the raw value —
    /// so this lookup is by `==` against the stored hash.
    /// </summary>
    Task<User?> GetByRefreshTokenHashAsync(string refreshTokenHash, CancellationToken ct = default);

    Task<bool> AnyAsync(CancellationToken ct = default);
    Task AddAsync(User user, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
