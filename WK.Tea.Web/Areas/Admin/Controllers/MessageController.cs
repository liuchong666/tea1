using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WK.Tea.DataModel.SqlModel;
using WK.Tea.DataProvider.DAL;
using WK.Tea.DataProvider.IDAL;
using WK.Tea.Web.Common;

namespace WK.Tea.Web.Areas.Admin.Controllers
{
    public class MessageController : Controller
    {
        // GET: Admin/Message
        public ActionResult Index(int orderId)
        {
            T_Order order = null;
            using (IT_Order repository=new T_OrderRepository())
            {
                order=repository.FindFirstOrDefault(o => o.ID == orderId);
            }

            if (order != null)
            {
                T_Shop shop = null;
                using (IT_Shop repository = new T_ShopRepository())
                {
                    shop = repository.FindFirstOrDefault(o => o.ID == order.ShopID);
                }
                if (shop != null)
                    WeixinTempMsg.SendCleanMsg(shop.ShopAddress, order.OrderNo, order.BTime, order.ETime,shop.CleanerOpenID);
                return Json("发送成功", JsonRequestBehavior.AllowGet);
            }

            return Json("发送失败",JsonRequestBehavior.AllowGet);
        }
    }
}