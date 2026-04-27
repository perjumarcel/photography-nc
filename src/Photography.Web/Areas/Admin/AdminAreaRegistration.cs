using System.Web.Mvc;

namespace Photography.Web.Admin
{
    public class AdminAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "admin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "admin_default",
                "admin/{controller}/{action}/{id}",
                new { controller="Albums", action = "Index", id = UrlParameter.Optional },
                new[] { "Photography.Web.Admin.Controllers" }
            );
        }
    }
}