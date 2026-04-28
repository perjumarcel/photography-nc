using System.Collections.Generic;
using System.Web.Mvc;
using Photography.Album.Dto;

namespace Photography.Web.Admin.Models.Albums
{
    public class AlbumListViewModel
    {
        public IReadOnlyList<AlbumDto> Albums { get; set; }

        public List<SelectListItem> Categories { get; set; }
    }
}