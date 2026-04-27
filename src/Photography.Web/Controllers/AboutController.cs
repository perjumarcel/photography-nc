using System.Web.Mvc;

namespace Photography.Web.Controllers
{
    public class AboutController : PhotographyControllerBase
    {
        public ActionResult Index()
        {
            return View();
        }
	}
}
