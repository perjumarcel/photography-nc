using System.Web.Mvc;

namespace Photography.Web.Controllers
{
    public class ContactController : PhotographyControllerBase
    {
        public ActionResult Index()
        {
            return View();
        }
	}
}
