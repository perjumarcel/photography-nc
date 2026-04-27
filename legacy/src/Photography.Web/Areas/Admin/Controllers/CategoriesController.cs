using System.Threading.Tasks;
using System.Web.Mvc;
using Abp.Application.Services.Dto;
using Abp.Web.Mvc.Authorization;
using Photography.Category;
using Photography.Web.Controllers;

namespace Photography.Web.Admin.Controllers
{
     [AbpMvcAuthorize]
    public class CategoriesController : PhotographyControllerBase
    {
        private readonly ICategoryAppService _categoryAppService;

        public CategoriesController(ICategoryAppService categoryAppService)
        {
            _categoryAppService = categoryAppService;
        }

        public async Task<ActionResult> Index()
        {
            var output = await _categoryAppService.GetAll(new PagedResultRequestDto { MaxResultCount = int.MaxValue }); // Paging not implemented yet
            return View(output);
        }

        public async Task<ActionResult> EditCategoryModal(int id)
        {
            var categoryDto = await _categoryAppService.Get(new EntityDto(id));
            return View("_EditCategoryModal", categoryDto);
        }
    }
}
