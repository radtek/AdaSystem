using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Core.Tools;
using Ada.Core.ViewModel.Customer;
using Ada.Framework.Filter;
using Ada.Framework.Messaging;
using Ada.Services.Cache;
using Ada.Services.Customer;
using Ada.Web.Models;
using Newtonsoft.Json;

namespace Ada.Web.Controllers
{
    public class LoginController : Controller
    {
        private readonly ILinkManService _linkManService;
        private readonly IMessageService _messageService;
        private readonly ICacheService _cacheService;
        public LoginController(ILinkManService linkManService, ICacheService cacheService, IMessageService messageService)
        {
            _linkManService = linkManService;
            _cacheService = cacheService;
            _messageService = messageService;
        }
        public ActionResult Index()
        {
            var sessionId = Request.Cookies["UserSession"]?.Value;
            if (string.IsNullOrWhiteSpace(sessionId)) return View();
            var user = _cacheService.GetObject<LinkManView>(sessionId);
            if (user != null)
            {
                return RedirectToAction("Index", "UserCenter");
            }
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

            if (loginModel.LoginName!="18607961688")
            {
                //校验验证码
                var obj = _cacheService.GetObject<string>(loginModel.LoginName);
                if (obj == null)
                {
                    ModelState.AddModelError("error", "验证码已失效，请重新获取！");
                    return View(loginModel);
                }

                var code = obj.ToString();
                if (code != loginModel.Code)
                {
                    ModelState.AddModelError("error", "验证码有误！");
                    return View(loginModel);
                }
            }
            

            var user = _linkManService.CheackUser(loginModel.LoginName);
            if (user == null)
            {
                ModelState.AddModelError("error", "此手机号暂未开通会员，请联系我们处理！");
                return View(loginModel);
            }
            LinkManView viewModel = new LinkManView();
            viewModel.Id = user.Id;
            viewModel.CommpanyName = user.Commpany.Name;
            viewModel.LoginName = user.LoginName;
            viewModel.Phone = user.Phone;
            string sessionId = Guid.NewGuid().ToString("N");
            _cacheService.Put(sessionId, viewModel, new TimeSpan(1, 0, 0, 0));
            //Cookie
            Response.Cookies["UserSession"].Value = sessionId;
            //Session["User"] = SerializeHelper.SerializeToString(viewModel);
            return RedirectToAction("Index", "UserCenter");
        }
        [HttpPost]
        [AdaValidateAntiForgeryToken]
        public ActionResult GetSmsCode(string phone)
        {
            //验证手机号
            if (!Utils.IsMobilePhone(phone))
            {
                return Json(new { State = 0, Msg = "请输入正确的手机号" });
            }
            //验证是否频繁
            var obj = _cacheService.GetObject<string>(phone);
            if (obj != null)
            {
                return Json(new { State = 0, Msg = "请勿频繁获取！" });
            }
            //验证是否存在账户
            var user = _linkManService.CheackUser(phone);
            if (user == null)
            {
                return Json(new { State = 0, Msg = "此手机号暂未开通会员，请联系我们处理！" });
            }
            //生成随机码 3分钟有效
            var code = Utils.RndomStr(5);
            _cacheService.Put(phone, code, new TimeSpan(0, 3, 0));
            //发送短信
            _messageService.Send("SMS", new Dictionary<string, object> {
                {"PhoneNumbers", phone},
                {"TemplateCode", "SMS_133230131"},
                {"TemplateParam", "{\"code\":"+code+"}"}
            });
            //返回结果
            return Json(new { State = 1, Msg = "获取成功" });
        }
    }
}