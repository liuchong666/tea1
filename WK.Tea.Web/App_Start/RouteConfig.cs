using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace WK.Tea.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Home",
                url: "Default",
                defaults: new { controller = "Home", action = "Default" },
                namespaces: new string[] { "WK.Tea.Web.Controllers" }
            );
            routes.MapRoute(
                name: "Layout",
                url: "Layout",
                defaults: new { controller = "Home", action = "Layout" },
                namespaces: new string[] { "WK.Tea.Web.Controllers" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new string[] { "WK.Tea.Web.Controllers" }
            );
        }
    }
}
