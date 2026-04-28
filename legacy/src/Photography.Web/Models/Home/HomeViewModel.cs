using System.Collections.Generic;
using Photography.Album.Dto;

namespace Photography.Web.Models.Home
{
    public class HomeViewModel
    {
        public IEnumerable<AlbumListItemDto> Albums { get; set; }
    }
}
