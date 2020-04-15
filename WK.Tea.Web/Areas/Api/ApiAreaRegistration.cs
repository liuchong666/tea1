using System.Web.Mvc;

namespace WK.Tea.Web.Areas.Api
{
    public class ApiAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Api";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.Routes.MapRoute(
                name: "DefaultApi",
                url: "api/{controller}/{id}",
                defaults: new { id = UrlParameter.Optional },
                namespaces: new string[] { "WK.Tea.Web.Areas.Api.Controllers" }
            );

            //context.MapRoute(
            //    "Api_default",
            //    "Api/{controller}/{action}/{id}",
            //    new { action = "Index", id = UrlParameter.Optional },
            //    namespaces: new string[] { "WK.Tea.Web.Areas.Api.Controllers" }
            //);
        }
    }
}