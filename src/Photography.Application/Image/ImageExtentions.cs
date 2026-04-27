using System.IO;
using System.Web.Hosting;

namespace Photography.Image
{
    public static class ImageExtentions
    {
        public static string RelaticeImagesFolder => Path.Combine("Images", "Albums", "Images");
        public static string AbsoluteImagesFolder => Path.Combine(HostingEnvironment.ApplicationPhysicalPath, RelaticeImagesFolder);
    }
}