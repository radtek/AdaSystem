using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Core;
using Ada.Core.Domain.Customer;
using Ada.Core.Tools;
using Ada.Core.ViewModel.Customer;
using Ada.Core.ViewModel.Setting;
using Ada.Framework.Filter;
using Ada.Framework.Messaging;
using Ada.Services.Cache;
using Ada.Services.Customer;
using Ada.Services.Setting;
using Ada.Web.Models;
using Newtonsoft.Json;

namespace Ada.Web.Controllers
{
    [UserException, Compress, WebStatus]
    public class LoginController : Controller
    {
        private readonly ILinkManService _linkManService;
        private readonly IMessageService _messageService;
        private readonly ICacheService _cacheService;
        private readonly ISettingService _settingService;
        public LoginController(ILinkManService linkManService,
            ICacheService cacheService, 
            IMessageService messageService,
            ISettingService settingService)
        {
            _linkManService = linkManService;
            _cacheService = cacheService;
            _messageService = messageService;
            _settingService = settingService;
        }
        [NoCache]
        public ActionResult Index()
        {
            var sessionId = Request.Cookies["UserSession"]?.Value;
            if (string.IsNullOrWhiteSpace(sessionId)) return View();
            var user = _cacheService.GetObject<LinkManView>(sessionId);
            if (user != null)
            {
                return RedirectToAction("Order", "UserCenter");
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
            var user = _linkManService.CheackUser(loginModel.LoginName);
            if (user == null)
            {
                ModelState.AddModelError("error", "此手机号暂未开通会员，请联系我们处理！");
                return View(loginModel);
            }

            var demo = _settingService.GetSetting<WeiGuang>();
            if (loginModel.LoginName != demo.UserDemo)
            {
                //校验验证码
                var obj = _cacheService.GetObject<string>(loginModel.LoginName.Trim());
                if (obj == null)
                {
                    ModelState.AddModelError("error", "验证码无效！");
                    return View(loginModel);
                }

                var code = obj.ToString();
                if (code != loginModel.Code.Trim())
                {
                    //会员登陆日志
                    user.FollowUps.Add(new FollowUp()
                    {
                        Id = IdBuilder.CreateIdNum(),
                        IpAddress = Utils.GetIpAddress(),
                        Content = Request.UserAgent,
                        NextTime = DateTime.Now,
                        FollowUpWay = "失败，ErrorCode：" + loginModel.Code + "，Code:" + code
                    });
                    _linkManService.Update(user);
                    ModelState.AddModelError("error", "验证码错误！");
                    return View(loginModel);
                }
            }
            else
            {
                //验证IP
                if (!string.IsNullOrWhiteSpace(demo.UserDemoAllowIP))
                {
                    var ips = demo.UserDemoAllowIP.Split(',');
                    var currentIp = Utils.GetIpAddress();
                    if (!ips.Contains(currentIp))
                    {
                        ModelState.AddModelError("error", "抱歉，测试账号只供内部测试使用！");
                        return View(loginModel);
                    }
                }
            }
            Login(user);
            return RedirectToAction("Order", "UserCenter");
        }
        public ActionResult Binding()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Binding(string phone, string code)
        {
            //校验用户
            var obj = Session["CurrentWeiXinOpenVIPUnionid"];
            if (obj == null)
            {
                return RedirectToAction("Index");
            }
            var user = _linkManService.CheackUser(phone);
            if (user == null)
            {
                ModelState.AddModelError("error", "此手机号暂未开通会员，请联系我们处理！");
                return View();
            }
            //校验验证码
            var cacheCode = _cacheService.GetObject<string>(phone.Trim());
            if (cacheCode == null)
            {
                ModelState.AddModelError("error", "验证码无效！");
                return View();
            }
            if (cacheCode.ToString() != code)
            {
                ModelState.AddModelError("error", "验证码错误！");
                return View();
            }

            if (!string.IsNullOrWhiteSpace(user.UnionId))
            {
                ModelState.AddModelError("error", "此账户已绑定微信，无需重复绑定！");
                return View();
            }
            user.UnionId = obj.ToString();
            Login(user,"绑定微信成功");
            return RedirectToAction("Order", "UserCenter");
        }
        public ActionResult SmsCode(string phone)
        {
            var demo = _settingService.GetSetting<WeiGuang>();
            if (phone == demo.UserDemo)
            {
                return Json(new { State = 1, Msg = "获取成功" },JsonRequestBehavior.AllowGet);
            }
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
            var user = _linkManService.CheackUser(phone.Trim());
            if (user == null)
            {
                return Json(new { State = 0, Msg = "此手机号暂未开通会员，请联系我们处理！" }, JsonRequestBehavior.AllowGet);
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

        private void Login(LinkMan user,string msg="成功")
        {
            LinkManView viewModel = new LinkManView();
            viewModel.Id = user.Id;
            viewModel.CommpanyName = user.Commpany.Name;
            viewModel.Name = user.Name;
            viewModel.LoginName = user.LoginName;
            viewModel.Phone = user.Phone;
            viewModel.Transactor = user.Transactor;
            viewModel.TransactorId = user.TransactorId;
            string sessionId = Guid.NewGuid().ToString("N");
            _cacheService.Put(sessionId, viewModel, new TimeSpan(1, 0, 0, 0));
            //Cookie
            Response.Cookies["UserSession"].Value = sessionId;
            //会员登陆日志
            user.FollowUps.Add(new FollowUp()
            {
                Id = IdBuilder.CreateIdNum(),
                IpAddress = Utils.GetIpAddress(),
                Content = Request.UserAgent,
                NextTime = DateTime.Now,
                FollowUpWay = msg
            });
            _linkManService.Update(user);
        }
    }
}