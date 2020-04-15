using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WK.Tea.Lock.ApiRequest;
using WK.Tea.Web.Auth;

namespace WK.Tea.Web.Areas.Admin.Controllers
{
    public class HomeController : Controller
    {
        // GET: Admin/Home
        [TeaAuthorize]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Layout()
        {
            return View();
        }

        public ActionResult Default()
        {
            return View();
        }

        /// <summary>
        /// 获取房卡示例
        /// </summary>
        public void Qrcode()
        {
            CreateCardRequest postEntity = new CreateCardRequest
            {
                communityNo = "1316882760",
                roomNo = "2108",
                floorNo = "021",
                buildNo = "001",
                startTime = "1911172300",
                endTime = "1911172330",
                mobile = "18942040358"
            };

            CreateCardResponse result = LockApiHelper.WebApi.Post<CreateCardRequest, CreateCardResponse>("https://api.uclbrt.com/?c=Qrcode&a=getLink", postEntity);



            string data = LockApiHelper.WebApi.GetCardDataParams("1316882760", "18942040358", "86", result.cardNo) ;
            // string data = ucl.get("1316879945", "18819238549","86","OxdpqKVrj4VrL2gZ");
            Response.Redirect("http://cz.uclbrt.com/apiLogin?data=" + data);
        }

        public void WeixinMsg()
        {
            
            //SendTemplateMsgRequest param = new SendTemplateMsgRequest();
            //param.template_id = "io6vUKjeXxWuuytiAdmRDejo-OAxNrelKNqYj2F3Lhc";
            //param.touser = "ofrrft9CqsS4iZrjQ_atRg_v9R9Q";
            //param.url = "http://weixin.qq.com/download";
            //dynamic mobile = new System.Dynamic.ExpandoObject();
            //mobile.value = "18942040358";
            //dynamic startTime = new System.Dynamic.ExpandoObject();
            //startTime.value = "2019/11/8 16:00";
            //dynamic endTime = new System.Dynamic.ExpandoObject();
            //endTime.value = "2019/11/8 18:00";
            //param.data = new System.Dynamic.ExpandoObject();
            //param.data.mobile = mobile;
            //param.data.startTime = startTime;
            //param.data.endTime = endTime;
            //var result = WeixinApi.SendTemplateMsg(param);
            //// string data = ucl.get("1316879945", "18819238549","86","OxdpqKVrj4VrL2gZ");
            //Response.Write(JsonConvert.SerializeObject(result));
        }
    }
}