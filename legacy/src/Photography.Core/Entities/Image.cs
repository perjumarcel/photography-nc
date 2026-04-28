using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Abp.Domain.Entities.Auditing;

namespace Photography.Entities
{
    public class Image : CreationAuditedEntity<Guid>
    {
        public const int MaxNameLength = 256;

        [Required]
        [MaxLength(MaxNameLength)]
        public virtual string OriginalName { get; set; }
        public virtual int Width { get; set; }
        public virtual int Height { get; set; }

        [Required]
        public virtual ImageOrientation ImageOrientation { get; set; }   
        [Required]
        [DefaultValue(ImageType.Default)]
        public virtual ImageType ImageType { get; set; }   

        public virtual Album Album { get; set; }
        public Guid AlbumId { get; set; }
    }

    public enum ImageOrientation
    {
        Vertical,
        Horizontal,
    }

    public enum ImageType
    {
        Default,
        Cover
    }
}