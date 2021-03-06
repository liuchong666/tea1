﻿using Codeplex.Data;
using Deepleo.Weixin.SDK;
using Deepleo.Weixin.SDK.Helpers;
using Deepleo.Weixin.SDK.Pay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using WK.Tea.DataModel;
using WK.Tea.DataModel.SqlModel;
using WK.Tea.DataProvider.DAL;
using WK.Tea.DataProvider.IDAL;
using WK.Tea.Lock.ApiRequest.EEUN;
using WK.Tea.Web.Common;
using WK.Tea.Web.Models;

namespace WK.Tea.Web.Controllers
{
    public class WxPayController : Controller
    {
        //public ActionResult Order()
        //{
        //    return View();
        //}

        public ActionResult Index(int id)
        {
            try
            {
                T_Order order = null; ;
                using (IT_Order repository = new T_OrderRepository())
                {
                    order = repository.Find(id);
                }
                if (order != null)
                {
                    var out_trade_no = order.OrderNo;
                    var domain = System.Configuration.ConfigurationManager.AppSettings["Domain"];
                    var body = "茶室预定";
                    var detail = "倒茶茶室预定";
                    var attach = "";
                    var product_id = "1";
                    var openid = User.Identity.Name;
                    var goods_tag = "";
                    var fee_type = "CNY";
                    var total_fee = (int)(order.FeeCode.Value * 100);
                    var trade_type = "JSAPI";
                    var appId = System.Configuration.ConfigurationManager.AppSettings["AppId"];
                    var nonceStr = Util.CreateNonce_str();
                    var timestamp = Util.CreateTimestamp();
                    var success_redict_url = string.Format("{0}/WxPay/Success?orderNo={1}", domain, order.OrderNo);
                    var url = domain + Request.Url.PathAndQuery;
                    var userAgent = Request.UserAgent;
                    var userVersion = userAgent.Substring(userAgent.LastIndexOf("MicroMessenger/") + 15, 3);//微信版本号高于或者等于5.0才支持微信支付
                    var spbill_create_ip = (trade_type == "APP" || trade_type == "NATIVE") ? Request.UserHostName : WeixinConfig.spbill_create_ip;
                    var time_start = DateTime.Now.ToString("yyyyMMddHHmmss");
                    var time_expire = DateTime.Now.AddMinutes(15).ToString("yyyyMMddHHmmss");//默认1个小时订单过期，开发者可自定义其他超时机制，原则上微信订单超时时间不超过2小时

                    var notify_url = string.Format("{0}/WxPay/Notify", domain);//与下面的Notify对应，开发者可自定义其他url地址
                    var partnerKey = WeixinConfig.PartnerKey;
                    var mch_id = WeixinConfig.mch_id;
                    var device_info = WeixinConfig.device_info;
                    var returnXML = "";
                    var paramaterXml = "";
                    var content = WxPayAPI.UnifiedOrder(
                                  appId, mch_id, device_info, nonceStr,
                                  body, detail, attach, out_trade_no, fee_type, total_fee, spbill_create_ip, time_start, time_expire,
                                  goods_tag, notify_url, trade_type, product_id, openid, partnerKey, out returnXML, out paramaterXml);
                    LogWriter.Default.WriteError(paramaterXml);
                    LogWriter.Default.WriteError(returnXML);
                    var prepay_id = "";
                    var sign = "";
                    var return_code = "";
                    var return_msg = "";
                    var err_code = "";
                    var err_code_des = "";
                    var isUnifiedOrderSuccess = false;
                    if (content.return_code.Value == "SUCCESS" && content.result_code.Value == "SUCCESS")
                    {
                        prepay_id = content.prepay_id.Value;
                        sign = WxPayAPI.SignPay(appId, timestamp.ToString(), nonceStr, prepay_id, partnerKey);
                        trade_type = content.trade_type.Value;
                        isUnifiedOrderSuccess = true;
                    }
                    else
                    {
                        return_code = content.return_code.Value;
                        return_msg = content.return_msg.Value;
                        isUnifiedOrderSuccess = false;
                    }
                    if (!isUnifiedOrderSuccess)
                    {
                        return RedirectToAction("Failed", new { msg = "(代码:101)服务器下单失败，请重试" });
                    }
                    var model = new JSPayModel
                    {
                        appId = appId,
                        nonceStr = nonceStr,
                        timestamp = timestamp,
                        userAgent = userAgent,
                        userVersion = userVersion,
                        attach = "",
                        body = body,
                        detail = detail,
                        openid = openid,
                        product_id = out_trade_no,
                        goods_tag = goods_tag,
                        price = order.FeeCode.Value,
                        total_fee = total_fee,
                        prepay_id = prepay_id,
                        trade_type = trade_type,
                        sign = sign,
                        return_code = return_code,
                        return_msg = return_msg,
                        err_code = err_code,
                        err_code_des = err_code_des,
                        success_redict_url = success_redict_url
                    };

                    return View(model);
                }
                else
                {
                    return View();
                }
            }
            catch (Exception ex)
            {
                LogWriter.Default.WriteError(ex.Message);
                return RedirectToAction("Failed", new { msg = "(代码:200)" + ex.Message });
            }
        }
        public ActionResult Success(string orderNo)
        {
            VOrderModel order = null; ;
            using (IT_Order repository = new T_OrderRepository())
            {
                order = repository.GetVOrderByOrderNo(orderNo);
            }
            return View(order);
        }

