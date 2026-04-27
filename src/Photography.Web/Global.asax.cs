using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using Abp.Castle.Logging.Log4Net;
using Abp.Web;
using Castle.Facilities.Logging;
using ImageProcessor.Web.Helpers;
using ImageProcessor.Web.HttpModules;

namespace Photography.Web
{
    public class MvcApplication : AbpWebApplication<PhotographyWebModule>
    {
        protected override void Application_Start(object sender, EventArgs e)
        {
            AbpBootstrapper.IocManager.IocContainer.AddFacility<LoggingFacility>(f => f.UseAbpLog4Net().WithConfig(Server.MapPath("log4net.config")));
            ImageProcessingModule.ValidatingRequest += ImageProcessingModuleOnValidatingRequest;

            base.Application_Start(sender, e);
        }

        private static void ImageProcessingModuleOnValidatingRequest(object sender, ValidatingRequestEventArgs args)
        {
            if (string.IsNullOrWhiteSpace(args.QueryString))
            {
                return;
            }

            NameValueCollection queryCollection = HttpUtility.ParseQueryString(args.QueryString);

            string[] allowed = {"width", "height"};

            IEnumerable<string> parameters = queryCollection.AllKeys.Intersect(allowed, StringComparer.OrdinalIgnoreCase);
            foreach (string parameter in parameters)
            {
                string value = queryCollection.Get(parameter);
                queryCollection.Set(parameter, GetResizedValue(value));
            }

            args.QueryString = queryCollection.ToString();
        }

        private static string GetResizedValue(string str)
        {
            int value;
            if (!int.TryParse(str, out value))
            {
                return str;
            }

            if (value > 0 && value <= 100)
            {
                return "100";
            }

            if (value > 100 && value <= 300)
            {
                return "300";
            }

            if (value > 300 && value <= 500)
            {
                return "500";
            }

            if (value > 500 && value <= 750)
            {
                return "750";
            }

            if (value > 750 && value <= 1000)
            {
                return "1000";
            }

            if (value > 1000 && value <= 1500)
            {
                return "1500";
            }

            return value > 1500 ? "2500" : str;
        }
    }
}