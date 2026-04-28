using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services;
using Photography.Album.Dto;
using Photography.Image.Dto;

namespace Photography.Readonly
{
    public interface IImageAppService : IApplicationService
    {
        Task<IEnumerable<ImageDto>> GetAll(Guid albumId);
    }
}
