using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Photography.Application.Auth;
using Photography.Core.Categories;
using Photography.Core.Users;
using Photography.Infrastructure.Persistence;

namespace Photography.Web;

/// <summary>
/// Idempotent application seeding:
///   - Inserts the default category set (mirrors the legacy site's portfolio
///     navigation: Wedding, Portrait, Event, Travel) when the table is empty.
///   - Creates a single Admin user from configuration when no users exist.
///     The admin email/password are read from <c>Seed:AdminEmail</c> and
///     <c>Seed:AdminPassword</c>; if either is missing, seeding is skipped and a
///     warning is logged so an operator can complete bootstrap manually.
/// </summary>
public static class StartupSeeder
{
    private static readonly (string Name, string Slug, int Order)[] DefaultCategories =
    [
        ("Wedding",  "wedding",  10),
        ("Portrait", "portrait", 20),
        ("Event",    "event",    30),
        ("Travel",   "travel",   40),
    ];

    public static async Task SeedAsync(IServiceProvider services, CancellationToken ct = default)
    {
        await using var scope = services.CreateAsyncScope();
        var sp = scope.ServiceProvider;
        var logger = sp.GetRequiredService<ILogger<AppDbContext>>();
        var db = sp.GetRequiredService<AppDbContext>();
        var config = sp.GetRequiredService<IConfiguration>();
        var hasher = sp.GetRequiredService<IPasswordHasher>();
        var users = sp.GetRequiredService<IUserRepository>();

        if (!await db.Categories.AnyAsync(ct))
        {
            foreach (var (name, slug, order) in DefaultCategories)
                db.Categories.Add(Category.Create(name, slug, order));
            await db.SaveChangesAsync(ct);
            logger.LogInformation("Seeded {Count} default categories", DefaultCategories.Length);
        }

        if (!await users.AnyAsync(ct))
        {
            var email = config["Seed:AdminEmail"];
            var password = config["Seed:AdminPassword"];
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                logger.LogWarning(
                    "No users in the database and Seed:AdminEmail / Seed:AdminPassword are not configured — skipping admin bootstrap");
                return;
            }
            var admin = User.Create(Guid.NewGuid(), email, hasher.Hash(password), role: "Admin");
            await users.AddAsync(admin, ct);
            await users.SaveChangesAsync(ct);
            logger.LogInformation("Seeded initial admin user {Email}", admin.Email);
        }
    }
}
