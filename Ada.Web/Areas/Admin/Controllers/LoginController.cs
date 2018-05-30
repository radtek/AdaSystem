using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Core;
using Ada.Core.Domain;
using Ada.Core.Domain.Admin;
using Ada.Core.Domain.Business;
using Ada.Core.Domain.Log;
using Ada.Core.Domain.Purchase;
using Ada.Core.Domain.Resource;
using Ada.Core.Infrastructure;
using Ada.Core.Tools;
using Ada.Core.ViewModel;
using Ada.Core.ViewModel.Admin;
using Ada.Core.ViewModel.Resource;
using Ada.Framework.Caching;
using Ada.Services.Admin;


namespace Admin.Controllers
{
    public class LoginController : Controller
    {
        private readonly IManagerService _service;
        private readonly ISignals _signals;
        public LoginController(IManagerService service,
            ISignals signals
            )
        {
            _service = service;
            _signals = signals;
        }
        public ActionResult Index()
        {
            if (Session["LoginManager"] != null)
            {
                return RedirectToAction("Index", "Home", new { area = "Dashboards" });
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(string userName, string password)
        {
            //校验用户
            var logModel = new LoginModel
            {
                LoginName = userName.Trim(),
                Password = password,
                LoginLog = new LoginLog() { UserAgent = Request.UserAgent }
            };
            var result = _service.Login(logModel);
            if (result == null)
            {
                ModelState.AddModelError("message", logModel.Message);
                return View();
            }

            Session["LoginManager"] = SerializeHelper.SerializeToString(result);
            //清空登陆日志缓存
            _signals.Trigger("LoginLog" + result.Id + ".Changed");
            return RedirectToAction("Index", "Home", new { area = "Dashboards" });

        }

        public ActionResult Binding()
        {
            //var guid = Guid.NewGuid().ToString("N");
            //Session["LoginOpenState"] = guid;
            //ViewBag.State = guid;
            //ViewBag.CallBack = Request.Url.Scheme + "://" + Request.Url.Authority + Url.Action("Open", "OAuth2", new{area="WeiXin"});
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Binding(string userName, string password)
        {
            //校验用户
            var obj = Session["CurrentWeiXinOpenUnionid"];
            if (obj == null)
            {
                return RedirectToAction("Index");
            }
            var logModel = new LoginModel
            {
                LoginName = userName.Trim(),
                Password = password,
                OpenId = obj.ToString(),
                LoginLog = new LoginLog() { UserAgent = Request.UserAgent }
            };
            var result = _service.BindingLogin(logModel);
            if (result == null)
            {
                ModelState.AddModelError("message", logModel.Message);
                return View();
            }
            Session["LoginManager"] = SerializeHelper.SerializeToString(result);
            //清空登陆日志缓存
            _signals.Trigger("LoginLog" + result.Id + ".Changed");
            return RedirectToAction("Index", "Home", new { area = "Dashboards" });

        }

        public ActionResult Quit()
        {
            Session.Abandon();
            Response.Cookies.Add(new HttpCookie("ASP.NET_SessionId", string.Empty) { HttpOnly = true });
            return RedirectToAction("Index", "Login");
        }



    }
}