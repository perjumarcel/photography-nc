using Photography.Application.Common.Imaging;
using SixLabors.ImageSharp;

namespace Photography.Infrastructure.Imaging;

/// <summary>
/// SixLabors.ImageSharp implementation of <see cref="IImageMetadataReader"/>. Uses the
/// streaming <c>Image.IdentifyAsync</c> API which decodes only image headers — orders
/// of magnitude cheaper than fully loading the pixel buffer.
/// </summary>
public sealed class ImageSharpMetadataReader : IImageMetadataReader
{
    public async Task<ImageDimensions?> ReadAsync(Stream stream, CancellationToken ct = default)
    {
        if (!stream.CanSeek)
            throw new ArgumentException("A seekable stream is required so the upload pipeline can re-read the bytes.", nameof(stream));

        var origin = stream.Position;
        try
        {
            var info = await Image.IdentifyAsync(stream, ct);
            return info is null ? null : new ImageDimensions(info.Width, info.Height);
        }
        catch (UnknownImageFormatException)
        {
            return null;
        }
        catch (InvalidImageContentException)
        {
            return null;
        }
        finally
        {
            stream.Position = origin;
        }
    }
}
