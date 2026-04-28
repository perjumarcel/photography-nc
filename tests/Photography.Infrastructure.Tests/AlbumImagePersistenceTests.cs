using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Photography.Core.Albums;
using Photography.Infrastructure.Persistence;
using Photography.Infrastructure.Persistence.Repositories;
using Xunit;

namespace Photography.Infrastructure.Tests;

/// <summary>
/// Regression coverage for the EF Core change-tracking issue where domain-generated
/// Guid keys (Album.Id, Image.Id) defaulted to <c>ValueGeneratedOnAdd</c>. When a new
/// <see cref="Image"/> was added to a tracked Album, EF saw the non-default Id and
/// emitted UPDATE…WHERE Id=&lt;new-guid&gt;, which affected 0 rows on a real relational
/// provider and threw <see cref="DbUpdateConcurrencyException"/>.
///
/// The InMemory provider hides this bug because it doesn't enforce row counts, so we
/// use SQLite (in-memory mode) here — it speaks the same change-tracking SQL contract
/// as Postgres for INSERT vs UPDATE.
/// </summary>
public sealed class AlbumImagePersistenceTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly DbContextOptions<AppDbContext> _options;

    public AlbumImagePersistenceTests()
    {
        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();
        _options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options;

        using var db = new AppDbContext(_options);
        db.Database.EnsureCreated();
    }

    public void Dispose()
    {
        _connection.Dispose();
    }

    [Fact]
    public async Task AddingImage_ToTrackedAlbum_Persists_Without_Concurrency_Exception()
    {
        var albumId = Guid.NewGuid();

        // Seed an album.
        await using (var db = new AppDbContext(_options))
        {
            db.Albums.Add(Album.Create(albumId, "Album", categoryId: 1));
            await db.SaveChangesAsync();
        }

        // Re-load it (tracked) and add an image via the aggregate root.
        await using (var db = new AppDbContext(_options))
        {
            var commands = new AlbumCommandRepository(db);
            var album = await commands.GetForUpdateAsync(albumId);
            Assert.NotNull(album);

            album!.AddImage(
                imageId: Guid.NewGuid(),
                originalName: "x.jpg",
                storageKey: $"albums/{albumId}/images/x.jpg",
                contentType: "image/jpeg",
                sizeBytes: 123,
                width: 100,
                height: 80);

            // The original bug threw DbUpdateConcurrencyException here because the new
            // Image was treated as Modified instead of Added.
            await commands.SaveChangesAsync();
        }

        // Verify persisted.
        await using (var db = new AppDbContext(_options))
        {
            var queries = new AlbumQueryRepository(db);
            var album = await queries.GetByIdAsync(albumId);
            Assert.NotNull(album);
            Assert.Single(album!.Images);
        }
    }
}
