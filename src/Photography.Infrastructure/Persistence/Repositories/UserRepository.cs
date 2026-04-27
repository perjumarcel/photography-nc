using Microsoft.EntityFrameworkCore;
using Photography.Core.Users;

namespace Photography.Infrastructure.Persistence.Repositories;

/// <summary>
/// EF Core implementation of <see cref="IUserRepository"/>.
/// Uses a primary constructor to inline the DbContext — a .NET / C# 12+ idiom that
/// keeps repository plumbing minimal.
/// </summary>
public sealed class UserRepository(AppDbContext db) : IUserRepository
{
    public Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
    {
        var normalized = (email ?? string.Empty).Trim().ToLowerInvariant();
        return db.Users.FirstOrDefaultAsync(u => u.Email == normalized, ct);
    }

    public Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        db.Users.FirstOrDefaultAsync(u => u.Id == id, ct);

    public Task<User?> GetByRefreshTokenHashAsync(string refreshTokenHash, CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(refreshTokenHash)) return Task.FromResult<User?>(null);
        return db.Users.FirstOrDefaultAsync(u => u.RefreshTokenHash == refreshTokenHash, ct);
    }

    public Task<bool> AnyAsync(CancellationToken ct = default) => db.Users.AnyAsync(ct);

    public async Task AddAsync(User user, CancellationToken ct = default) =>
        await db.Users.AddAsync(user, ct);

    public Task SaveChangesAsync(CancellationToken ct = default) => db.SaveChangesAsync(ct);
}
