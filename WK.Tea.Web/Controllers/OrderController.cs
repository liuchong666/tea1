using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WK.Tea.DataProvider.DAL;
using WK.Tea.DataProvider.IDAL;

namespace WK.Tea.Web.Controllers
{
    public class OrderController : Controller
    {
        // GET: Order
        public ActionResult Settlement(int id)
        {
            using (IT_Order repository = new T_OrderRepository())
            {
                var model = repository.GetVOrderByID(id);
                return View(model);
            }
        }

        public ActionResult XuDing()
        {
            return View();

        }
    }
}