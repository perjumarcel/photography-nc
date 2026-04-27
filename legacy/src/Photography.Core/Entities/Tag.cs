using System;
using System.ComponentModel.DataAnnotations;
using Abp.Domain.Entities;

namespace Photography.Entities
{
    public class Tag : Entity
    {
        public const int MaxNameLength = 20;

        [Required]
        [StringLength(MaxNameLength)]
        public virtual string Name { get; set; }
    }
}