using WK.Tea.DataModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WK.Tea.DataModel.SqlModel;
using System.Data.SqlClient;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Core.Objects;

namespace WK.Tea.DataProvider
{
    public class TeaDbContext : DbContext
    {
        public TeaDbContext()
            : base("name=TeaDbContext")
        {
            this.Configuration.ProxyCreationEnabled = false;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //解决EF动态建库数据库表名变为复数问题
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            //throw new UnintentionalCodeFirstException();
        }

        public virtual DbSet<Area> Area { get; set; }
        public virtual DbSet<City> City { get; set; }
        public virtual DbSet<Province> Province { get; set; }
        public virtual DbSet<T_Shop> T_Shop { get; set; }
        public virtual DbSet<T_Order> T_Order { get; set; }
        public virtual DbSet<T_Admin> T_Admin { get; set; }
        public virtual DbSet<T_User> T_User { get; set; }

        public virtual int C_Shop_Add(T_Shop shop)
        {
            var shopNameParameter = shop.ShopName != null ?
                new SqlParameter("ShopName", shop.ShopName) :
                new SqlParameter("ShopName", Convert.DBNull);

            var provinceIDParameter = shop.ProvinceID.HasValue ?
                new SqlParameter("ProvinceID", shop.ProvinceID) :
                new SqlParameter("ProvinceID", Convert.DBNull);

            var cityIDParameter = shop.CityID.HasValue ?
                new SqlParameter("CityID", shop.CityID) :
                new SqlParameter("CityID", Convert.DBNull);

            var areaIDParameter = shop.AreaID.HasValue ?
                new SqlParameter("AreaID", shop.AreaID) :
                new SqlParameter("AreaID", Convert.DBNull);

            var shopAddressParameter = shop.ShopAddress != null ?
                new SqlParameter("ShopAddress", shop.ShopAddress) :
                new SqlParameter("ShopAddress", Convert.DBNull);

            var lNGParameter = shop.LNG != null ?
                new SqlParameter("LNG", shop.LNG) :
                new SqlParameter("LNG", Convert.DBNull);

            var lATParameter = shop.LAT != null ?
                new SqlParameter("LAT", shop.LAT) :
                new SqlParameter("LAT", Convert.DBNull);

            var shopPhoneNumParameter = shop.ShopPhoneNum != null ?
                new SqlParameter("ShopPhoneNum", shop.ShopPhoneNum) :
                new SqlParameter("ShopPhoneNum", Convert.DBNull);

            var shopContactsParameter = shop.ShopContacts != null ?
                new SqlParameter("ShopContacts", shop.ShopContacts) :
                new SqlParameter("ShopContacts", Convert.DBNull);

            var buildNoParameter = shop.BuildNo != null ?
                new SqlParameter("BuildNo", shop.BuildNo) :
                new SqlParameter("BuildNo", Convert.DBNull);

            var floorNoParameter = shop.FloorNo != null ?
                new SqlParameter("FloorNo", shop.FloorNo) :
                new SqlParameter("FloorNo", Convert.DBNull);

            var roomNoParameter = shop.RoomNo != null ?
                new SqlParameter("RoomNo", shop.RoomNo) :
                new SqlParameter("RoomNo", Convert.DBNull);

            var flagParameter = shop.Flag.HasValue ?
                new SqlParameter("Flag", shop.Flag) :
                new SqlParameter("Flag", Convert.DBNull);

            var cTimeParameter = shop.CTime.HasValue ?
                new SqlParameter("CTime", shop.CTime) :
                new SqlParameter("CTime", Convert.DBNull);

            return ((IObjectContextAdapter)this).ObjectContext.ExecuteStoreCommand("C_Shop_Add @ShopName,@ProvinceID,@CityID,@AreaID,@ShopAddress,@LNG,@LAT,@ShopPhoneNum,@ShopContacts,@BuildNo,@FloorNo,@RoomNo,@Flag,@CTime", 
                shopNameParameter, provinceIDParameter, cityIDParameter, areaIDParameter, shopAddressParameter, lNGParameter, lATParameter, shopPhoneNumParameter, shopContactsParameter, buildNoParameter, floorNoParameter, roomNoParameter, flagParameter, cTimeParameter);
        }

