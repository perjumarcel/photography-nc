using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Abp.Application.Services.Dto;
using Photography.Category;
using Photography.Image;
using Photography.Readonly;
using Photography.Web.Models.Portfolio;
using IImageAppService = Photography.Readonly.IImageAppService;

namespace Photography.Web.Controllers
{
    public class PortfolioController : PhotographyControllerBase
    {
        private readonly IAlbumAppService _albumAppService;
        private readonly ICategoryAppService _categoryAppService;
        private readonly IImageAppService _imageAppService;

        public PortfolioController(IAlbumAppService albumAppService, ICategoryAppService categoryAppService, IImageAppService imageAppService)
        {
            _albumAppService = albumAppService;
            _categoryAppService = categoryAppService;
            _imageAppService = imageAppService;
        }
        public async Task<ActionResult> Index()
        {
            var albums = await _albumAppService.GetAllPortfoliosAsync();
            var categories = await _categoryAppService.GetAll(new PagedResultRequestDto() { MaxResultCount = int.MaxValue });

            return View(new PortfolioViewModel
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
        
        public async Task<ActionResult> Previous(Guid albumId)
        {
            var album = await _albumAppService.GetPrevious(albumId);
            var images = await _imageAppService.GetAll(album.Id);

            return View("Details", new AlbumViewModel
                        {
                            Album = album,
                            Images = images
                        });
        }

        public async Task<ActionResult> Next(Guid albumId)
        {
            var album = await _albumAppService.GetNext(albumId);
            var images = await _imageAppService.GetAll(album.Id);

            return View("Details", new AlbumViewModel
                        {
                            Album = album,
                            Images = images
                        });
        }
    }
}
