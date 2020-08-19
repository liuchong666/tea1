using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using WK.Tea.DataModel;
using WK.Tea.DataModel.SqlModel;

namespace WK.Tea.DataProvider.IDAL
{
    public interface IT_Order : IRepository<T_Order>
    {
        bool AddOrder(T_Order order);

        bool CheckOrderTime(T_Order order);

        PagedList<VOrderModel> GetVOrderPageList(OrderSearchModel search);

        List<OrderReportModel> GetOrderReportList(string tjWay, string where, byte? sort, string op);

        List<DateTime> GetOrderTimeList(int shopId, DateTime dateTime);

        VOrderModel GetVOrderByID(int id);

        VOrderModel GetVOrderByOrderNo(string orderNo);

        List<OrderStatics> GetOrderStatics();

        string GetOrderStaticsData();

        int C_SendSSM(string mobile, string msg);
    }
}
