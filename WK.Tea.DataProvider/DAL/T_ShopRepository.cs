using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WK.Tea.DataModel;
using WK.Tea.DataModel.SqlModel;
using WK.Tea.DataProvider.IDAL;

namespace WK.Tea.DataProvider.DAL
{
    public class T_ShopRepository : BaseRepository<TeaDbContext, T_Shop>, IT_Shop
    {
        public bool AddShop(T_Shop shop)
        {
            return context.C_Shop_Add(shop) > 0;
        }

        public bool EditShop(T_Shop shop)
        {
            return context.C_Shop_Edit(shop) > 0;
        }

        public PagedList<VShopModel> GetVShopPageList(int pageindex, int pagesize)
        {
            var queryList = from s in context.T_Shop
                       join p in context.Province on s.ProvinceID.ToString() equals p.ProvinceID.ToString()
                       join c in context.City on s.CityID.ToString() equals c.CityID.ToString()
                       join a in context.Area on s.AreaID.ToString() equals a.AreaID.ToString()
                       select new VShopModel
                       {
                           ID = s.ID,
                           ShopName = s.ShopName,
                           ProvinceID = s.ProvinceID,
                           ProvinceName = p.ProvinceName,
                           CityID = s.CityID,
                           CityName = c.CityName,
                           AreaID = s.AreaID,
                           AreaName = a.AreaName,
                           ShopAddress = s.ShopAddress,
                           ShopContacts = s.ShopContacts,
                           ShopPhoneNum = s.ShopPhoneNum,
                           Flag = s.Flag,
                           BuildNo = s.BuildNo,
                           FloorNo = s.FloorNo,
                           RoomNo = s.RoomNo,
                           LAT = s.LAT,
                           LNG = s.LNG
                       };


            queryList = queryList.OrderBy(o => o.ID);
            return queryList.ToPagedList(pageindex, pagesize);
        }

        public VShopModel GetVShopByID(int id)
        {
            var query = from s in context.T_Shop
                            join p in context.Province on s.ProvinceID.ToString() equals p.ProvinceID.ToString()
                            join c in context.City on s.CityID.ToString() equals c.CityID.ToString()
                            join a in context.Area on s.AreaID.ToString() equals a.AreaID.ToString()
                            select new VShopModel
                            {
                                ID = s.ID,
                                ShopName = s.ShopName,
                                ProvinceID = s.ProvinceID,
                                ProvinceName = p.ProvinceName,
                                CityID = s.CityID,
                                CityName = c.CityName,
                                AreaID = s.AreaID,
                                AreaName = a.AreaName,
                                ShopAddress = s.ShopAddress,
                                ShopContacts = s.ShopContacts,
                                ShopPhoneNum = s.ShopPhoneNum,
                                Flag = s.Flag,
                                BuildNo = s.BuildNo,
                                FloorNo = s.FloorNo,
                                RoomNo = s.RoomNo,
                                LAT = s.LAT,
                                LNG = s.LNG
                            };
            
            return query.FirstOrDefault(o=>o.ID == id);
        }
    }
}
