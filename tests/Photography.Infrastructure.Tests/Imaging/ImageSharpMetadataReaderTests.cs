using Photography.Application.Common.Imaging;
using Photography.Infrastructure.Imaging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using Xunit;

namespace Photography.Infrastructure.Tests.Imaging;

public class ImageSharpMetadataReaderTests
{
    [Fact]
    public async Task Reads_Width_And_Height_From_Png_Stream()
    {
        await using var ms = new MemoryStream();
        using (var image = new Image<Rgba32>(64, 32))
        {
            await image.SaveAsync(ms, new PngEncoder());
        }
        ms.Position = 0;

        IImageMetadataReader reader = new ImageSharpMetadataReader();
        var dims = await reader.ReadAsync(ms);

        Assert.NotNull(dims);
        Assert.Equal(64, dims!.Width);
        Assert.Equal(32, dims.Height);
        Assert.Equal(0, ms.Position); // stream rewound for the uploader
    }

    [Fact]
    public async Task Returns_Null_For_NonImage_Bytes()
    {
        await using var ms = new MemoryStream("not an image"u8.ToArray());
        IImageMetadataReader reader = new ImageSharpMetadataReader();
        var dims = await reader.ReadAsync(ms);
        Assert.Null(dims);
    }
}