        public virtual int C_Shop_Edit(T_Shop shop)
        {
            var idParameter = new SqlParameter("ID", shop.ID);

            var shopNameParameter = shop.ShopName != null ?
                new SqlParameter("ShopName", shop.ShopName) :
                new SqlParameter("ShopName", Convert.DBNull);

            var provinceIDParameter = shop.ProvinceID.HasValue ?
                new SqlParameter("ProvinceID", shop.ProvinceID) :
                new SqlParameter("ProvinceID", Convert.DBNull);

            var cityIDParameter = shop.CityID.HasValue ?
                new SqlParameter("CityID", shop.CityID) :
                new SqlParameter("CityID", Convert.DBNull);

            var areaIDParameter = shop.AreaID.HasValue ?
                new SqlParameter("AreaID", shop.AreaID) :
                new SqlParameter("AreaID", Convert.DBNull);

            var shopAddressParameter = shop.ShopAddress != null ?
                new SqlParameter("ShopAddress", shop.ShopAddress) :
                new SqlParameter("ShopAddress", Convert.DBNull);

            var lNGParameter = shop.LNG != null ?
                new SqlParameter("LNG", shop.LNG) :
                new SqlParameter("LNG", Convert.DBNull);

            var lATParameter = shop.LAT != null ?
                new SqlParameter("LAT", shop.LAT) :
                new SqlParameter("LAT", Convert.DBNull);

            var shopPhoneNumParameter = shop.ShopPhoneNum != null ?
                new SqlParameter("ShopPhoneNum", shop.ShopPhoneNum) :
                new SqlParameter("ShopPhoneNum", Convert.DBNull);

            var shopContactsParameter = shop.ShopContacts != null ?
                new SqlParameter("ShopContacts", shop.ShopContacts) :
                new SqlParameter("ShopContacts", Convert.DBNull);

            var buildNoParameter = shop.BuildNo != null ?
                new SqlParameter("BuildNo", shop.BuildNo) :
                new SqlParameter("BuildNo", Convert.DBNull);

            var floorNoParameter = shop.FloorNo != null ?
                new SqlParameter("FloorNo", shop.FloorNo) :
                new SqlParameter("FloorNo", Convert.DBNull);

            var roomNoParameter = shop.RoomNo != null ?
                new SqlParameter("RoomNo", shop.RoomNo) :
                new SqlParameter("RoomNo", Convert.DBNull);

            var flagParameter = shop.Flag.HasValue ?
                new SqlParameter("Flag", shop.Flag) :
                new SqlParameter("Flag", Convert.DBNull);

            var cTimeParameter = shop.CTime.HasValue ?
                new SqlParameter("CTime", shop.CTime) :
                new SqlParameter("CTime", Convert.DBNull);

            return ((IObjectContextAdapter)this).ObjectContext.ExecuteStoreCommand(@"C_Shop_Edit @ID,@ShopName,@ProvinceID,
                @CityID,@AreaID,@ShopAddress,@LNG,@LAT,@ShopPhoneNum,@ShopContacts,@BuildNo,@FloorNo,@RoomNo,@Flag,@CTime",
                idParameter, shopNameParameter, provinceIDParameter, cityIDParameter, areaIDParameter, shopAddressParameter, 
                lNGParameter, lATParameter, shopPhoneNumParameter, shopContactsParameter, buildNoParameter, floorNoParameter, 
                roomNoParameter, flagParameter, cTimeParameter);
        }

