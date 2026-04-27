using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Photography.Web.Helpers
{
    public static class UrlHelpers
    {
        public static string Content(this UrlHelper urlHelper, string contentPath, bool toAbsolute = false)
        {
            var path = urlHelper.Content(contentPath);
            var url = new Uri(HttpContext.Current.Request.Url, path);

            return toAbsolute ? url.AbsoluteUri : path;
        }
    }
}