using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WK.Tea.Lock.ApiRequest
{
    public class CreateCardResponse : ResultMsg
    {
        public string cardNo { get; set; }

        public string cardType { get; set; }
    }
}
