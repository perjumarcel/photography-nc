using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Photography.Album.Dto;

namespace Photography.Album
{
    public interface IAlbumAppService : IAsyncCrudAppService<AlbumDto, Guid, PagedResultRequestDto, CreateAlbumDto, AlbumDto>
    {
        Task<IEnumerable<AlbumDdDto>> GetAllKeyValuesAsync();
    }
}