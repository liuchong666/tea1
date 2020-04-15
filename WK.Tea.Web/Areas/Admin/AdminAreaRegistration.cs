using System.Web.Mvc;

namespace WK.Tea.Web.Areas.Admin
{
    public class AdminAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Admin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                name: "Admin_home",
                url: "Admin/Default",
                defaults: new { controller = "Home", action = "Default" },
                namespaces: new string[] { "WK.Tea.Web.Areas.Admin.Controllers" }
            );
            context.MapRoute(
                name: "Admin_layout",
                url: "Admin/Layout",
                defaults: new { controller = "Home", action = "Layout" },
                namespaces: new string[] { "WK.Tea.Web.Areas.Admin.Controllers" }
            );
            context.MapRoute(
                "Admin_default",
                "Admin/{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new string[] { "WK.Tea.Web.Areas.Admin.Controllers" }
            );
        }
    }
}