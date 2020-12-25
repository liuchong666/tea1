using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WK.Tea.Lock.ApiRequest.EEUN
{
    public class BaseRequest
    {
        public string APPID { get; set; }

        public string AT { get; set; }

        public string NONCESTR { get; set; }

        public string SIGN { get; set; }
    }

    public class TokenResponse
    {
        public int result { get; set; }
        
        public string token { get; set; }
    }

    public class QCodeRespinse
    {
        public int result { get; set; }

        public string data { get; set; }
    }

    public class LockKeyRespinse
    {
        public int result { get; set; }

        public string data { get; set; }
        public string msg { get; set; }
    }

}