        public ActionResult Failed(string msg)
        {
            ViewBag.msg = msg;
            return View();
        }

        /// <summary>
        /// 公共API => 支付结果通用通知
        /// http://pay.weixin.qq.com/wiki/doc/api/index.php?chapter=9_7
        /// 微信支付回调,不需要证书 
        /// 
        /// 应用场景 
        /// 支付完成后，微信会把相关支付结果和用户信息发送给商户，商户需要接收处理，并返回应答。 
        /// 对后台通知交互时，如果微信收到商户的应答不是成功或超时，微信认为通知失败，微信会通过一定的策略（如30分钟共8次）定期重新发起通知，尽可能提高通知的成功率，但微信不保证通知最终能成功。 
        /// 由于存在重新发送后台通知的情况，因此同样的通知可能会多次发送给商户系统。商户系统必须能够正确处理重复的通知。 
        /// 推荐的做法是，当收到通知进行处理时，首先检查对应业务数据的状态，判断该通知是否已经处理过，如果没有处理过再进行处理，如果处理过直接返回结果成功。在对业务数据进行状态检查和处理之前，要采用数据锁进行并发控制，以避免函数重入造成的数据混乱。 
        /// 技术人员可登进微信商户后台扫描加入接口报警群。 
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult Notify()
        {
            var doc = XDocument.Load(Request.InputStream);
            var sPara = doc.Root.Descendants().ToDictionary(x => x.Name.LocalName, x => x.Value);
            if (sPara.Count <= 0)
            {
                throw new ArgumentNullException();
            }

            LogWriter.Default.WriteError("Notify Parameters:" + sPara.ToString());//记录请求关键信息到日志中去

            if (sPara["return_code"] == "SUCCESS" && sPara["result_code"] == "SUCCESS")
            {
                var sign = sPara["sign"];
                var signValue = WxPayAPI.Sign(sPara, WeixinConfig.PartnerKey);
                bool isVerify = sign == signValue;
                LogWriter.Default.WriteError("Verify:" + isVerify + "|sign/signValue:" + sign + "," + signValue);
                if (isVerify)
                {
                    string out_trade_no = sPara["out_trade_no"];//商户订单ID： 1.注意交易单不要重复处理；2.注意判断返回金额
                    string transaction_id = sPara["transaction_id"]; //微信支付订单号
                    string time_end = sPara["time_end"];//支付完成时间
                    int total_fee = int.Parse(sPara["total_fee"]); //总金额
                    string bank_type = sPara["bank_type"]; //付款银行

                    var openid = sPara["openid"];

                    //****************************************************************************************
                    //TODO 商户处理订单逻辑： 1.注意交易单不要重复处理；2.注意判断返回金额
                    T_Order order = null;
                    using (IT_Order repository = new T_OrderRepository())
                    {
                        order = repository.FindFirstOrDefault(o => o.OrderNo == out_trade_no);
                    }

                    if (order != null)
                    {
                        order.PayStatus = 1;
                        T_Shop shop = null;
                        using (IT_Shop repository = new T_ShopRepository())
                        {
                            shop = repository.FindFirstOrDefault(o => o.ID == order.ShopID);
                        }
                        if (shop != null)
                        {
                            string cardNo = string.Empty;
                            var code = new Random().Next(1000, 9999).ToString();
                            if (shop.LockType != 1)
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
                                
                                cardNo = result.cardNo;
                            }
                            else
                            {
                                var startTime = order.BTime.AddMinutes(-15).ToString("yyMMddHHmm");
                                var endTime = order.ETime.AddMinutes(10).ToString("yyMMddHHmm");
                                
                                WebApiHelper.CreateInstance().AddLockKey(code, shop.LockID, startTime, endTime);
                            }

                            order.CardNo = cardNo;
                            order.LockPW = code;

                            using (IT_Order repository = new T_OrderRepository())
                            {
                                repository.Update(order);
                            }

                            //TODO:postData中携带该次支付的用户相关信息，这将便于商家拿到openid，以便后续提供更好的售后服务，譬如：微信公众好通知用户付款成功。如果不提供服务则可以删除此代码
                            string url = "http://dc.orangenet.com.cn/Door/Qrcode?orderId=" + order.ID;
                            WeixinTempMsg.SendOrderPaySuccessMsg(openid, url, shop.ShopAddress, order.BTime, order.ETime, order.FeeCode.Value, order.OrderNo);
                            WeixinTempMsg.SendManagerOrderMsg(url, shop.ShopAddress, order.BTime, order.ETime, order.FeeCode.Value, order.OrderNo);
                            WeixinTempMsg.SendSMS(shop.ShopAddress, order.Mobile, shop.ShopPhoneNum, order.BTime, order.ETime, url);
                            //WeixinTempMsg.SendCleanMsg(shop.ShopAddress, order.OrderNo, order.BTime, order.ETime);
                        }
                        else
                        {
                            LogWriter.Default.WriteError("Shop Error, out_trade_no:" + out_trade_no + ", shop is null");
                        }
                    }
                    else
                    {
                        LogWriter.Default.WriteError("Order Error, out_trade_no:" + out_trade_no + ", order is null");
                    }


                    LogWriter.Default.WriteError("Notify Success, out_trade_no:" + out_trade_no + ",transaction_id" + transaction_id + ",time_end:" + time_end + ",total_fee:" + total_fee + ",bank_type:" + bank_type + ",openid:" + openid);
                    return Content(string.Format("<xml><return_code><![CDATA[{0}]]></return_code><return_msg><![CDATA[{1}]]></return_msg></xml>", "SUCCESS", "OK"));
                }
            }
            return Content(string.Format("<xml><return_code><![CDATA[{0}]]></return_code></xml>", "FAIL"));
        }


