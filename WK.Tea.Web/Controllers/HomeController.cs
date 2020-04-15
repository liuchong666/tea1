using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WK.Tea.Web.Auth;

namespace WK.Tea.Web.Controllers
{
    public class HomeController : Controller
    {
        [TeaAuthorize(Roles = "Weixin")]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Layout()
        {
            return View();
        }

        public ActionResult Default()
        {
            return View();
        }
    }
}