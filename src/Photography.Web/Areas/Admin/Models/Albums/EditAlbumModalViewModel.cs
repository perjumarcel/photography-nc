using System.Collections.Generic;
using System.Web.Mvc;
using Photography.Album.Dto;

namespace Photography.Web.Admin.Models.Albums
{
    public class EditAlbumModalViewModel
    {
        public AlbumDto Album { get; set; }

        public IEnumerable<SelectListItem> Categories { get; set; }
    }
}