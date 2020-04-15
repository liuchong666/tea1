using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WK.Tea.Web.Auth;
using WK.Tea.Web.Models;
using WK.Tea.DataProvider.DAL;
using WK.Tea.DataProvider.IDAL;

namespace WK.Tea.Web.Areas.Admin.Controllers
{
    public class AccountController : Controller
    {
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            if (string.IsNullOrWhiteSpace(model.UserName) || string.IsNullOrWhiteSpace(model.Password))
            {
                ModelState.AddModelError("", "请输入用户名和密码");
                return View(model);
            }
            string msg = string.Empty;
            using (IT_Admin repository = new T_AdminRepository())
            {
                if (!repository.CheckUserAndPwd(model.UserName, model.Password))
                {
                    ModelState.AddModelError("", "用户名或密码错误");
                    return View(model);
                }
            }

            model.Roles = "Admin";
            FormsAuthHelper.AddFormsAuthCookie(model.UserName, model, 0);//设置ticket票据的名称为用户的id，设置有效时间为60分钟

            return Redirect(returnUrl ?? "~/Admin");
        }

        [TeaAuthorize]
        public ActionResult Logout(string returnUrl)
        {
            FormsAuthHelper.RemoveFormsAuthCookie();

            return Redirect(returnUrl ?? "~/Admin");
        }
    }
}