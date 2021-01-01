using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;

namespace WK.Tea.Lock.ApiRequest.EEUN
{
    public class WebApiHelper
    {
        private static Cache cache = HttpRuntime.Cache;
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
        public string GetTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalMilliseconds).ToString();
        }

        /// <summary>  
        /// 获取随机数
        /// </summary>  
        /// <returns></returns>  
        public string GetRandom()
        {
            //System.Web.Caching.Cache cache = new System.Web.Caching.Cache();
            //cache.Add("","",null,)
            Random rd = new Random(DateTime.Now.Millisecond);
            int i = rd.Next(0, int.MaxValue);
            return i.ToString();
        }

        public string GetToken()
        {
            var token = cache["token"] == null?null:cache["token"].ToString();
            if (token==null)
            {
                Dictionary<string, string> sortedParams = new Dictionary<string, string>()
                {
                    { "APPID","6005BAFEA0C54011B6602D7A70C36E6C"},
                    { "AT",GetTimeStamp()},
                    { "NONCESTR",GetRandom()},
                    { "PASSWORD","pw112233"},
                    { "USERNAME","13311237111"}
                };

                var sign = GetSignature(sortedParams);
                sortedParams.Add("SIGN", sign);
                var result = Get("https://yylock.eeun.cn/dms/app/dmsLogin", sortedParams);

                token = JsonConvert.DeserializeObject<TokenResponse>(result).token;
                cache.Add("token", token, null, DateTime.Now.AddDays(1).Date, Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return token;
        }

        /// <summary>
        /// Get请求
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="webApi"></param>
        /// <param name="queryStr"></param>
        /// <param name="staffId"></param>
        /// <returns></returns>
        public string Get(string webApi, Dictionary<string, string> parames)
        {
            Tuple<string, string> parameters = GetQueryString(parames);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(webApi + "?" + parameters.Item2);

            request.Method = "GET";
            request.ContentType = "application/json";
            request.Timeout = 90000;
            request.Headers.Set("Pragma", "no-cache");
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream streamReceive = response.GetResponseStream();
            StreamReader streamReader = new StreamReader(streamReceive, Encoding.UTF8);
            string strResult = streamReader.ReadToEnd();

            streamReader.Close();
            streamReceive.Close();
            request.Abort();
            response.Close();

            return strResult;
            //return JsonConvert.DeserializeObject<ResultMsg>(strResult);
        }

        /// <summary>
        /// 拼接get参数
        /// </summary>
        /// <param name="parames"></param>
        /// <returns></returns>
        private Tuple<string, string> GetQueryString(Dictionary<string, string> parames)
        {
            // 第一步：把字典按Key的字母顺序排序
            IDictionary<string, string> sortedParams = new SortedDictionary<string, string>(parames);
            IEnumerator<KeyValuePair<string, string>> dem = sortedParams.GetEnumerator();

            // 第二步：把所有参数名和参数值串在一起
            StringBuilder query = new StringBuilder("");  //签名字符串
            StringBuilder queryStr = new StringBuilder(""); //url参数
            if (parames == null || parames.Count == 0)
                return new Tuple<string, string>("", "");

            while (dem.MoveNext())
            {
                string key = dem.Current.Key;
                string value = dem.Current.Value;
                if (!string.IsNullOrEmpty(key))
                {
                    query.Append(key).Append(value);
                    queryStr.Append("&").Append(key).Append("=").Append(value);
                }
            }

            return new Tuple<string, string>(query.ToString(), queryStr.ToString().Substring(1, queryStr.Length - 1));
        }

        /// <summary>
        /// 计算签名
        /// </summary>
        /// <param name="timeStamp"></param>
        /// <param name="nonce"></param>
        /// <param name="staffId"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public string GetSignature(Dictionary<string, string> date)
        {

            System.Security.Cryptography.MD5 md5Hasher = System.Security.Cryptography.MD5.Create();
            var str = GetQueryString(date).Item2;
            str += "&APPKEY=B8E534AA63664EFC84D038BC1131C719";
            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(str));

            // Create a new Stringbuilder to collect the bytes and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("X2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString().ToUpper();
        }

        public bool AddLockKey(string code, string lockId, string beginTime, string endTime)
        {
            Dictionary<string, string> sortedParams = new Dictionary<string, string>()
                            {
                                { "APPID","6005BAFEA0C54011B6602D7A70C36E6C"},
                                { "AT",WebApiHelper.CreateInstance().GetTimeStamp()},
                                { "NONCESTR",WebApiHelper.CreateInstance().GetRandom()},
                                { "KEYUSERID","13311237111"},
                                { "TOKEN",WebApiHelper.CreateInstance().GetToken()},
                                { "KEYLOCKID",lockId},
                                { "CARDPSWID",code},
                                { "CARDTYPE","2"},
                                { "ENDDATE",endTime},
                                { "OPERATETYPE","1"},
                                { "STARTDATE",beginTime},
                            };

            var sign = WebApiHelper.CreateInstance().GetSignature(sortedParams);
            sortedParams.Add("SIGN", sign);

            var result = WebApiHelper.CreateInstance().Get(" https://yylock.eeun.cn/dms/app/addLockKey", sortedParams);
            var lockKey = JsonConvert.DeserializeObject<LockKeyRespinse>(result).result == 0;

            return lockKey;
        }
    }
}
