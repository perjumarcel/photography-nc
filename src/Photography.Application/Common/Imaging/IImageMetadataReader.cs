namespace Photography.Application.Common.Imaging;

public sealed record ImageDimensions(int Width, int Height);

/// <summary>
/// Extracts pixel dimensions from an image stream without persisting the bytes.
/// Implementations live in the Infrastructure layer (using SixLabors.ImageSharp).
/// </summary>
public interface IImageMetadataReader
{
    /// <summary>
    /// Reads width/height from <paramref name="stream"/>. Returns <c>null</c> when the
    /// stream does not contain a recognisable image.
    /// The stream's position is restored to the original location on completion so the
    /// same stream can be re-read by the storage uploader.
    /// </summary>
    Task<ImageDimensions?> ReadAsync(Stream stream, CancellationToken ct = default);
}
