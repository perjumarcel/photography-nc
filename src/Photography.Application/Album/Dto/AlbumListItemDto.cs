using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Photography.Image;
using Photography.Image.Dto;

namespace Photography.Album.Dto
{
    [AutoMapFrom(typeof(Entities.Album))]
    public class AlbumListItemDto : EntityDto<Guid>
    {
        public string Title { get; set; }
        public string Category { get; set; }
        public ImageDto Cover { get; set; }

        public static string DefaultDomain = /*"~/"; */ @"http://covercenco.com/";
        public static string DefaultCoverUrl = DefaultDomain + ImageExtentions.RelaticeImagesFolder.Replace(@"\", @"/") + "/cover.jpg";

        [NotMapped]
        public string CoverUrl
        {
            get
            {
                if (Cover == null)
                {
                    return DefaultCoverUrl;
                }

                return DefaultDomain + ImageExtentions.RelaticeImagesFolder.Replace(@"\", @"/") + "/" + Id + "/" + Cover.Id + Path.GetExtension(Cover.OriginalName);
            }
        }
    }
}
