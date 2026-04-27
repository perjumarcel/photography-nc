using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;

namespace Photography.Album.Dto
{
    [AutoMapFrom(typeof(Entities.Album))]
    public class AlbumDdDto : EntityDto<Guid>
    {
        public string Title { get; set; }
        public string Category { get; set; }
    }
}
