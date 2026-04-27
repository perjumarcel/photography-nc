using AutoMapper;

namespace Photography.Category.Dto
{
    public class CategoryMapProfile : Profile
    {
        public CategoryMapProfile()
        {
            CreateMap<CategoryDto, Entities.Category>();
            CreateMap<CreateCategoryDto, Entities.Category>();
        }
    }
}
