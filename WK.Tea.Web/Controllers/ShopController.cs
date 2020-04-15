using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WK.Tea.DataProvider.DAL;
using WK.Tea.DataProvider.IDAL;

namespace WK.Tea.Web.Controllers
{
    public class ShopController : Controller
    {
        // GET: Shop
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Detail(int id)
        {
            using (IT_Shop repository = new T_ShopRepository())
            {
                var model = repository.GetVShopByID(id);
                return View(model);
            }
        }
    }
}