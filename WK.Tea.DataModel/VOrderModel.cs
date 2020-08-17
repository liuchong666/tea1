using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WK.Tea.DataModel.SqlModel;

namespace WK.Tea.DataModel
{
    public class VOrderModel
    {
        public int ID { get; set; }
        public string Mobile { get; set; }
        public int ShopID { get; set; }
        public string ShopName { get; set; }
        public string ShopAddress { get; set; }
        public string ShopPhoneNum { get; set; }

        private DateTime bTime;
        public System.DateTime BTime
        {
            get
            {
                return DateTime.Parse(bTime.ToString("yyyy/MM/dd HH:mm"));
            }
            set
            {
                bTime = value;
            }
        }

        private DateTime eTime;
        public System.DateTime ETime
        {
            get
            {
                return DateTime.Parse(eTime.ToString("yyyy/MM/dd HH:mm"));
            }
            set
            {
                eTime = value;
            }
        }
        public int Duration { get; set; }
        public string Ver_Code { get; set; }
        public string OrderNo { get; set; }
        public string ParentNo { get; set; }
        public string CardNo { get; set; }
        public Nullable<System.DateTime> CTime { get; set; }
        public Nullable<decimal> FeeCode { get; set; }
        public Nullable<byte> ReMarks { get; set; }
        public Nullable<byte> Flag { get; set; }
        public byte PayStatus { get; set; }
        public string OP { get; set; }
        public string OpenID { get; set; }

    }
}
