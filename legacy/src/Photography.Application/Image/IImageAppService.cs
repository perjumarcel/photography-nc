using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Photography.Image.Dto;

namespace Photography.Image
{
    public interface IImageAppService : IAsyncCrudAppService<ImageDto, Guid, ImagesPagedResultRequestDto, CreateImageDto, ImageDto>
    {
        Task SetImageAsCover(Guid id);
    }
}