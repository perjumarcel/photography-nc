using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Photography.Category.Dto;

namespace Photography.Category
{
    public interface ICategoryAppService : IAsyncCrudAppService<CategoryDto, int, PagedResultRequestDto, CreateCategoryDto, CategoryDto>
    {
    }
}