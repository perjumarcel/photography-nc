using System;
using System.Collections.Generic;
using Photography.Album.Dto;
using Photography.Image.Dto;

namespace Photography.Web.Admin.Models.Images
{
    public class ImageListViewModel
    {
        public IReadOnlyList<ImageDto> Images { get; set; }

        public IEnumerable<AlbumDdDto> Albums { get; set; }
        public Guid AlbumId { get; set; }
    }


}
