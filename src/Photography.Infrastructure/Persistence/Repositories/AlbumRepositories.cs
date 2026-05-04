using Microsoft.EntityFrameworkCore;
using Photography.Core.Albums;

namespace Photography.Infrastructure.Persistence.Repositories;

public sealed class AlbumQueryRepository : IAlbumQueryRepository
{
    private readonly AppDbContext _db;
    public AlbumQueryRepository(AppDbContext db) => _db = db;

    public async Task<IReadOnlyList<Album>> ListAsync(bool publicOnly, CancellationToken ct = default)
    {
        var query = _db.Albums.AsNoTracking().Include(a => a.Images);
        var filtered = publicOnly
            ? query.Where(a => a.ShowInPortfolio || a.ShowInHome || a.ShowInStories)
            : query;
        return await filtered
            .OrderByDescending(a => a.EventDate ?? a.CreatedAtUtc)
            .ToListAsync(ct);
    }

    public Task<Album?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        _db.Albums.AsNoTracking().Include(a => a.Images).FirstOrDefaultAsync(a => a.Id == id, ct);

    public Task<Album?> GetBySlugAsync(string slug, CancellationToken ct = default) =>
        _db.Albums.AsNoTracking().Include(a => a.Images).FirstOrDefaultAsync(a => a.Slug == slug, ct);

    public Task<int> CountAsync(CancellationToken ct = default) => _db.Albums.CountAsync(ct);

    public Task<bool> AnyInCategoryAsync(int categoryId, CancellationToken ct = default) =>
        _db.Albums.AsNoTracking().AnyAsync(a => a.CategoryId == categoryId, ct);
}

public sealed class AlbumCommandRepository : IAlbumCommandRepository
{
    private readonly AppDbContext _db;
    public AlbumCommandRepository(AppDbContext db) => _db = db;

    public Task<Album?> GetForUpdateAsync(Guid id, CancellationToken ct = default) =>
        _db.Albums.Include(a => a.Images).FirstOrDefaultAsync(a => a.Id == id, ct);

    public async Task AddAsync(Album album, CancellationToken ct = default) =>
        await _db.Albums.AddAsync(album, ct);

    public void Remove(Album album) => _db.Albums.Remove(album);

    public Task<bool> ExistsAsync(Guid id, CancellationToken ct = default) =>
        _db.Albums.AnyAsync(a => a.Id == id, ct);

    public Task<bool> SlugExistsAsync(string slug, Guid? excludingAlbumId = null, CancellationToken ct = default) =>
        _db.Albums.AnyAsync(a => a.Slug == slug && (!excludingAlbumId.HasValue || a.Id != excludingAlbumId.Value), ct);

    public Task SaveChangesAsync(CancellationToken ct = default) => _db.SaveChangesAsync(ct);
}
