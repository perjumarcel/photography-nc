using System;
using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;

namespace Photography.Album.Dto
{
    [AutoMapFrom(typeof(Entities.Album))]
    public class AlbumDto : EntityDto<Guid>
    {
        public const int MaxTitleLength = 64;

        [Required]
        [StringLength(MaxTitleLength)]
        public string Title { get; set; }

        public string Description { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? EventDate { get; set; }
        public string Client { get; set; }
        public string Location { get; set; }

        public bool ShowInPortfolio { get; set; }
        public bool ShowInStories { get; set; }
        public bool ShowInHome { get; set; }

        [Required]
        public int CategoryId { get; set; }
        public string Category { get; set; }
    }
}
