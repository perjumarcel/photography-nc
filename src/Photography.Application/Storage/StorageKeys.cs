namespace Photography.Application.Storage;

public static class StorageKeys
{
    /// <summary>
    /// Format: <c>albums/{albumId}/images/{imageId}{extension}</c>.
    /// Deterministic so the production migration can recompute the key from legacy data.
    /// </summary>
    public static string ImageKey(Guid albumId, Guid imageId, string originalName)
    {
        var ext = Path.GetExtension(originalName);
        return $"albums/{albumId:D}/images/{imageId:D}{ext.ToLowerInvariant()}";
    }
}
