using System.Threading.Tasks;
using System.Web.Mvc;
using Photography.Album.Dto;
using Photography.Readonly;
using Photography.Web.Models.Home;

namespace Photography.Web.Controllers
{
    public class HomeController : PhotographyControllerBase
    {
        private readonly IAlbumAppService _albumAppService;

        public HomeController(IAlbumAppService albumAppService)
        {
            _albumAppService = albumAppService;
        }
        public async Task<ActionResult> Index()
        {
            var albums = await _albumAppService.GetAllHomeAlbumsAsync();

            return View(new HomeViewModel
                        {
                            Albums = albums
                        });
        }
	}
}
