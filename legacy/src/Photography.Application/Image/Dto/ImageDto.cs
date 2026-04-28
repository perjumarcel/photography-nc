using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Photography.Entities;

namespace Photography.Image.Dto
{
    [AutoMapFrom(typeof(Entities.Image))]
    public class ImageDto : EntityDto<Guid>
    {
        public const int MaxNameLength = 256;

        [Required]
        [MaxLength(MaxNameLength)]
        public string OriginalName { get; set; }
        public virtual int Width { get; set; }
        public virtual int Height { get; set; }

        public ImageOrientation ImageOrientation { get; set; }
        public virtual ImageType ImageType { get; set; }   

        [NotMapped]
        public string RelativeUrl => "~/" + ImageExtentions.RelaticeImagesFolder.Replace(@"\", @"/") + "/" + AlbumId + "/" + Id + Path.GetExtension(OriginalName);

        public Guid AlbumId { get; set; }
    }
}
