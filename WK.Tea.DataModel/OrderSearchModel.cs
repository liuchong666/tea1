using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WK.Tea.DataModel
{
    public class OrderSearchModel : SearchModel
    {
        public int? ShopID { get; set; }

        public DateTime? BTimeStart { get; set; }

        public DateTime? BTimeEnd { get; set; }

    }
}
