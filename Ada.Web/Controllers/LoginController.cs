using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Core.Tools;
using Ada.Core.ViewModel.Customer;
using Ada.Framework.Filter;
using Ada.Services.Cache;
using Ada.Services.Customer;
using Ada.Web.Models;
using Newtonsoft.Json;

namespace Ada.Web.Controllers
{
    public class LoginController : Controller
    {
        private readonly ILinkManService _linkManService;
        private readonly ICacheService _cacheService;
        public LoginController(ILinkManService linkManService, ICacheService cacheService)
        {
            _linkManService = linkManService;
            _cacheService = cacheService;
        }
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(LoginModel loginModel)
        {

            if (!ModelState.IsValid)
            {
                return View(loginModel);
            }

            var user = _linkManService.CheackUser(loginModel.LoginName, loginModel.PassWord);
            if (user == null)
            {
                ModelState.AddModelError("error", "用户名或密码有误！");
                return View(loginModel);
            }
            LinkManView viewModel = new LinkManView();
            viewModel.Id = user.Id;
            viewModel.CommpanyName = user.Commpany.Name;
            viewModel.LoginName = user.LoginName;
            viewModel.Phone = user.Phone;
            Session["User"] = SerializeHelper.SerializeToString(viewModel);
            return RedirectToAction("Index", "UserCenter");
        }
        [HttpPost]
        [AdaValidateAntiForgeryToken]
        public ActionResult GetSmsCode(string phone)
        {
            //验证手机号
            //验证是否频繁
            //生成随机码
            var code = Utils.RndomStr(5);
            //发送短信
            //返回结果
            return Json(new {State = 1, Msg = code});
        }
    }
}