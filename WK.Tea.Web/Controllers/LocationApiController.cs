using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WK.Tea.DataProvider.DAL;
using WK.Tea.DataProvider.IDAL;
using WK.Tea.Web.Common;
using WK.Tea.Web.Models;

namespace WK.Tea.Web.Controllers
{
    [RoutePrefix("api/location")]
    public class LocationApiController : ApiController
    {
        [Route("provinces")]
        public HttpResponseMessage GetProvinces()
        {
            ResultMsg resultMsg = new ResultMsg();
            try
            {

                using (IProvince repository = new ProvinceRepository())
                {
                    var list = repository.FindAll();
                    resultMsg.code = 0;
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

        [Route("cities")]
        public HttpResponseMessage GetCitiesByProvinceID(int provinceId)
        {
            ResultMsg resultMsg = new ResultMsg();
            try
            {
                string provinceCode = string.Empty;
                using (IProvince repository = new ProvinceRepository())
                {
                    provinceCode = repository.Find(provinceId).ProvinceCode;
                }
                using (ICity repository = new CityRepository())
                {
                    var list = repository.FindAll(o=>o.ProvinceCode == provinceCode);
                    resultMsg.code = 0;
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

        [Route("areas")]
        public HttpResponseMessage GetAreasByCityID(int cityId)
        {
            ResultMsg resultMsg = new ResultMsg();
            try
            {
                string cityCode = string.Empty;
                using (ICity repository = new CityRepository())
                {
                    cityCode = repository.Find(cityId).CityCode;
                }
                using (IArea repository = new AreaRepository())
                {
                    var list = repository.FindAll(o => o.CityCode == cityCode);
                    resultMsg.code = 0;
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
    }
}
