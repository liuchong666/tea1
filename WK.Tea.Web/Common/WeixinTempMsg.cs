using Codeplex.Data;
using Deepleo.Weixin.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace WK.Tea.Web.Common
{
    public class WeixinTempMsg
    {
        public class TemplateMsgContent
        {
            public string template_id;

            public string touser;

            public string url;
        }



        public static void SendOrderPaySuccessMsg(string touser, string url, string shopAddress, DateTime orderBeginTime, DateTime orderEndTime, decimal feeCode, string orderNo)
        {
            StringBuilder jsonString = new StringBuilder();

            jsonString.Append("{");
            jsonString.AppendFormat("\"template_id\":\"{0}\",", WeixinConfig.OrderPaySuccessMsgTemplateId);
            jsonString.AppendFormat("\"touser\":\"{0}\",", touser);
            jsonString.AppendFormat("\"url\":\"{0}\",", url);
            jsonString.Append("\"data\":{");
            jsonString.Append("\"first\":{");
            jsonString.AppendFormat("\"value\":\"{0}\"", "尊敬的客户，您好！您预订的茶室支付结果通知");
            jsonString.Append("},");
            jsonString.Append("\"keyword1\":{");
            jsonString.AppendFormat("\"value\":\"{0} 元\", ", feeCode);
            jsonString.AppendFormat("\"color\":\"{0}\"", "#576b95");
            jsonString.Append("},");

            jsonString.Append("\"keyword2\":{");
            jsonString.AppendFormat("\"value\":\"{0}\",", shopAddress);
            jsonString.AppendFormat("\"color\":\"{0}\"", "#ff0000");
            jsonString.Append("},");

            jsonString.Append("\"keyword3\":{");
            jsonString.AppendFormat("\"value\":\"{0} 到 {1}\",", orderBeginTime.ToString("yyyy/MM/dd HH:mm"), orderEndTime.ToString("yyyy/MM/dd HH:mm"));
            jsonString.AppendFormat("\"color\":\"{0}\"", "#ff0000");
            jsonString.Append("},");

            jsonString.Append("\"keyword4\":{");
            jsonString.AppendFormat("\"value\":\"{0}\",", orderNo);
            jsonString.AppendFormat("\"color\":\"{0}\"", "#576b95");
            jsonString.Append("},");

            jsonString.Append("\"remark\":{");
            jsonString.AppendFormat("\"value\":\"{0}\"", "点击查看门锁二维码");
            jsonString.Append("}");
            jsonString.Append("}");
            jsonString.Append("}");

            var sendResult = TemplateMessageAPI.SendTemplateMessage(WeixinConfig.AccessToken, jsonString.ToString());

            LogWriter.Default.WriteError("Send Message, Content:" + jsonString.ToString() + ", Result: " + DynamicJson.Serialize(sendResult));
        }

        public static void SendManagerOrderMsg(string url, string shopAddress, DateTime orderBeginTime, DateTime orderEndTime, decimal feeCode,string orderNo,int type=0)
        {
            foreach (var touser in WeixinConfig.ManagerId)
            {
                StringBuilder jsonString = new StringBuilder();

                jsonString.Append("{");
                jsonString.AppendFormat("\"template_id\":\"{0}\",", WeixinConfig.OrderPaySuccessMsgTemplateId);
                jsonString.AppendFormat("\"touser\":\"{0}\",", touser);
                jsonString.AppendFormat("\"url\":\"{0}\",", url);
                jsonString.Append("\"data\":{");
                jsonString.Append("\"first\":{");
                jsonString.AppendFormat("\"value\":\"{0}\"", $"尊敬的管理员，您好！客户{(type == 0 ? "预定" : "续订")}茶室支付结果通知");
                jsonString.Append("},");

                jsonString.Append("\"keyword1\":{");
                jsonString.AppendFormat("\"value\":\"{0} 元\", ", feeCode);
                jsonString.AppendFormat("\"color\":\"{0}\"", "#576b95");
                jsonString.Append("},");

                jsonString.Append("\"keyword2\":{");
                jsonString.AppendFormat("\"value\":\"{0}\",", shopAddress);
                jsonString.AppendFormat("\"color\":\"{0}\"", "#ff0000");
                jsonString.Append("},");

                jsonString.Append("\"keyword3\":{");
                jsonString.AppendFormat("\"value\":\"{0} 到 {1}\",", orderBeginTime.ToString("yyyy/MM/dd HH:mm"), orderEndTime.ToString("yyyy/MM/dd HH:mm"));
                jsonString.AppendFormat("\"color\":\"{0}\"", "#ff0000");
                jsonString.Append("},");
                
                jsonString.Append("\"keyword4\":{");
                jsonString.AppendFormat("\"value\":\"{0}\",", orderNo);
                jsonString.AppendFormat("\"color\":\"{0}\"", "#576b95");
                jsonString.Append("},");

                jsonString.Append("\"remark\":{");
                jsonString.AppendFormat("\"value\":\"{0}\"", "点击查看门锁二维码");
                jsonString.Append("}");
                jsonString.Append("}");
                jsonString.Append("}");
                
                var sendResult = TemplateMessageAPI.SendTemplateMessage(WeixinConfig.AccessToken, jsonString.ToString());

                LogWriter.Default.WriteError("Send Message, Content:" + jsonString.ToString() + ", Result: " + DynamicJson.Serialize(sendResult));
            }
            
        }

        public static void SendCleanMsg(string shopAddress, string orderNo, DateTime orderBeginTime, DateTime orderEndTime,string cleanOpenId,int type=0)
        {
            StringBuilder jsonString = new StringBuilder();

            jsonString.Append("{");
            jsonString.AppendFormat("\"template_id\":\"{0}\",", WeixinConfig.CleanMsgTemplateId);
            jsonString.AppendFormat("\"touser\":\"{0}\",", cleanOpenId);
            //jsonString.AppendFormat("\"url\":\"{0}\",", url);
            jsonString.Append("\"data\":{");
            jsonString.Append("\"first\":{");
            jsonString.AppendFormat("\"value\":\"{0}\"", shopAddress);
            //jsonString.AppendFormat("\"color\":\"{0}\"", "#ff0000");
            jsonString.Append("},");
            jsonString.Append("\"keyword1\":{");
            jsonString.AppendFormat("\"value\":\"{0}\",", orderNo);
            jsonString.AppendFormat("\"color\":\"{0}\"", "#ff0000");
            jsonString.Append("},");
            jsonString.Append("\"keyword2\":{");
            jsonString.AppendFormat("\"value\":\"{1}结束时间为{0}\",", orderEndTime.ToString("yyyy/MM/dd HH:mm"), (type == 0 ? "使用" : "续订"));
            jsonString.AppendFormat("\"color\":\"{0}\"", "#ff0000");
            jsonString.Append("},");
            jsonString.Append("\"remark\":{");
            jsonString.AppendFormat("\"value\":\"{0}\"", "保洁阿姨烦请于订单结束时间5分钟后及时进行保洁，谢谢！");
            jsonString.Append("}");
            jsonString.Append("}");
            jsonString.Append("}");

            var sendResult = TemplateMessageAPI.SendTemplateMessage(WeixinConfig.AccessToken, jsonString.ToString());

            LogWriter.Default.WriteError("Send Message, Content:" + jsonString.ToString() + ", Result: " + DynamicJson.Serialize(sendResult) + "|" + sendResult.errmsg);
        }
    }
}