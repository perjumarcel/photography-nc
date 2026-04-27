using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services;
using Photography.Album.Dto;

namespace Photography.Readonly
{
    public interface IAlbumAppService : IApplicationService
    {
        Task<AlbumDto> Get(Guid id);
        Task<AlbumDto> GetNext(Guid albumId);
        Task<AlbumDto> GetPrevious(Guid albumId);
        Task<IEnumerable<AlbumListItemDto>> GetAllPortfoliosAsync();
        Task<IEnumerable<AlbumListItemDto>> GetAllHomeAlbumsAsync();
        Task<IEnumerable<AlbumListItemDto>> GetAllStoriesAlbumsAsync();
    }
}