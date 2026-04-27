using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Abp.Domain.Entities;

namespace Photography.Entities
{
    public class Category : Entity
    {
        public const int MaxTitleLength = 64;

        [Required]
        [StringLength(MaxTitleLength)]
        public virtual string Title { get; set; }

        [DefaultValue(true)]
        public virtual bool ShowAsFilter { get; set; }
    }
}