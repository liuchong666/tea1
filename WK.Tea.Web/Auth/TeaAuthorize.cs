using Codeplex.Data;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using System.Web.Security;
using WK.Tea.Web.Common;

namespace WK.Tea.Web.Auth
{
    public class TeaAuthorizeAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            var identity = filterContext.HttpContext.User.Identity;
            
            if (!identity.IsAuthenticated)
            {
                if (Roles == "Weixin")
                {
                    //filterContext.Result = WeixinOAuth(filterContext);
                }
                else
                {
                    base.OnAuthorization(filterContext);
                }
            }
            else
            {
                var user = filterContext.HttpContext.User as MyFormsPrincipal<dynamic>;
                var userInfo = user.UserData as DynamicJson;
                if (Roles == "Weixin" && (!userInfo.IsDefined("Roles") || user.UserData.Roles != Roles))
                {
                    //filterContext.Result = WeixinOAuth(filterContext);
                }
                else
                {
                    base.OnAuthorization(filterContext);
                }
            }
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (Roles == "Weixin")
            {
                var user = httpContext.User as MyFormsPrincipal<dynamic>;
                var userInfo = user.UserData as DynamicJson;

                return userInfo.IsDefined("Roles") && user.UserData.Roles == Roles;
            }
            else
            {
                return base.AuthorizeCore(httpContext);
            }
        }

        public RedirectResult WeixinOAuth(AuthorizationContext filterContext)
        {
            var domain = System.Configuration.ConfigurationManager.AppSettings["Domain"];
            var target_uri = filterContext.RequestContext.HttpContext.Request.Url.ToString();

            var userAgent = filterContext.RequestContext.HttpContext.Request.UserAgent;
            var redirect_uri = string.Format("{0}/OAuth/Callback", domain);//这里需要完整url地址，对应Controller里面的OAuthController的Callback
            var scope = WeixinConfig.OauthScope;
            var state = Math.Abs(DateTime.Now.ToBinary()).ToString();//state保证唯一即可,可以用其他方式生成
                                                                     //这里为了实现简单，将state和target_uri保存在Cache中，并设置过期时间为2分钟。您可以采用其他方法!!!
            HttpContext.Current.Cache.Add(state, target_uri, null, DateTime.Now.AddMinutes(2), TimeSpan.Zero, CacheItemPriority.Normal, null);
            LogWriter.Default.WriteInfo(string.Format("begin weixin oauth: scope: {0}, redirect_uri: {1} , state: {2} , user agent: {3} ", scope, redirect_uri, state, userAgent));
            var weixinOAuth2Url = string.Format(
                     "https://open.weixin.qq.com/connect/oauth2/authorize?appid={0}&redirect_uri={1}&response_type=code&scope={2}&state={3}#wechat_redirect",
                      WeixinConfig.AppID, HttpUtility.UrlEncode(redirect_uri), scope, state);
            return new RedirectResult(weixinOAuth2Url);
        }
    }
}