using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;

namespace WK.Tea.Web.Common
{
    public static class HttpResponseExtension
    {
        public static HttpResponseMessage toJson(this Object obj)
        {
            String str;
            if (obj is String || obj is Char)
            {
                str = obj.ToString();
            }
            else
            {
                str = JsonConvert.SerializeObject(obj, new JsonSerializerSettings { NullValueHandling= NullValueHandling.Ignore, DateFormatString = "yyyy-MM-dd HH:mm:ss" });
            }
            HttpResponseMessage result = new HttpResponseMessage { Content = new StringContent(str, Encoding.GetEncoding("UTF-8"), "application/json") };
            return result;
        }
    }
}