        public virtual int C_OrderNo_Add(T_Order order)
        {

            var mobileParameter = order.Mobile != null ?
                new SqlParameter("Mobile", order.Mobile) :
                new SqlParameter("Mobile", Convert.DBNull);

            var shopIDParameter = new SqlParameter("ShopID", order.ShopID);

            var bTimeParameter = new SqlParameter("BTime", order.BTime);

            var durationParameter = new SqlParameter("Duration", order.Duration);

            var ver_CodeParameter = order.Ver_Code != null ?
                new SqlParameter("Ver_Code", order.Ver_Code) :
                new SqlParameter("Ver_Code", Convert.DBNull);

            var orderNoParameter = order.OrderNo != null ?
                new SqlParameter("OrderNo", order.OrderNo) :
                new SqlParameter("OrderNo", Convert.DBNull);

            var parentNoParameter = order.ParentNo != null ?
                new SqlParameter("ParentNo", order.ParentNo) :
                new SqlParameter("ParentNo", Convert.DBNull);

            var cardNoParameter = order.CardNo != null ?
                new SqlParameter("CardNo", order.CardNo) :
                new SqlParameter("CardNo", Convert.DBNull);

            var feeCodeParameter = order.FeeCode != null ?
                new SqlParameter("FeeCode", order.FeeCode) :
                new SqlParameter("FeeCode", Convert.DBNull);

            var reMarksParameter = order.ReMarks != null ?
                new SqlParameter("ReMarks", order.ReMarks) :
                new SqlParameter("ReMarks", Convert.DBNull);

            var flagParameter = order.Flag.HasValue ?
                new SqlParameter("Flag", order.Flag) :
                new SqlParameter("Flag", Convert.DBNull);

            var payStatusParameter = new SqlParameter("PayStatus", order.PayStatus);

            var opParameter = order.OP != null ?
                new SqlParameter("Op", order.OP) :
                new SqlParameter("Op", Convert.DBNull);

            return ((IObjectContextAdapter)this).ObjectContext.ExecuteStoreCommand(@"C_OrderNo_Add @Mobile,@ShopID,@BTime,@Duration,
                                @Ver_Code,@OrderNo,@ParentNo,@CardNo,@FeeCode,@ReMarks,@Flag,@PayStatus,@Op",
                                mobileParameter, shopIDParameter, bTimeParameter, durationParameter, ver_CodeParameter, orderNoParameter, 
                                parentNoParameter, cardNoParameter, feeCodeParameter, reMarksParameter, flagParameter, payStatusParameter, opParameter);
        }

        public virtual ObjectResult<OrderReportModel> C_Stat(string tjWay, string where, byte? sort, string op)
        {

            var tjWayParameter = new SqlParameter("TjWay", tjWay);

            var whereParameter = where != null ?
                new SqlParameter("Where", where) :
                new SqlParameter("Where", "1=1");

            var sortParameter = sort != null ?
                new SqlParameter("Sort", sort) :
                new SqlParameter("Sort", 1);

            var opParameter = new SqlParameter("Op", op);

            return ((IObjectContextAdapter)this).ObjectContext.ExecuteStoreQuery<OrderReportModel>(@"C_Stat @TjWay,@Where,@Sort,@Op",
                                tjWayParameter, whereParameter, sortParameter, opParameter);
        }

        public virtual ObjectResult<DateTime> Pro_GetOrderTimeByDate(int shopID, DateTime dateTime)
        {
            var shopIDParameter = new SqlParameter("ShopID", shopID);
            
            var dateTimeParameter = new SqlParameter("DateTime", dateTime);
            
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteStoreQuery<DateTime>(@"Pro_GetOrderTimeByDate @ShopID,@DateTime",
                                shopIDParameter, dateTimeParameter);
        }
        
        public virtual ObjectResult<OrderStatics> C_GetOrderTotalDate()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteStoreQuery<OrderStatics>(@"C_GetOrderTotalDate");
        }

        public virtual ObjectResult<string> C_GetOrderDate()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteStoreQuery<string>(@"C_GetOrderDate");
        }
    }
}
