using System.Collections.Generic;
using Photography.Album.Dto;
using Photography.Category.Dto;

namespace Photography.Web.Models.Stories
{
    public class StoriesViewModel
    {
        public IEnumerable<AlbumListItemDto> Albums { get; set; }
        public IEnumerable<CategoryDto> Categories { get; set; }
    }
}
