namespace Photography.Core.Albums;

/// <summary>
/// Read-only album access. Implementations must use AsNoTracking + projections.
/// </summary>
public interface IAlbumQueryRepository
{
    Task<IReadOnlyList<Album>> ListAsync(bool publicOnly, CancellationToken ct = default);
    Task<Album?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<int> CountAsync(CancellationToken ct = default);
}

/// <summary>
/// Write-side album access. Loads full tracked aggregates including the Images collection.
/// </summary>
public interface IAlbumCommandRepository
{
    Task<Album?> GetForUpdateAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(Album album, CancellationToken ct = default);
    void Remove(Album album);
    Task<bool> ExistsAsync(Guid id, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
