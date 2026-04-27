using System;

namespace Photography.Web.Admin.Models.FineUploader
{
    public class FineUploaderViewModel
    {
        public Guid AlbumId { get; set; }
        public bool Multiple { get; set; }
        public bool UseOrientationValidation { get; set; }
        public string Action { get; set; }
    }
}