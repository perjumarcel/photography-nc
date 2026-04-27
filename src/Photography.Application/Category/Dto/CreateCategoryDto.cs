using System;
using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;

namespace Photography.Category.Dto
{
    [AutoMapFrom(typeof(Entities.Category))]
    public class CreateCategoryDto 
    {
        public const int MaxTitleLength = 64;

        [Required]
        [StringLength(MaxTitleLength)]
        public string Title { get; set; }
        public bool ShowInFilter { get; set; }
    }
}
