using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Photography.Category.Dto;

namespace Photography.Category
{
    public class CategoryAppService : AsyncCrudAppService<Entities.Category, CategoryDto, int, PagedResultRequestDto, CreateCategoryDto, CategoryDto>,
                                      ICategoryAppService
    {
        public CategoryAppService(IRepository<Entities.Category> repository) : base(repository)
        {
        }
    }
}
