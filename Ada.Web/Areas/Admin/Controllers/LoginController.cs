using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Ada.Core.Tools;
using Ada.Core.ViewModel;
using Ada.Core.ViewModel.Setting;
using Ada.Framework.Caching;
using Ada.Framework.Messaging;
using Ada.Services.Admin;
using Ada.Services.Cache;
using Ada.Services.Setting;


namespace Admin.Controllers
{
    public class LoginController : Controller
    {
        private readonly IManagerService _service;
        private readonly ISignals _signals;
        private readonly IMessageService _messageService;
        private readonly ICacheService _cacheService;
        private readonly ISettingService _settingService;
        public LoginController(IManagerService service,
            ISignals signals,
            ICacheService cacheService,
            IMessageService messageService,
            ISettingService settingService
            )
        {
            _service = service;
            _signals = signals;
            _cacheService = cacheService;
            _messageService = messageService;
            _settingService = settingService;
        }
        public ActionResult Index()
        {
            if (Session["LoginManager"] != null)
            {
                return RedirectToAction("Index", "Home", new { area = "Dashboards" });
            }
            //判断是否是手机浏览器
            var isPhoneBroise = Regex.IsMatch(Request.UserAgent, "(iPhone|iPad|iPod|iOS|Android)", RegexOptions.IgnoreCase);
            if (isPhoneBroise)
            {
                return View("Phone");
            }
            var isWxLogin = _settingService.GetSetting<Site>().SystemIsWeiXinLogin;
            if (isWxLogin)
            {
                var guid = Guid.NewGuid().ToString("N");
                Session["LoginOpenState"] = guid;
                ViewBag.State = guid;
                ViewBag.CallBack = Request.Url.Scheme + "://" + Request.Url.Authority + Url.Action("Open", "OAuth2", new { area = "WeiXin" });
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

        public ActionResult Phone()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Phone(LoginPhoneModel loginModel)
        {
            if (!ModelState.IsValid)
            {
                return View(loginModel);
            }
            //校验验证码
            var obj = _cacheService.GetObject<string>(loginModel.Phone.Trim());
            if (obj == null)
            {
                ModelState.AddModelError("error", "验证码无效！");
                return View(loginModel);
            }

            var code = obj.ToString();
            if (code != loginModel.Code.Trim())
            {
                ModelState.AddModelError("error", "验证码错误！");
                return View(loginModel);
            }
            //校验用户
            var logModel = new LoginModel
            {
                Phone = loginModel.Phone.Trim(),
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

        public ActionResult SmsCode(string phone)
        {
            //验证手机号
            if (!Utils.IsMobilePhone(phone.Trim()))
            {
                return Json(new { State = 0, Msg = "请输入正确的手机号" }, JsonRequestBehavior.AllowGet);
            }
            //验证是否频繁
            var obj = _cacheService.GetObject<string>(phone);
            if (obj != null)
            {
                return Json(new { State = 0, Msg = "请勿频繁获取！" }, JsonRequestBehavior.AllowGet);
            }
            //验证是否存在账户
            var user = _service.GetMangerByPhone(phone.Trim());
            if (user == null)
            {
                return Json(new { State = 0, Msg = "此手机号暂未绑定账户，请联系管理员！" }, JsonRequestBehavior.AllowGet);
            }
            //生成随机码 3分钟有效
            RandomHelper random = new RandomHelper();
            var code = random.GenerateCheckCodeNum(5);
            _cacheService.Put(phone, code, new TimeSpan(0, 3, 0));
            //发送短信
            _messageService.Send("SMS", new Dictionary<string, object> {
                {"PhoneNumbers", phone},
                {"TemplateCode", "SMS_133230131"},
                {"TemplateParam", "{\"code\":\""+code+"\"}"}
            });
            //返回结果
            return Json(new { State = 1, Msg = "获取成功" }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Binding()
        {
            
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

    public class LoginPhoneModel
    {
        [Required(ErrorMessage = "请输入您的手机号")]
        public string Phone { get; set; }
        //[Required(ErrorMessage = "请输入登陆密码")]
        public string PassWord { get; set; }
        [Required(ErrorMessage = "请输入手机验证码")]
        public string Code { get; set; }
    }
}