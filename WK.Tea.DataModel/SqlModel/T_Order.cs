//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace WK.Tea.DataModel.SqlModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class T_Order
    {
        [Key]
        public int ID { get; set; }
        public string Mobile { get; set; }
        public int ShopID { get; set; }
        public System.DateTime BTime { get; set; }
        public System.DateTime ETime { get; set; }
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

        public string LockPW { get; set; }
        public string LockURL { get; set; }
    }
}
