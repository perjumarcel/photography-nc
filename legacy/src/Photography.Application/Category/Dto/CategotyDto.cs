using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;

namespace Photography.Category.Dto
{
    [AutoMapFrom(typeof(Entities.Category))]
    public class CategoryDto : EntityDto
    {
        public const int MaxTitleLength = 64;

        [Required]
        [StringLength(MaxTitleLength)]
        public string Title { get; set; }
        public bool ShowAsFilter { get; set; }

    }
}
