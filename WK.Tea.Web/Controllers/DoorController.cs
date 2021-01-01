using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WK.Tea.DataModel.SqlModel;
using WK.Tea.DataProvider.DAL;
using WK.Tea.DataProvider.IDAL;
using WK.Tea.Lock.ApiRequest;
using WK.Tea.Lock.ApiRequest.EEUN;
using WK.Tea.Web.Common;

namespace WK.Tea.Web.Controllers
{
    public class DoorController : Controller
    {
        // GET: Door
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 获取房卡示例
        /// </summary>
        public ActionResult Qrcode(int orderId)
        {
            using (IT_Order repository = new T_OrderRepository())
            {
                T_Order order = repository.FindFirstOrDefault(o => o.ID == orderId);
                if (order == null)
                {
                    ViewBag.Message = "订单不存在";
                }
                else
                {
                    T_Shop shop = null;
                    using (IT_Shop repositoryShop = new T_ShopRepository())
                    {
                        shop = repositoryShop.FindFirstOrDefault(o => o.ID == order.ShopID);
                    }
                    if (shop != null && shop.LockType == 1)
                    {
                        Dictionary<string, string> sortedParams = new Dictionary<string, string>()
                            {
                                { "APPID","6005BAFEA0C54011B6602D7A70C36E6C"},
                                { "AT",WebApiHelper.CreateInstance().GetTimeStamp()},
                                { "NONCESTR",WebApiHelper.CreateInstance().GetRandom()},
                                { "USERID","13311237111"},
                                { "TOKEN",WebApiHelper.CreateInstance().GetToken()},
                                { "KEYLOCKID",shop.LockID},
                                { "VALIDMINUTE","30"}
                            };

                        DateTime nowTime1 = DateTime.Now;
                        if (nowTime1 > order.BTime.AddMinutes(-15) && nowTime1 < order.ETime.AddMinutes(10))
                        {
                            sortedParams.Add("STARTDATE", order.BTime.AddMinutes(-15).ToString("yyyyMMddHHmmss"));
                            sortedParams.Add("ENDDATE", order.ETime.AddMinutes(10).ToString("yyyyMMddHHmmss"));
                            var sign = WebApiHelper.CreateInstance().GetSignature(sortedParams);
                            sortedParams.Add("SIGN", sign);

                            var result = WebApiHelper.CreateInstance().Get("https://yylock.eeun.cn/dms/app/getLockQRCode", sortedParams);
                            var qcode = JsonConvert.DeserializeObject<QCodeRespinse>(result).data;

                            ViewBag.qcode = "data:image/png;base64,"+qcode;
                            ViewBag.pwd = order.LockPW;
                            LogWriter.Default.WriteWarning(string.Format("uclbrt lock url: {0}", ViewBag.qcode));
                        }
                        else if (nowTime1 <= order.BTime.AddMinutes(-15))
                        {
                            ViewBag.Message = string.Format("您的预定的时间为：\n{0} - {1}\n门锁二维码可以提前15分钟获取！"
                                , order.BTime.ToString("MM/dd HH:mm"), order.ETime.ToString("MM/dd HH:mm"));
                        }
                        else
                        {
                            ViewBag.Message = string.Format("预定已过期！");
                        }

                        return View();
                    }


                    DateTime nowTime = DateTime.Now;
                    if (nowTime > order.BTime.AddMinutes(-15) && nowTime < order.ETime.AddMinutes(10))
                    {
                        string mobile = string.IsNullOrWhiteSpace(order.Mobile) ? LockApiHelper.Mobile : order.Mobile;
                        string data = LockApiHelper.WebApi.GetCardDataParams("1316882760",
                            mobile, "86", order.CardNo);
                        ViewBag.Url = "http://cz.uclbrt.com/apiLogin?data=" + data;
                        LogWriter.Default.WriteWarning(string.Format("uclbrt lock url: {0}", ViewBag.Url));
                        return Redirect(ViewBag.Url);
                    }
                    else if (nowTime <= order.BTime.AddMinutes(-15))
                    {
                        ViewBag.Message = string.Format("您的预定的时间为：\n{0} - {1}\n门锁二维码可以提前15分钟获取！"
                            , order.BTime.ToString("MM/dd HH:mm"), order.ETime.ToString("MM/dd HH:mm"));
                    }
                    else
                    {
                        ViewBag.Message = string.Format("预定已过期！");
                    }
                }
            }

            return View();
        }
    }
}