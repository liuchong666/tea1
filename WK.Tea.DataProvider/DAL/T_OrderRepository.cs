using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using WK.Tea.DataModel;
using WK.Tea.DataModel.SqlModel;
using WK.Tea.DataProvider.IDAL;

namespace WK.Tea.DataProvider.DAL
{
    public class T_OrderRepository : BaseRepository<TeaDbContext, T_Order>, IT_Order
    {
        public bool AddOrder(T_Order order)
        {
            return context.C_OrderNo_Add(order) > 0;
        }

        public bool CheckOrderTime(T_Order order)
        {

            if (order.ID != 0)
            {
                return context.T_Order.Any(o => o.ID != order.ID && o.ShopID == order.ShopID && (o.BTime <= order.BTime && o.ETime >= order.BTime || o.BTime <= order.ETime && o.ETime >= order.ETime));
            }
            else
            {
                return context.T_Order.Any(o => o.ShopID == order.ShopID && (o.BTime <= order.BTime && o.ETime >= order.BTime || o.BTime <= order.ETime && o.ETime >= order.ETime));
            }
        }

        public PagedList<VOrderModel> GetVOrderPageList(OrderSearchModel search)
        {
            var queryList = from o in context.T_Order
                            join s in context.T_Shop on o.ShopID.ToString() equals s.ID.ToString()
                            select new VOrderModel
                            {
                                ID = o.ID,
                                Mobile = o.Mobile,
                                ShopID = o.ShopID,
                                ShopName = s.ShopName,
                                ShopAddress = s.ShopAddress,
                                ShopPhoneNum = s.ShopPhoneNum,
                                BTime = o.BTime,
                                ETime = o.ETime,
                                Duration = o.Duration,
                                Ver_Code = o.Ver_Code,
                                OrderNo = o.OrderNo,
                                ParentNo = o.ParentNo,
                                CardNo = o.CardNo,
                                CTime = o.CTime,
                                FeeCode = o.FeeCode,
                                ReMarks = o.ReMarks,
                                Flag = o.Flag,
                                PayStatus = o.PayStatus
                            };

            if (search.ShopID.HasValue)
            {
                queryList = queryList.Where(o => o.ShopID == search.ShopID);
            }
            if (search.BTimeStart.HasValue)
            {
                queryList = queryList.Where(o => o.BTime >= search.BTimeStart);
            }
            if (search.BTimeEnd.HasValue)
            {
                queryList = queryList.Where(o => o.BTime < search.BTimeEnd);
            }

            queryList = queryList.OrderByDescending(o => o.BTime);
            return queryList.ToPagedList(search.page, search.limit);
        }

        public List<OrderReportModel> GetOrderReportList(string tjWay, string where, byte? sort, string op)
        {
            return context.C_Stat(tjWay, where, sort, op).ToList();
        }

        public List<DateTime> GetOrderTimeList(int shopId, DateTime dateTime)
        {
            return context.Pro_GetOrderTimeByDate(shopId, dateTime).ToList();
        } 
        
        public List<OrderStatics> GetOrderStatics()
        {
            return context.C_GetOrderTotalDate().ToList();
        } 
        
        public string GetOrderStaticsData()
        {
            return context.C_GetOrderDate().ToList()[0];
        }

        public VOrderModel GetVOrderByID(int id)
        {
            var query = from o in context.T_Order
                            join s in context.T_Shop on o.ShopID.ToString() equals s.ID.ToString()
                            select new VOrderModel
                            {
                                ID = o.ID,
                                Mobile = o.Mobile,
                                ShopID = o.ShopID,
                                ShopName = s.ShopName,
                                BTime = o.BTime,
                                ETime = o.ETime,
                                Duration = o.Duration,
                                Ver_Code = o.Ver_Code,
                                OrderNo = o.OrderNo,
                                ParentNo = o.ParentNo,
                                CardNo = o.CardNo,
                                CTime = o.CTime,
                                FeeCode = o.FeeCode,
                                ReMarks = o.ReMarks,
                                Flag = o.Flag,
                                PayStatus = o.PayStatus
                            };

            return query.FirstOrDefault(o => o.ID == id);
        }
    }
}
