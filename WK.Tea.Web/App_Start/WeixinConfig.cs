using Codeplex.Data;
using Deepleo.Weixin.SDK;
using Deepleo.Weixin.SDK.JSSDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WK.Tea.Web
{
    public class WeixinConfig
    {
        /// <summary>
        ///access_token expire time 
        /// </summary>
        public const int ACCESS_TOKEN_EXPIRE_SECONDS = 7000;

        /// <summary>
        /// cache 
        /// </summary>
        private static System.Web.Caching.Cache cache = HttpRuntime.Cache;


        class weixin_token
        {
            public string access_token { set; get; }
            public string jssdk_ticket { set; get; }

        }

        public static string Token { private set; get; }
        public static string EncodingAESKey { private set; get; }
        public static string AppID { private set; get; }
        public static string AppSecret { private set; get; }
        public static string PartnerKey { private set; get; }
        public static string mch_id { private set; get; }
        public static string device_info { private set; get; }
        public static string spbill_create_ip { private set; get; }
        public static string OauthScope { private set; get; }
        public static string OrderPaySuccessMsgTemplateId { private set; get; }
        public static string OrderPayFailMsgTemplateId { private set; get; }
        public static string OrderManagerMsgTemplateId { private set; get; }
        public static string CleanMsgTemplateId { private set; get; }
        public static string[] ManagerId { private set; get; }
        public static string CleanerId { private set; get; }
        public static string AccessToken
        {
            get
            {
                //return "29_nW9PQfzOjv-iboRlmlOpAyW4yMua8omqsKfRSyO3q9evJwZivP--lsbdLEgRjQ2voR7IOPNg2cSUtoP0Z8qLz1iMVDFT6mqlNZiqQ_MWoVURpW54Shdk6PugNsLqBSFDdO0vTHmyGQPvkGqdCYJgABAXTE";
                if (cache.Get(AppID) == null)
                {
                    ReqWeixinToken();
                }

                var weixin_token = DynamicJson.Parse(cache.Get(AppID).ToString());
                return weixin_token.access_token;
            }
        }

        public static string JsSdkTicket
        {
            get
            {
                if (cache.Get(AppID) == null)
                {
                    ReqWeixinToken();
                }

                var weixin_token = DynamicJson.Parse(cache.Get(AppID).ToString());
                return weixin_token.jssdk_ticket;
            }
        }

        private static void ReqWeixinToken()
        {
            var access_token = BasicAPI.GetAccessToken(AppID, AppSecret).access_token;
            var js = JSAPI.GetTickect(access_token);
            var jssdk_ticket = js.ticket;
            var json = DynamicJson.Serialize(new weixin_token { access_token = access_token, jssdk_ticket = jssdk_ticket });
            cache.Insert(AppID, json, null, DateTime.Now.AddSeconds(ACCESS_TOKEN_EXPIRE_SECONDS), System.Web.Caching.Cache.NoSlidingExpiration);
        }

        public static void Register()
        {

            Token = System.Configuration.ConfigurationManager.AppSettings["Token"];
            EncodingAESKey = System.Configuration.ConfigurationManager.AppSettings["EncodingAESKey"];
            AppID = System.Configuration.ConfigurationManager.AppSettings["AppID"];
            AppSecret = System.Configuration.ConfigurationManager.AppSettings["AppSecret"];
            PartnerKey = System.Configuration.ConfigurationManager.AppSettings["PartnerKey"];
            mch_id = System.Configuration.ConfigurationManager.AppSettings["mch_id"];
            device_info = System.Configuration.ConfigurationManager.AppSettings["device_info"];
            spbill_create_ip = System.Configuration.ConfigurationManager.AppSettings["spbill_create_ip"];
            OauthScope = System.Configuration.ConfigurationManager.AppSettings["OauthScope"];

            OrderPaySuccessMsgTemplateId = System.Configuration.ConfigurationManager.AppSettings["OrderPaySuccessMsgTemplateId"];
            OrderPayFailMsgTemplateId = System.Configuration.ConfigurationManager.AppSettings["OrderPayFailMsgTemplateId"];
            OrderManagerMsgTemplateId = System.Configuration.ConfigurationManager.AppSettings["OrderManagerMsgTemplateId"];
            CleanMsgTemplateId = System.Configuration.ConfigurationManager.AppSettings["CleanMsgTemplateId"];

            var managers = System.Configuration.ConfigurationManager.AppSettings.AllKeys.Where(o => o.IndexOf("ManagerId_") > -1).ToArray();
            ManagerId = new string[managers.Count()];
            for (int i=0; i< managers.Count(); i++)
            {
                ManagerId[i] = System.Configuration.ConfigurationManager.AppSettings[managers[i]];
            }
            CleanerId = System.Configuration.ConfigurationManager.AppSettings["CleanerId"];
        }
    }
}