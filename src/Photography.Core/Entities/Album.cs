using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;

namespace Photography.Entities
{
    public class Album : CreationAuditedEntity<Guid>
    {
        public const int MaxTitleLength = 64;

        [Required]
        [StringLength(MaxTitleLength)]
        public virtual string Title { get; set; }

        public virtual string Description { get; set; }

        public virtual DateTime? EventDate { get; set; }
        public virtual string Client { get; set; }
        public virtual string Location { get; set; }

        public virtual bool ShowInPortfolio { get; set; }
        public virtual bool ShowInStories { get; set; }
        public virtual bool ShowInHome { get; set; }

        [Required]
        public virtual int CategoryId { get; set; }
        public virtual Category Category { get; set; }

        [ForeignKey("AlbumId")]
        public virtual ICollection<Image> Images { get; set; }
    }
}
