using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Abp.Application.Services.Dto;
using Photography.Category;
using Photography.Readonly;
using Photography.Web.Models.Portfolio;
using Photography.Web.Models.Stories;
using IImageAppService = Photography.Readonly.IImageAppService;

namespace Photography.Web.Controllers
{
    public class StoriesController : PhotographyControllerBase
    {
        private readonly IAlbumAppService _albumAppService;
        private readonly ICategoryAppService _categoryAppService;
        private readonly IImageAppService _imageAppService;

        public StoriesController(IAlbumAppService albumAppService, ICategoryAppService categoryAppService, IImageAppService imageAppService)
        {
            _albumAppService = albumAppService;
            _categoryAppService = categoryAppService;
            _imageAppService = imageAppService;
        }
        public async Task<ActionResult> Index()
        {
            var albums = await _albumAppService.GetAllStoriesAlbumsAsync();
            var categories = await _categoryAppService.GetAll(new PagedResultRequestDto() { MaxResultCount = int.MaxValue });

            return View(new StoriesViewModel
            {
                Albums = albums,
                Categories = categories.Items.Where(x => x.ShowAsFilter)
            });
        }

        public async Task<ActionResult> Details(Guid id)
        {
            var album = await _albumAppService.Get(id);
            var images = await _imageAppService.GetAll(id);

            return View(new AlbumViewModel
            {
                Album = album,
                Images = images
            });
        }
    }
}
