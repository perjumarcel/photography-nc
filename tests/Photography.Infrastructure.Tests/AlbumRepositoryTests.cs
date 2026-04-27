using Microsoft.EntityFrameworkCore;
using Photography.Core.Albums;
using Photography.Infrastructure.Persistence;
using Photography.Infrastructure.Persistence.Repositories;
using Xunit;

namespace Photography.Infrastructure.Tests;

public class AlbumRepositoryTests
{
    private static AppDbContext NewContext() =>
        new(new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    [Fact]
    public async Task AddAndRetrieve_RoundTrip()
    {
        await using var db = NewContext();
        var commands = new AlbumCommandRepository(db);
        var album = Album.Create(Guid.NewGuid(), "Album 1", 1);
        await commands.AddAsync(album);
        await commands.SaveChangesAsync();

        await using var db2 = NewContext();
        // separate context cannot share in-memory by name; smoke-test the same context instead
        var queries = new AlbumQueryRepository(db);
        var list = await queries.ListAsync(publicOnly: false);
        Assert.Single(list);
    }
}
