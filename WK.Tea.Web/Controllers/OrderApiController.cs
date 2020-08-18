using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web.Http;
using WK.Tea.DataModel;
using WK.Tea.DataModel.SqlModel;
using WK.Tea.DataProvider.DAL;
using WK.Tea.DataProvider.IDAL;
using WK.Tea.Web.Common;
using WK.Tea.Web.Models;

namespace WK.Tea.Web.Controllers
{
    [RoutePrefix("api/order")]
    public class OrderApiController : ApiController
    {
        [Route("admin/add")]
        public HttpResponseMessage AddOrderAdmin([FromBody] T_Order order)
        {
            ResultMsg resultMsg = new ResultMsg();
            try
            {
                order.Duration = (int)(order.ETime - order.BTime).TotalMinutes;
                if (order.BTime.Minute != 0 && order.BTime.Minute != 30)
                {
                    resultMsg.code = 1;
                    resultMsg.msg = "预定开始时间必须以半小时为间隔，如：12:00，12:30！";
                }
                else if (order.ETime.Minute != 0 && order.ETime.Minute != 30)
                {
                    resultMsg.code = 1;
                    resultMsg.msg = "预定结束时间必须以半小时为间隔，如：14:00，14:30！";
                }
                else if (order.Duration < 120 || order.Duration % 30 != 0)
                {
                    resultMsg.code = 1;
                    resultMsg.msg = "预定时长至少2小时，时长必须半小时为整";
                }
                else
                {
                    bool checkOrderTime = false;
                    using (IT_Order repository = new T_OrderRepository())
                    {
                        checkOrderTime = repository.CheckOrderTime(order);
                    }
                    if (checkOrderTime)
                    {
                        resultMsg.code = 1;
                        resultMsg.msg = "预定时间段被占用，请重新选择预定时间";
                    }
                    else
                    {
                        T_Shop shop = null;
                        using (IT_Shop repository = new T_ShopRepository())
                        {
                            shop = repository.FindFirstOrDefault(o => o.ID == order.ShopID);
                        }
                        if (shop != null)
                        {
                            WK.Tea.Lock.ApiRequest.CreateCardRequest postEntity = new WK.Tea.Lock.ApiRequest.CreateCardRequest
                            {
                                communityNo = "1316882760",
                                roomNo = shop.RoomNo,
                                floorNo = shop.FloorNo,
                                buildNo = shop.BuildNo,
                                startTime = order.BTime.AddMinutes(-15).ToString("yyMMddHHmm"),
                                endTime = order.ETime.AddMinutes(10).ToString("yyMMddHHmm"),
                                mobile = string.IsNullOrWhiteSpace(order.Mobile) ? WK.Tea.Lock.ApiRequest.LockApiHelper.Mobile : order.Mobile
                            };
                            WK.Tea.Lock.ApiRequest.CreateCardResponse result =
                                WK.Tea.Lock.ApiRequest.LockApiHelper.WebApi.Post<WK.Tea.Lock.ApiRequest.CreateCardRequest, WK.Tea.Lock.ApiRequest.CreateCardResponse>("https://api.uclbrt.com/?c=Qrcode&a=getLink", postEntity);
                            using (IT_Order repository = new T_OrderRepository())
                            {
                                order.CardNo = result.cardNo;
                                order.Flag = 0;
                                order.OP = User.Identity.Name;
                                order.CTime = DateTime.Now;
                                repository.Insert(order);
                                string url = "http://dc.orangenet.com.cn/Door/Qrcode?orderId=" + order.ID;
                                WeixinTempMsg.SendManagerOrderMsg(url, shop.ShopAddress, order.BTime, order.ETime, order.FeeCode.Value,order.OrderNo);
                                WeixinTempMsg.SendSMS(shop.ShopAddress, order.Mobile, shop.ShopPhoneNum, order.BTime, order.ETime, url);
                                //WeixinTempMsg.SendCleanMsg(shop.ShopAddress, order.OrderNo, order.BTime, order.ETime);
                            }
                        }
                        else
                        {
                            resultMsg.code = 1;
                            resultMsg.msg = "门店不存在";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                resultMsg.code = (int)StatusCodeEnum.Error;
                resultMsg.msg = ex.Message;
            }
            return resultMsg.toJson();
        }

        [Route("wx/add")]
        public HttpResponseMessage AddOrderWX([FromBody] T_Order order)
        {
            ResultMsg resultMsg = new ResultMsg();
            try
            {
                order.ETime = order.BTime.AddMinutes(order.Duration);
                if (order.BTime.Minute != 0 && order.BTime.Minute != 30)
                {
                    resultMsg.code = 1;
                    resultMsg.msg = "预定开始时间必须以半小时为间隔，如：12:00，12:30！";
                }
                else if (order.ETime.Minute != 0 && order.ETime.Minute != 30)
                {
                    resultMsg.code = 1;
                    resultMsg.msg = "预定结束时间必须以半小时为间隔，如：14:00，14:30！";
                }
                else if (order.Duration < 120 || order.Duration % 30 != 0)
                {
                    resultMsg.code = 1;
                    resultMsg.msg = "预定时长至少2小时，时长必须半小时为整";
                }
                else
                {
                    bool checkOrderTime = false;
                    using (IT_Order repository = new T_OrderRepository())
                    {
                        checkOrderTime = repository.CheckOrderTime(order);
                    }
                    if (checkOrderTime)
                    {
                        resultMsg.code = 1;
                        resultMsg.msg = "预定时间段被占用，请重新选择预定时间";
                    }
                    else
                    {
                        T_Shop shop = null;
                        using (IT_Shop repository = new T_ShopRepository())
                        {
                            shop = repository.FindFirstOrDefault(o => o.ID == order.ShopID);
                        }
                        if (shop != null)
                        {
                            using (IT_Order repository = new T_OrderRepository())
                            {
                                TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
                                order.OrderNo = "DCWX" + Convert.ToInt64(ts.TotalMilliseconds).ToString();
                                order.ReMarks = 1;
                                order.PayStatus = 0;
                                order.Flag = 0;
                                order.FeeCode = shop.OrderFee;
                                var t = order.Duration - 120;
                                if (t > 0)
                                {
                                    order.FeeCode += t / 30 * shop.ReOrderFee;
                                }
                                order.OpenID = User.Identity.Name;
                                order.CTime = DateTime.Now;
                                order = repository.Insert(order);
                                resultMsg.data = order;
                            }
                        }
                        else
                        {
                            resultMsg.code = 1;
                            resultMsg.msg = "门店不存在";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                resultMsg.code = (int)StatusCodeEnum.Error;
                resultMsg.msg = ex.Message;
            }
            return resultMsg.toJson();
        }

        [Route("wx/search")]
        public HttpResponseMessage SearchOrderWX([FromBody] T_Order order)
        {
            ResultMsg resultMsg = new ResultMsg();
            T_Order orderResult = null;
            using (IT_Order repository = new T_OrderRepository())
            {
                orderResult = repository.FindAll(c => c.Mobile == order.Mobile && c.PayStatus == 1).OrderByDescending(c => c.ID).FirstOrDefault();
            }

            if (orderResult == null)
            {
                resultMsg.code = 500;
                resultMsg.msg = "暂无订单信息";

                return resultMsg.toJson();
            }

            resultMsg.code = 200;
            resultMsg.data = orderResult;

            return resultMsg.toJson();
        }

        [Route("admin/renew")]
        public HttpResponseMessage RenewOrderAdmin([FromBody] T_Order order)
        {
            ResultMsg resultMsg = new ResultMsg();
            try
            {
                order.Duration = (int)(order.ETime - order.BTime).TotalMinutes;
                if (order.BTime.Minute != 0 && order.BTime.Minute != 30)
                {
                    resultMsg.code = 1;
                    resultMsg.msg = "预定开始时间必须以半小时为间隔，如：12:00，12:30！";
                }
                else if (order.ETime.Minute != 0 && order.ETime.Minute != 30)
                {
                    resultMsg.code = 1;
                    resultMsg.msg = "预定结束时间必须以半小时为间隔，如：14:00，14:30！";
                }
                else if (order.Duration < 30 || order.Duration % 30 != 0)
                {
                    resultMsg.code = 1;
                    resultMsg.msg = "预定时长至少半小时，时长必须半小时为整";
                }
                else
                {
                    bool checkOrderTime = false;
                    using (IT_Order repository = new T_OrderRepository())
                    {
                        checkOrderTime = repository.CheckOrderTime(order);
                    }
                    if (checkOrderTime)
                    {
                        resultMsg.code = 1;
                        resultMsg.msg = "预定时间段被占用，请重新选择预定时间";
                    }
                    else
                    {
                        T_Order porder = null;
                        using (IT_Order repository = new T_OrderRepository())
                        {
                            porder = repository.FindFirstOrDefault(o => o.OrderNo == order.ParentNo);

                        }
                        if (porder != null)
                        {
                            order.ShopID = porder.ShopID;
                            order.Mobile = porder.Mobile;
                            order.ReMarks = 0;
                            order.Flag = 1;

                            T_Shop shop = null;
                            using (IT_Shop repository = new T_ShopRepository())
                            {
                                shop = repository.FindFirstOrDefault(o => o.ID == order.ShopID);
                            }
                            if (shop != null)
                            {
                                WK.Tea.Lock.ApiRequest.CreateCardRequest postEntity = new WK.Tea.Lock.ApiRequest.CreateCardRequest
                                {
                                    communityNo = "1316882760",
                                    roomNo = shop.RoomNo,
                                    floorNo = shop.FloorNo,
                                    buildNo = shop.BuildNo,
                                    startTime = order.BTime.AddMinutes(-15).ToString("yyMMddHHmm"),
                                    endTime = order.ETime.AddMinutes(10).ToString("yyMMddHHmm"),
                                    mobile = string.IsNullOrWhiteSpace(order.Mobile) ? WK.Tea.Lock.ApiRequest.LockApiHelper.Mobile : order.Mobile
                                };
                                WK.Tea.Lock.ApiRequest.CreateCardResponse result =
                                    WK.Tea.Lock.ApiRequest.LockApiHelper.WebApi.Post<WK.Tea.Lock.ApiRequest.CreateCardRequest, WK.Tea.Lock.ApiRequest.CreateCardResponse>("https://api.uclbrt.com/?c=Qrcode&a=getLink", postEntity);
                                using (IT_Order repository = new T_OrderRepository())
                                {
                                    order.CardNo = result.cardNo;
                                    order.Flag = 0;
                                    order.OP = User.Identity.Name;
                                    order.CTime = DateTime.Now;
                                    repository.Insert(order);
                                    string url = "http://dc.orangenet.com.cn/Door/Qrcode?orderId=" + order.ID;
                                    WeixinTempMsg.SendManagerOrderMsg(url, shop.ShopAddress, order.BTime, order.ETime, order.FeeCode.Value, order.OrderNo,1);
                                    WeixinTempMsg.SendCleanMsg(shop.ShopAddress, order.OrderNo, order.BTime, order.ETime, shop.CleanerOpenID, 1);
                                    WeixinTempMsg.SendSMS(shop.ShopAddress, order.Mobile, shop.ShopPhoneNum, order.BTime, order.ETime, url);
                                }
                            }
                            else
                            {
                                resultMsg.code = 1;
                                resultMsg.msg = "门店不存在";
                            }
                        }
                        else
                        {
                            resultMsg.code = 1;
                            resultMsg.msg = "主订单不存在";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                resultMsg.code = (int)StatusCodeEnum.Error;
                resultMsg.msg = ex.Message;
            }
            return resultMsg.toJson();
        }

        [Route("wx/renew")]
        public HttpResponseMessage RenewOrderWX([FromBody] T_Order order)
        {
            ResultMsg resultMsg = new ResultMsg();
            try
            {
                order.Duration = (int)(order.ETime - order.BTime).TotalMinutes;
                if (order.BTime.Minute != 0 && order.BTime.Minute != 30)
                {
                    resultMsg.code = 1;
                    resultMsg.msg = "预定开始时间必须以半小时为间隔，如：12:00，12:30！";
                }
                else if (order.ETime.Minute != 0 && order.ETime.Minute != 30)
                {
                    resultMsg.code = 1;
                    resultMsg.msg = "预定结束时间必须以半小时为间隔，如：14:00，14:30！";
                }
                else if (order.Duration < 30 || order.Duration % 30 != 0)
                {
                    resultMsg.code = 1;
                    resultMsg.msg = "预定时长至少半小时，时长必须半小时为整";
                }
                else
                {
                    bool checkOrderTime = false;
                    using (IT_Order repository = new T_OrderRepository())
                    {
                        checkOrderTime = repository.CheckOrderTime(order);
                    }
                    if (checkOrderTime)
                    {
                        resultMsg.code = 1;
                        resultMsg.msg = "预定时间段被占用，请重新选择预定时间";
                    }
                    else
                    {
                        T_Order porder = null;
                        using (IT_Order repository = new T_OrderRepository())
                        {
                            porder = repository.FindFirstOrDefault(o => o.OrderNo == order.ParentNo);

                        }
                        if (porder != null)
                        {
                            order.ShopID = porder.ShopID;
                            order.Mobile = porder.Mobile;
                            order.ReMarks = 0;
                            order.Flag = 1;

                            T_Shop shop = null;
                            using (IT_Shop repository = new T_ShopRepository())
                            {
                                shop = repository.FindFirstOrDefault(o => o.ID == order.ShopID);
                            }
                            if (shop != null)
                            {
                                WK.Tea.Lock.ApiRequest.CreateCardRequest postEntity = new WK.Tea.Lock.ApiRequest.CreateCardRequest
                                {
                                    communityNo = "1316882760",
                                    roomNo = shop.RoomNo,
                                    floorNo = shop.FloorNo,
                                    buildNo = shop.BuildNo,
                                    startTime = order.BTime.AddMinutes(-15).ToString("yyMMddHHmm"),
                                    endTime = order.ETime.AddMinutes(10).ToString("yyMMddHHmm"),
                                    mobile = string.IsNullOrWhiteSpace(order.Mobile) ? WK.Tea.Lock.ApiRequest.LockApiHelper.Mobile : order.Mobile
                                };
                                WK.Tea.Lock.ApiRequest.CreateCardResponse result =
                                    WK.Tea.Lock.ApiRequest.LockApiHelper.WebApi.Post<WK.Tea.Lock.ApiRequest.CreateCardRequest, WK.Tea.Lock.ApiRequest.CreateCardResponse>("https://api.uclbrt.com/?c=Qrcode&a=getLink", postEntity);
                                using (IT_Order repository = new T_OrderRepository())
                                {
                                    order.CardNo = result.cardNo;
                                    order.Flag = 0;
                                    order.OP = User.Identity.Name;
                                    repository.Insert(order);
                                    string url = "http://dc.orangenet.com.cn/Door/Qrcode?orderId=" + order.ID;
                                    WeixinTempMsg.SendManagerOrderMsg(url, shop.ShopAddress, order.BTime, order.ETime, order.FeeCode.Value, order.OrderNo, 1);
                                    WeixinTempMsg.SendCleanMsg(shop.ShopAddress, order.OrderNo, order.BTime, order.ETime, shop.CleanerOpenID, 1);
                                    WeixinTempMsg.SendSMS(shop.ShopAddress, order.Mobile, shop.ShopPhoneNum, order.BTime, order.ETime, url);
                                }
                            }
                            else
                            {
                                resultMsg.code = 1;
                                resultMsg.msg = "门店不存在";
                            }
                        }
                        else
                        {
                            resultMsg.code = 1;
                            resultMsg.msg = "主订单不存在";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                resultMsg.code = (int)StatusCodeEnum.Error;
                resultMsg.msg = ex.Message;
            }
            return resultMsg.toJson();
        }

        [Route("admin/edit")]
        public HttpResponseMessage EditOrderAdmin([FromBody] T_Order order)
        {
            ResultMsg resultMsg = new ResultMsg();
            try
            {
                order.Duration = (int)(order.ETime - order.BTime).TotalMinutes;
                if (order.BTime.Minute != 0 && order.BTime.Minute != 30)
                {
                    resultMsg.code = 1;
                    resultMsg.msg = "预定开始时间必须以半小时为间隔，如：12:00，12:30！";
                }
                else if (order.ETime.Minute != 0 && order.ETime.Minute != 30)
                {
                    resultMsg.code = 1;
                    resultMsg.msg = "预定结束时间必须以半小时为间隔，如：14:00，14:30！";
                }
                else if (order.Duration % 30 != 0)
                {
                    resultMsg.code = 1;
                    resultMsg.msg = "时长必须半小时为整";
                }
                else
                {
                    bool checkOrderTime = false;
                    using (IT_Order repository = new T_OrderRepository())
                    {
                        checkOrderTime = repository.CheckOrderTime(order);
                    }
                    if (order.OrderNo.Substring(0, 1) != "R" && checkOrderTime)
                    {
                        resultMsg.code = 1;
                        resultMsg.msg = "预定时间段被占用，请重新选择预定时间";
                    }
                    else
                    {
                        T_Shop shop = null;
                        using (IT_Shop repository = new T_ShopRepository())
                        {
                            shop = repository.FindFirstOrDefault(o => o.ID == order.ShopID);
                        }
                        if (shop != null)
                        {
                            WK.Tea.Lock.ApiRequest.CreateCardRequest postEntity = new WK.Tea.Lock.ApiRequest.CreateCardRequest
                            {
                                communityNo = "1316882760",
                                roomNo = shop.RoomNo,
                                floorNo = shop.FloorNo,
                                buildNo = shop.BuildNo,
                                startTime = order.BTime.AddMinutes(-15).ToString("yyMMddHHmm"),
                                endTime = order.ETime.AddMinutes(10).ToString("yyMMddHHmm"),
                                mobile = string.IsNullOrWhiteSpace(order.Mobile) ? WK.Tea.Lock.ApiRequest.LockApiHelper.Mobile : order.Mobile
                            };
                            WK.Tea.Lock.ApiRequest.CreateCardResponse result =
                                WK.Tea.Lock.ApiRequest.LockApiHelper.WebApi.Post<WK.Tea.Lock.ApiRequest.CreateCardRequest, WK.Tea.Lock.ApiRequest.CreateCardResponse>("https://api.uclbrt.com/?c=Qrcode&a=getLink", postEntity);
                            using (IT_Order repository = new T_OrderRepository())
                            {
                                order.CardNo = result.cardNo;
                                order.OP = null;
                                repository.Update(order);
                            }
                        }
                        else
                        {
                            resultMsg.code = 1;
                            resultMsg.msg = "门店不存在";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                resultMsg.code = (int)StatusCodeEnum.Error;
                resultMsg.msg = ex.Message;
            }
            return resultMsg.toJson();
        }

        [HttpPost]
        [Route("{id}/delete")]
        public HttpResponseMessage DeleteOrder(int id)
        {
            ResultMsg resultMsg = new ResultMsg();
            try
            {
                using (IT_Order repository = new T_OrderRepository())
                {
                    T_Order order = new T_Order { ID = id };
                    repository.Delete(order);
                }
            }
            catch (Exception ex)
            {
                resultMsg.code = (int)StatusCodeEnum.Error;
                resultMsg.msg = ex.Message;
            }
            return resultMsg.toJson();
        }

        [Route("{prefix}/number")]
        public HttpResponseMessage GetOrderNo(string prefix)
        {
            ResultMsg resultMsg = new ResultMsg();
            try
            {
                TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
                resultMsg.data = prefix.ToUpper() + Convert.ToInt64(ts.TotalMilliseconds).ToString();
            }
            catch (Exception ex)
            {
                resultMsg.code = (int)StatusCodeEnum.Error;
                resultMsg.msg = ex.Message;
            }
            return resultMsg.toJson();
        }

        [Route("pagelist")]
        public HttpResponseMessage GetOrderList(int limit, int page, int? SearchShopID = null, string SearchDate = null)
        {
            ResultMsg resultMsg = new ResultMsg();
            try
            {
                if (limit < 1)
                    limit = 10;
                if (page < 1)
                    page = 1;
                OrderSearchModel search = new OrderSearchModel();
                search.ShopID = SearchShopID;

                if (!string.IsNullOrWhiteSpace(SearchDate))
                {
                    Regex reg = new Regex(@"[0-9]{4}[/-][0-9]{1,2}[/-][0-9]{1,2}");
                    var dates = reg.Matches(SearchDate);
                    if (dates.Count > 1)
                    {
                        search.BTimeStart = Convert.ToDateTime(dates[0].Value);
                        search.BTimeEnd = Convert.ToDateTime(dates[1].Value).AddDays(1);
                    }
                }
                search.limit = limit;
                search.page = page;
                using (IT_Order repository = new T_OrderRepository())
                {
                    var pagelist = repository.GetVOrderPageList(search);
                    resultMsg.data = pagelist.Data;
                    resultMsg.count = pagelist.TotalItemCount;
                }

            }
            catch (Exception ex)
            {
                resultMsg.code = (int)StatusCodeEnum.Error;
                resultMsg.msg = ex.Message;
            }
            return resultMsg.toJson();
        }

        [Route("report")]
        public HttpResponseMessage GetOrderReport(string ReportWay, string SearchDate = null)
        {
            ResultMsg resultMsg = new ResultMsg();
            try
            {
                string tjWay = "";
                byte? sort = null;
                if (string.IsNullOrEmpty(ReportWay))
                {
                    resultMsg.code = (int)StatusCodeEnum.Error;
                    resultMsg.msg = "请选择筛选条件";
                }
                var ways = ReportWay.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < ways.Length; i++)
                {
                    if (ways[i] == "1")
                    {
                        if (!string.IsNullOrWhiteSpace(tjWay))
                            tjWay += ",";
                        tjWay += "A.ShopID,B.ShopName";
                        sort = 0;
                    }
                    else if (ways[i] == "2")
                    {
                        if (!string.IsNullOrWhiteSpace(tjWay))
                            tjWay += ",";
                        tjWay += "convert(varchar(10),A.BTime,120)";
                    }
                }
                string where = null;
                if (!string.IsNullOrWhiteSpace(SearchDate))
                {
                    Regex reg = new Regex(@"[0-9]{4}[/-][0-9]{1,2}[/-][0-9]{1,2}");
                    var dates = reg.Matches(SearchDate);
                    if (dates.Count > 1)
                    {
                        where = "A.BTime>='" + Convert.ToDateTime(dates[0].Value).ToString() + "'";
                        where += " AND A.BTime<'" + Convert.ToDateTime(dates[1].Value).AddDays(1).ToString() + "'";
                    }
                }
                using (IT_Order repository = new T_OrderRepository())
                {
                    var list = repository.GetOrderReportList(tjWay, where, sort, "test");
                    resultMsg.data = list;
                    resultMsg.count = list.Count;
                }

            }
            catch (Exception ex)
            {
                resultMsg.code = (int)StatusCodeEnum.Error;
                resultMsg.msg = ex.Message;
            }
            return resultMsg.toJson();
        }

        [Route("used/times")]
        public HttpResponseMessage GetOrderUsedTimes(int shopId, DateTime dateTime)
        {
            ResultMsg resultMsg = new ResultMsg();
            try
            {
                using (IT_Order repository = new T_OrderRepository())
                {
                    var list = repository.GetOrderTimeList(shopId, dateTime);
                    resultMsg.data = list;
                }

            }
            catch (Exception ex)
            {
                resultMsg.code = (int)StatusCodeEnum.Error;
                resultMsg.msg = ex.Message;
            }
            return resultMsg.toJson();
        }

        [Route("wx/xuding")]
        public HttpResponseMessage XuDingOrderWX([FromBody] T_Order order)
        {
            ResultMsg resultMsg = new ResultMsg();
            try
            {
                order.ETime = order.BTime.AddMinutes(order.Duration);
                if (order.ETime < DateTime.Now)
                {
                    resultMsg.code = 1;
                    resultMsg.msg = "预定结束时间必须大于当前时间！";
                }
                else if (order.BTime.Minute != 0 && order.BTime.Minute != 30)
                {
                    resultMsg.code = 1;
                    resultMsg.msg = "预定开始时间必须以半小时为间隔，如：12:00，12:30！";
                }
                else if (order.ETime.Minute != 0 && order.ETime.Minute != 30)
                {
                    resultMsg.code = 1;
                    resultMsg.msg = "预定结束时间必须以半小时为间隔，如：14:00，14:30！";
                }
                else if (order.Duration < 30 || order.Duration % 30 != 0)
                {
                    resultMsg.code = 1;
                    resultMsg.msg = "预定时长至少半小时，时长必须半小时为整";
                }
                else
                {
                    bool checkOrderTime = false;
                    using (IT_Order repository = new T_OrderRepository())
                    {
                        checkOrderTime = repository.CheckOrderTime(order);
                    }
                    if (checkOrderTime)
                    {
                        resultMsg.code = 1;
                        resultMsg.msg = "预定时间段被占用，请重新选择预定时间";
                    }
                    else
                    {
                        T_Order porder = null;
                        using (IT_Order repository = new T_OrderRepository())
                        {
                            porder = repository.FindFirstOrDefault(o => o.OrderNo == order.ParentNo && o.PayStatus == 1);

                        }
                        if (porder != null)
                        {
                            order.ShopID = porder.ShopID;
                            order.Mobile = porder.Mobile;
                            order.CardNo = porder.CardNo;
                            order.ReMarks = 0;
                            order.Flag = 1;

                            T_Shop shop = null;
                            using (IT_Shop repository = new T_ShopRepository())
                            {
                                shop = repository.FindFirstOrDefault(o => o.ID == order.ShopID);
                            }
                            if (shop != null)
                            {
                                WK.Tea.Lock.ApiRequest.CreateCardRequest postEntity = new WK.Tea.Lock.ApiRequest.CreateCardRequest
                                {
                                    communityNo = "1316882760",
                                    roomNo = shop.RoomNo,
                                    floorNo = shop.FloorNo,
                                    buildNo = shop.BuildNo,
                                    startTime = order.BTime.AddMinutes(-15).ToString("yyMMddHHmm"),
                                    endTime = order.ETime.AddMinutes(10).ToString("yyMMddHHmm"),
                                    mobile = string.IsNullOrWhiteSpace(order.Mobile) ? WK.Tea.Lock.ApiRequest.LockApiHelper.Mobile : order.Mobile
                                };
                                //WK.Tea.Lock.ApiRequest.CreateCardResponse result =
                                //    WK.Tea.Lock.ApiRequest.LockApiHelper.WebApi.Post<WK.Tea.Lock.ApiRequest.CreateCardRequest, WK.Tea.Lock.ApiRequest.CreateCardResponse>("https://api.uclbrt.com/?c=Qrcode&a=getLink", postEntity);
                                using (IT_Order repository = new T_OrderRepository())
                                {
                                    TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
                                    order.OrderNo = "XDWX" + Convert.ToInt64(ts.TotalMilliseconds).ToString();
                                    //order.CardNo = result.cardNo;
                                    order.ReMarks = 1;
                                    order.PayStatus = 0;
                                    order.Flag = 0;
                                    order.FeeCode = shop.ReOrderFee;
                                    var t = order.Duration - 30;
                                    if (t > 0)
                                    {
                                        order.FeeCode += t / 30 * shop.ReOrderFee;
                                    }
                                    order.OpenID = User.Identity.Name;
                                    order.CTime = DateTime.Now;
                                    order = repository.Insert(order);

                                    resultMsg.code = 200;
                                    resultMsg.data = order;
                                }
                            }
                            else
                            {
                                resultMsg.code = 1;
                                resultMsg.msg = "门店不存在";
                            }
                        }
                        else
                        {
                            resultMsg.code = 1;
                            resultMsg.msg = "主订单不存在";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                resultMsg.code = (int)StatusCodeEnum.Error;
                resultMsg.msg = ex.Message;
            }
            return resultMsg.toJson();
        }


        [Route("statics")]
        public HttpResponseMessage GetOrderStatics()
        {
            ResultMsg resultMsg = new ResultMsg();
            try
            {
                using (IT_Order repository = new T_OrderRepository())
                {
                    var list = repository.GetOrderStatics();

                    resultMsg.data = new { orderDate = list.Select(c => c.OrderDate).ToList(), orderCount = list.Select(c => c.OrderCount).ToList(), orderAmount = list.Select(c => c.OrderAmount).ToList(), total = repository.GetOrderStaticsData() };
                }

            }
            catch (Exception ex)
            {
                resultMsg.code = (int)StatusCodeEnum.Error;
                resultMsg.msg = ex.Message;
            }
            return resultMsg.toJson();
        }

        [Route("userstatics")]
        public HttpResponseMessage GetUserStatics(int limit, int page)
        {

            ResultMsg resultMsg = new ResultMsg();
            try
            {
                if (limit < 1)
                    limit = 10;
                if (page < 1)
                    page = 1;
                OrderSearchModel search = new OrderSearchModel();
                search.limit = limit;
                search.page = page;
                using (T_AdminRepository repository = new T_AdminRepository())
                {
                    var pagelist = repository.GetVSUserPageList(search.page, search.limit);
                    resultMsg.data = pagelist.Data;
                    resultMsg.count = pagelist.TotalItemCount;
                }

            }
            catch (Exception ex)
            {
                resultMsg.code = (int)StatusCodeEnum.Error;
                resultMsg.msg = ex.Message;
            }
            return resultMsg.toJson();
        }
    }
}
