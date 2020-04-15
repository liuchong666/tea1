using Codeplex.Data;
using System;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using WK.Tea.Lock.ApiRequest;
using WK.Tea.Web.Auth;
using WK.Tea.Web.Common;

namespace WK.Tea.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);


            WeixinConfig.Register();

            LockApiHelper.CreateInstance("543c90f2a87c630adb83d447892db4ce", "733aa1ae86c42f32d1eab02672af47");
            //WeixinApiHelper.CreateInstance("wxe1a8aacc6680a077", "09e24ff14b0884688e1209b2431f8bbf");


            log4net.Config.XmlConfigurator.Configure(new FileInfo(HttpContext.Current.Server.MapPath("~/LogWriterConfig.xml")));
            LogWriter.Default.WriteWarning("app started.");
        }

        protected void Application_End()
        {
            LogWriter.Default.WriteWarning("app stopped.");
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception ex = Server.GetLastError().GetBaseException();
            StringBuilder str = new StringBuilder();
            str.Append("\r\n.客户信息：");
            string ip = "";
            if (Request.ServerVariables.Get("HTTP_X_FORWARDED_FOR") != null)
            {
                ip = Request.ServerVariables.Get("HTTP_X_FORWARDED_FOR").ToString().Trim();
            }
            else
            {
                ip = Request.ServerVariables.Get("Remote_Addr").ToString().Trim();
            }
            str.Append("\r\n\tIp:" + ip);
            str.Append("\r\n\t浏览器:" + Request.Browser.Browser.ToString());
            str.Append("\r\n\t浏览器版本:" + Request.Browser.MajorVersion.ToString());
            str.Append("\r\n\t操作系统:" + Request.Browser.Platform.ToString());
            str.Append("\r\n.错误信息：");
            str.Append("\r\n\t页面：" + Request.Url.ToString());
            str.Append("\r\n\t错误信息：" + ex.Message);
            str.Append("\r\n\t错误源：" + ex.Source);
            str.Append("\r\n\t异常方法：" + ex.TargetSite);
            str.Append("\r\n\t堆栈信息：" + ex.StackTrace);
            str.Append("\r\n--------------------------------------------------------------------------------------------------");
            //创建路径 
            LogWriter.Default.WriteError(str.ToString());
        }

        protected void Application_AuthenticateRequest()
        {
            // 1. 读登录Cookie
            HttpCookie cookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            try
            {
                dynamic userData = null;
                // 2. 解密Cookie值，获取FormsAuthenticationTicket对象
                FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(cookie?.Value);
                if (ticket != null && string.IsNullOrEmpty(ticket.UserData) == false)
                    // 3. 还原用户数据
                    userData = DynamicJson.Parse(ticket.UserData);
                if (ticket != null && userData != null)
                    // 4. 构造我们的MyFormsPrincipal实例，重新给context.User赋值。
                    HttpContext.Current.User =  new MyFormsPrincipal<dynamic>(ticket, userData);
            }
            catch { /* 有异常也不要抛出，防止攻击者试探。 */ }
        }
    }
}
