using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WK.Tea.DataModel.SqlModel;

namespace WK.Tea.DataModel
{
    public class VShopModel
    {
        public int ID { get; set; }
        public string ShopName { get; set; }
        public Nullable<int> ProvinceID { get; set; }
        public Nullable<int> CityID { get; set; }
        public Nullable<int> AreaID { get; set; }
        public string ShopAddress { get; set; }
        public string LNG { get; set; }
        public string LAT { get; set; }
        public string ShopPhoneNum { get; set; }
        public string ShopContacts { get; set; }
        public string BuildNo { get; set; }
        public string FloorNo { get; set; }
        public string RoomNo { get; set; }
        public Nullable<byte> Flag { get; set; }

        public string ProvinceName;

        public string CityName;

        public string AreaName;
        
    }
}
