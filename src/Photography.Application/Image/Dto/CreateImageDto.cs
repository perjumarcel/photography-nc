using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using Abp.AutoMapper;
using Photography.Entities;

namespace Photography.Image.Dto
{
    [AutoMapFrom(typeof(Entities.Image))]
    public class CreateImageDto 
    {
        public const int MaxNameLength = 256;
        public const int MaxRatioLength = 256;

        public virtual int Width { get; set; }
        public virtual int Height { get; set; }

        [Required]
        [MaxLength(MaxNameLength)]
        public string OriginalName { get; set; }

        public ImageOrientation ImageOrientation { get; set; }

        public Guid AlbumId { get; set; }

        [NotMapped] 
        public virtual Stream File { get; set; }

    }
}
