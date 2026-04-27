using System.Collections.Generic;
using Photography.Album.Dto;
using Photography.Image.Dto;

namespace Photography.Web.Models.Portfolio
{
    public class AlbumViewModel
    {
        public AlbumDto Album { get; set; }
        public IEnumerable<ImageDto> Images { get; set; }
    }
}
