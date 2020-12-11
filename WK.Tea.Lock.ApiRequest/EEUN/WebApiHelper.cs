using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Caching;

namespace WK.Tea.Lock.ApiRequest.EEUN
{
    public class WebApiHelper
    {
        private static Cache cache = new Cache();
        private static WebApiHelper _LockApiHelper = null;
        private static object Lock = new object();
        public static WebApiHelper WebApi
        {
            get
            {
                return _LockApiHelper;
            }
        }

        public static string Mobile = "13311237111";

        public static WebApiHelper CreateInstance()
        {
            if (_LockApiHelper == null)
            {
                lock (Lock)
                {
                    if (_LockApiHelper == null)
                    {
                        _LockApiHelper = new WebApiHelper();
                    }
                }
            }
            return _LockApiHelper;
        }

        /// <summary>  
        /// 获取时间戳  
        /// </summary>  
        /// <returns></returns>  
        private string GetTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalMilliseconds).ToString();
        }

        /// <summary>  
        /// 获取随机数
        /// </summary>  
        /// <returns></returns>  
        private string GetRandom()
        {
            //System.Web.Caching.Cache cache = new System.Web.Caching.Cache();
            //cache.Add("","",null,)
            Random rd = new Random(DateTime.Now.Millisecond);
            int i = rd.Next(0, int.MaxValue);
            return i.ToString();
        }

        private string GetToken()
        {
           var token= cache.Get("token").ToString();
            if (string.IsNullOrEmpty(token))
            {
                token = "";
                cache.Add("token",token,null,DateTime.Now.AddDays(1).Date,Cache.NoSlidingExpiration, CacheItemPriority.Default,null);
            }

            return token;
        }
    }
}
