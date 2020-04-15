using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WK.Tea.DataModel.SqlModel
{
    public partial class T_Admin
    {
        [Key]
        public string UserAccount { get; set; }
        public string UserPw { get; set; }
        public System.DateTime CTime { get; set; }
        public System.DateTime LTime { get; set; }
    }

    public partial class T_User
    {
        [Key]
        public string UserAccount { get; set; }
        public string UserName { get; set; }
        public string User_Pw { get; set; }
        public string OpenID { get; set; }
        public string Mobile { get; set; }
        public string Balance { get; set; }
        public string Points { get; set; }
        public System.DateTime CTime { get; set; }
    }
}
