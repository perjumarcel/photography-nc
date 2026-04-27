using System;
using Abp.Application.Services.Dto;

namespace Photography.Image.Dto
{
    [Serializable]
    public class ImagesPagedResultRequestDto : PagedAndSortedResultRequestDto   
    {
        public virtual Guid AlbumId { get; set; }
    }
}
