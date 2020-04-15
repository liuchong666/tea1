using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WK.Tea.DataModel;
using WK.Tea.DataModel.SqlModel;

namespace WK.Tea.DataProvider.IDAL
{
    public interface IT_Shop : IRepository<T_Shop>
    {
        bool AddShop(T_Shop shop);
        bool EditShop(T_Shop shop);

        PagedList<VShopModel> GetVShopPageList(int pageindex, int pagesize);

        VShopModel GetVShopByID(int id);
    }
}
