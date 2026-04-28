using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Abp.Application.Services.Dto;
using Abp.Web.Mvc.Authorization;
using Photography.Album;
using Photography.Category;
using Photography.Web.Admin.Models.Albums;
using Photography.Web.Controllers;

namespace Photography.Web.Admin.Controllers
{
    [AbpMvcAuthorize]
    public class AlbumsController : PhotographyControllerBase
    {
        private readonly IAlbumAppService _albumAppService;
        private readonly ICategoryAppService _categoryAppService;

        public AlbumsController(IAlbumAppService albumAppService, ICategoryAppService categoryAppService)
        {
            _albumAppService = albumAppService;
            _categoryAppService = categoryAppService;
        }

        public async Task<ActionResult> Index()
        {

            var albums = await _albumAppService.GetAll(new PagedResultRequestDto { MaxResultCount = int.MaxValue }); // Paging not implemented yet
            var categories = (await _categoryAppService.GetAll(new PagedResultRequestDto { MaxResultCount = int.MaxValue }));
            var model = new AlbumListViewModel
                        {
                            Albums = albums.Items,
                            Categories = categories.Items.Select(p => new SelectListItem
                            {
                            Value = p.Id.ToString(),
                            Text = p.Title
                        }).ToList()
            };

            return View(model);
        }

        public async Task<ActionResult> EditAlbumModal(Guid id)
        {
            var dto = await _albumAppService.Get(new EntityDto<Guid>(id));
            var categories = (await _categoryAppService.GetAll(new PagedResultRequestDto { MaxResultCount = int.MaxValue }));
            var model = new EditAlbumModalViewModel
            {
                            Album = dto,
                            Categories = categories.Items.Select(p => new SelectListItem
                                                                      {
                                                                          Value = p.Id.ToString(),
                                                                          Text = p.Title,
                                                                          Selected = dto.CategoryId == p.Id
                                                                      }).ToList()
                        };
            return View("_EditAlbumModal", model);
        }
    }
}
