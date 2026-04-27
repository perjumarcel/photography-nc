using Microsoft.EntityFrameworkCore;
using Photography.Core.Albums;
using Photography.Core.Categories;
using Photography.Core.Tags;

namespace Photography.Infrastructure.Persistence;

/// <summary>
/// EF Core context for the Photography app. PostgreSQL via Npgsql.
/// Configured purely via Fluent API in <c>Configurations/</c> — no data annotations on entities.
/// Always use migrations; never <c>EnsureCreated()</c>.
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Album> Albums => Set<Album>();
    public DbSet<Image> Images => Set<Image>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Tag> Tags => Set<Tag>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
