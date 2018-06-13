using System;
using System.Linq;
using System.Web.Mvc;
using Ada.Core;
using Ada.Core.Domain.Customer;
using Ada.Core.Domain.WeiXin;
using Ada.Core.Tools;
using Ada.Core.ViewModel;
using Ada.Core.ViewModel.Customer;
using Ada.Framework.Caching;
using Ada.Services.Admin;
using Ada.Services.Cache;
using Ada.Services.Customer;
using Senparc.Weixin;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.Open.QRConnect;

namespace WeiXin.Controllers
{
    public class OAuth2Controller : Controller
    {
        private readonly IRepository<WeiXinAccount> _repository;
        private readonly ILinkManService _linkManService;
        private readonly IManagerService _service;
        private readonly ICacheService _cacheService;
        private readonly ISignals _signals;

        public OAuth2Controller(IRepository<WeiXinAccount> repository,
            IManagerService service, 
            ISignals signals,
            ILinkManService linkManService,
            ICacheService cacheService)
        {
            _repository = repository;
            _service = service;
            _signals = signals;
            _linkManService = linkManService;
            _cacheService = cacheService;
        }
        /// <summary>
        /// OAuthScope.snsapi_userinfo方式回调
        /// </summary>
        /// <param name="code"></param>
        /// <param name="state"></param>
        /// <param name="returnUrl">用户最初尝试进入的页面</param>
        /// <returns></returns>
        public ActionResult Index(string code, string state, string returnUrl)
        {
            if (string.IsNullOrEmpty(code))
            {
                return Content("授权拒绝");
            }
            //通过，用code换取access_token
            var account = _repository.LoadEntities(d => d.AppId == state).FirstOrDefault();
            if (account == null)
            {
                return Content("缺少微信参数");
            }
            var result = OAuthApi.GetAccessToken(account.AppId, account.AppSecret, code);
            if (result.errcode != ReturnCode.请求成功)
            {
                return Content("错误：" + result.errmsg);
            }
            Session["CurrentWeiXin"] = SerializeHelper.SerializeToString(UserApi.Info(account.AppId, result.openid)); ;//进行登录
            return Redirect(returnUrl);
        }
        public ActionResult Open(string code, string state)
        {
            if (string.IsNullOrEmpty(code))
            {
                return Content("您拒绝了授权登陆");
            }
            //验证State
            var sessionState = Session["LoginOpenState"] as string;
            Session.Remove("LoginOpenState");
            if (state != sessionState)
            {
                return Content("登陆参数异常");
            }
            var result = QRConnectAPI.GetAccessToken("wx4cc90f050274ab2e", "92a6a89964a6dafec1b9eb5fac4f49a5", code);
            if (result.errcode != ReturnCode.请求成功)
            {
                return Content("错误：" + result.errmsg);
            }

            var logModel = new LoginModel
            {
                OpenId = result.unionid,
                LoginLog = new LoginLog() { UserAgent = Request.UserAgent }
            };
            var manager = _service.Login(logModel);
            if (manager == null)
            {
                var weixin = QRConnectAPI.GetUserInfo(result.access_token, result.openid);
                Session["CurrentWeiXinOpenUnionid"] = weixin.unionid;
                //绑定页面
                return RedirectToAction("Binding", "Login", new { area = "Admin" });
            }
            Session["LoginManager"] = SerializeHelper.SerializeToString(manager);
            //清空登陆日志缓存
            _signals.Trigger("LoginLog" + manager.Id + ".Changed");
            return RedirectToAction("Index", "Login", new { area = "Admin" });
        }

        public ActionResult OpenVIP(string code, string state)
        {
            if (string.IsNullOrEmpty(code))
            {
                return Content("您拒绝了授权登陆");
            }
            //验证State
            var sessionState = Session["LoginOpenVIPState"] as string;
            Session.Remove("LoginOpenVIPState");
            if (state != sessionState)
            {
                return Content("登陆参数异常");
            }
            var result = QRConnectAPI.GetAccessToken("wx4cc90f050274ab2e", "92a6a89964a6dafec1b9eb5fac4f49a5", code);
            if (result.errcode != ReturnCode.请求成功)
            {
                return Content("错误：" + result.errmsg);
            }

            var user = _linkManService.GetUserByOpenId(result.unionid);
            if (user == null)
            {
                Session["CurrentWeiXinOpenVIPUnionid"] = result.unionid;
                //绑定页面
                return RedirectToAction("Binding", "Login", new { area = "" });
            }
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
                FollowUpWay = "微信登陆成功"
            });
            _linkManService.Update(user);
            return RedirectToAction("Order", "UserCenter", new { area = "" });
        }
    }
}