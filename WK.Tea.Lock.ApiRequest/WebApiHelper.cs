using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace WK.Tea.Lock.ApiRequest
{
    public class LockApiHelper
    {
        private LockApiHelper(string account, string token)
        {
            this.m_account = account;
            this.m_token = token;
        }

        private string m_account = null;
        private string m_token = null;

        private static LockApiHelper _LockApiHelper = null;
        private static object Lock = new object();
        public static LockApiHelper WebApi
        {
            get
            {
                return _LockApiHelper;
            }
        }

        public static string Mobile = "18611107715";

        public static LockApiHelper CreateInstance(string account, string token)
        {
            if (_LockApiHelper == null)
            {
                lock (Lock)
                {
                    if (_LockApiHelper == null)
                    {
                        _LockApiHelper = new LockApiHelper(account, token);
                    }
                }
            }
            return _LockApiHelper;
        }

        /// <summary>
        /// Post请求
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <param name="staffId"></param>
        /// <returns></returns>
        public OutT Post<T, OutT>(string url, T parames)
        {
            string date = DateTime.Now.ToString("yyyyMMddhhmmss");
            string sigStr = GetSignature(date);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url + "&sig=" + sigStr);
            setCertificateValidationCallBack();


            Encoding myEncoding = Encoding.GetEncoding("utf-8");
            byte[] myByte = myEncoding.GetBytes(m_account + ":" + date);
            string authStr = Convert.ToBase64String(myByte);
            request.Headers.Add("Authorization", authStr);

            string data = JsonConvert.SerializeObject(parames);
            byte[] bytes = Encoding.UTF8.GetBytes(data);
            //写数据
            request.Method = "POST";
            request.Accept = "application/json";
            request.ContentType = "application/x-www-form-urlencode";

            Stream reqstream = request.GetRequestStream();
            reqstream.Write(bytes, 0, bytes.Length);

            //读数据
            request.Timeout = 300000;
            //request.Headers.Set("Pragma", "no-cache");
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream streamReceive = response.GetResponseStream();
            StreamReader streamReader = new StreamReader(streamReceive, Encoding.UTF8);
            string strResult = streamReader.ReadToEnd();

            //关闭流
            reqstream.Close();
            streamReader.Close();
            streamReceive.Close();
            request.Abort();
            response.Close();



            return JsonConvert.DeserializeObject<OutT>(strResult);
        }

        /// <summary>
        /// Get请求
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="webApi"></param>
        /// <param name="queryStr"></param>
        /// <param name="staffId"></param>
        /// <returns></returns>
        public ResultMsg Get(string webApi, Dictionary<string, string> parames)
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

            return JsonConvert.DeserializeObject<ResultMsg>(strResult);
        }

        /// <summary>
        /// 返回加密编码数据结果
        /// </summary>
        /// <param name="communityNo">集群编号</param>
        /// <param name="mobile">手机号</param>
        /// <param name="areaCode">手机区号</param>
        /// <param name="cardNo">房卡唯一号(可选)</param>
        /// <returns>返回rsa加密编码结果</returns>
        public string GetCardDataParams(string communityNo, string mobile, string areaCode, string cardNo = null)
        {
            string param = "communityNo=" + communityNo + "&id=" + this.m_account + "&token=" + this.m_token + "&mobile=" + mobile + "&areaCode=" + areaCode + "&time=" + GetTimeStamp();
            if (null != cardNo)
            {
                param = param + "&cardNo=" + cardNo;
            }
            
            //RSA加密Base64编码
            string encrypt = RSAEncrypt(param);
            //返回urlencode编码结果
            return System.Web.HttpUtility.UrlEncode(encrypt, Encoding.GetEncoding("utf-8"));
        }

        /// <summary>
        /// 返回RSA加密Base64编码结果
        /// </summary>
        /// <param name="content">加密内容</param>
        /// <returns>加密后Base64编码结果</returns>
        private string RSAEncrypt(string content)
        {
            string publickey = @"-----BEGIN PUBLIC KEY-----MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAqxqOJg0kqL4/xoNf0iDbjz/oM7ujsXOd92vQDkwO/rCP9wwZY0AvrMhcc56X4LmIbsbc1EZQ5ryMrIDbyCgtpgJJTQG/u/FBiwG2Yvqgx+9keVGZhBA+Oph34HFPWz4OEB+Py4QkaJPXALkjjh2Zf7Lgpv5gO8gRyg/o9FwCOZyEGiUmVorwPvwT3oMeNPCHxzlpGzdqV1kfqNmbS4ZkCiXGNhxxN0LJDnhaJJUl4bcnUjpcIxUlgSMX2CcooffIk3E1ROP051Xf/zmUWE6DTcGetf6ni2s2irDCgeanylyjLTgM6xaOYWqtG0yUC5lyzO46yTmE1Q47XMM2h1KJswIDAQAB-----END PUBLIC KEY-----";
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(2048);
            byte[] cipherbytes;
            rsa.ImportParameters(ConvertFromPemPublicKey(publickey));

            cipherbytes = rsa.Encrypt(Encoding.UTF8.GetBytes(content), false);
            return Convert.ToBase64String(cipherbytes);
        }

        #region MD5 和 https交互函数定义
        
        private RSAParameters ConvertFromPemPublicKey(string pemFileConent)
        {
            if (string.IsNullOrEmpty(pemFileConent))
            {
                throw new ArgumentNullException("pemFileConent", "This arg cann't be empty.");
            }
            pemFileConent = pemFileConent.Replace("-----BEGIN PUBLIC KEY-----", "").Replace("-----END PUBLIC KEY-----", "").Replace("\n", "").Replace("\r", "");
            byte[] keyData = Convert.FromBase64String(pemFileConent);
            bool keySize1024 = (keyData.Length == 162);
            bool keySize2048 = (keyData.Length == 294);
            if (!(keySize1024 || keySize2048))
            {
                throw new ArgumentException("pem file content is incorrect, Only support the key size is 1024 or 2048");
            }
            byte[] pemModulus = (keySize1024 ? new byte[128] : new byte[256]);
            var pemPublicExponent = new byte[3];
            Array.Copy(keyData, (keySize1024 ? 29 : 33), pemModulus, 0, (keySize1024 ? 128 : 256));
            Array.Copy(keyData, (keySize1024 ? 159 : 291), pemPublicExponent, 0, 3);
            var para = new RSAParameters { Modulus = pemModulus, Exponent = pemPublicExponent };
            return para;
        }

        #endregion


        /// <summary>  
        /// 获取时间戳  
        /// </summary>  
        /// <returns></returns>  
        private string GetTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }

        public string GetUnixTime()
        {
            return ((int)(DateTime.Now - TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1))).TotalSeconds).ToString();
        }


        /// <summary>  
        /// 获取随机数
        /// </summary>  
        /// <returns></returns>  
        private string GetRandom()
        {
            Random rd = new Random(DateTime.Now.Millisecond);
            int i = rd.Next(0, int.MaxValue);
            return i.ToString();
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
        private string GetSignature(string date)
        {

            System.Security.Cryptography.MD5 md5Hasher = System.Security.Cryptography.MD5.Create();

            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(m_account + m_token + date));

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

        /// <summary>
        /// 设置服务器证书验证回调
        /// </summary>
        public void setCertificateValidationCallBack()
        {
            System.Net.ServicePointManager.ServerCertificateValidationCallback = CertificateValidationResult;
        }

        /// <summary>
        ///  证书验证回调函数  
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="cer"></param>
        /// <param name="chain"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public bool CertificateValidationResult(object obj, System.Security.Cryptography.X509Certificates.X509Certificate cer, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors error)
        {
            return true;
        }
    }
}
