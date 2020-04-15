using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WK.Tea.DataModel.SqlModel;
using WK.Tea.DataProvider.DAL;
using WK.Tea.DataProvider.IDAL;
using WK.Tea.Web.Common;
using WK.Tea.Web.Models;

namespace WK.Tea.Web.Areas.Api.Controllers
{
    [RoutePrefix("api/shop")]
    public class ShopController : ApiController
    {
        [Route("pagelist")]
        public HttpResponseMessage GetShopList(int limit, int page)
        {
            ResultMsg resultMsg = new ResultMsg();
            try
            {
                if (limit < 1)
                    limit = 10;
                if (page < 1)
                    page = 1;

                using (IT_Shop repository = new T_ShopRepository())
                {
                    var list = repository.GetVShopPageList(page, limit);
                    resultMsg.data = list.Data;
                    resultMsg.count = list.TotalItemCount;
                }

            }
            catch (Exception ex)
            {
                resultMsg.code = (int)StatusCodeEnum.Error;
                resultMsg.msg = ex.Message;
            }
            return resultMsg.toJson();
        }

        [Route("list")]
        public HttpResponseMessage GetShopList()
        {
            ResultMsg resultMsg = new ResultMsg();
            try
            {
                using (IT_Shop repository = new T_ShopRepository())
                {
                    var list = repository.FindAll();
                    resultMsg.data = list;
                }

            }
            catch (Exception ex)
            {
                resultMsg.code = (int)StatusCodeEnum.Error;
                resultMsg.msg = ex.Message;
            }
            return resultMsg.toJson();
        }

        [Route("add")]
        public HttpResponseMessage AddShop([FromBody] T_Shop shop)
        {
            ResultMsg resultMsg = new ResultMsg();
            try
            {
                using (IT_Shop repository = new T_ShopRepository())
                {
                    if (!repository.AddShop(shop))
                    {
                        resultMsg.code = 1;
                        resultMsg.msg = "添加失败";
                    }
                }
            }
            catch (Exception ex)
            {
                resultMsg.code = (int)StatusCodeEnum.Error;
                resultMsg.msg = ex.Message;
            }
            return resultMsg.toJson();
        }

        [Route("edit")]
        public HttpResponseMessage EditShop([FromBody] T_Shop shop)
        {
            ResultMsg resultMsg = new ResultMsg();
            try
            {
                using (IT_Shop repository = new T_ShopRepository())
                {
                    if (!repository.EditShop(shop))
                    {
                        resultMsg.code = 1;
                        resultMsg.msg = "修改失败";
                    }
                }
            }
            catch (Exception ex)
            {
                resultMsg.code = (int)StatusCodeEnum.Error;
                resultMsg.msg = ex.Message;
            }
            return resultMsg.toJson();
        }

        [HttpPost]
        [Route("{id}/delete")]
        public HttpResponseMessage DeleteShop(short id)
        {
            ResultMsg resultMsg = new ResultMsg();
            try
            {
                using (IT_Shop repository = new T_ShopRepository())
                {
                    T_Shop shop = new T_Shop { ID = id };
                    repository.Delete(shop);
                }
            }
            catch (Exception ex)
            {
                resultMsg.code = (int)StatusCodeEnum.Error;
                resultMsg.msg = ex.Message;
            }
            return resultMsg.toJson();
        }


        [Route("{id}")]
        public HttpResponseMessage GetShopByID(int id)
        {
            ResultMsg resultMsg = new ResultMsg();
            try
            {
                using (IT_Shop repository = new T_ShopRepository())
                {
                    var detail = repository.GetVShopByID(id);
                    resultMsg.data = detail;
                }

            }
            catch (Exception ex)
            {
                resultMsg.code = (int)StatusCodeEnum.Error;
                resultMsg.msg = ex.Message;
            }
            return resultMsg.toJson();
        }
    }
}