        /// <summary>
        /// 退款结果通知
        /// https://pay.weixin.qq.com/wiki/doc/api/jsapi.php?chapter=9_16&index=9
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult Refund()
        {
            var doc = XDocument.Load(Request.InputStream);
            var sPara = doc.Root.Descendants().ToDictionary(x => x.Name.LocalName, x => x.Value);
            if (sPara.Count <= 0)
            {
                throw new ArgumentNullException();
            }
            LogWriter.Default.WriteError("Refund Parameters:" + sPara.ToString());//记录请求关键信息到日志中去
            bool isVerify = false;
            var sign = sPara["sign"];
            var retcode = sPara["retcode"];
            if (retcode != "0")
            {
                isVerify = false;
            }
            else
            {
                var signValue = WxPayAPI.Sign(sPara, WeixinConfig.PartnerKey);
                isVerify = sign == signValue;
            }
            if (!isVerify)
            {
                return Content(string.Format("<xml><return_code><![CDATA[{0}]]></return_code></xml>", "FAIL"));
            }
            //TODO:商户处理订单逻辑

            //END 商户处理订单逻辑
            string out_trade_no = sPara["out_trade_no"];//商户订单ID： 1.注意交易单不要重复处理；2.注意判断返回金额

            return Content(string.Format("<xml><return_code><![CDATA[{0}]]></return_code><return_msg><![CDATA[{1}]]></return_msg></xml>", "SUCCESS", "OK"));
        }
    }
}