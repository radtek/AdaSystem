using System.Web.Mvc;
using Ada.Core.Tools;
using Ada.Core.ViewModel;
using Ada.Framework.Caching;
using Ada.Services.Admin;
using Senparc.Weixin;
using Senparc.Weixin.MP.AdvancedAPIs.User;
using Senparc.Weixin.Open.QRConnect;
using WeiXin.Filters;

namespace WeiXin.Controllers
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
        [WeiXinOAuth("/WeiXin/OAuth2")]
        public ActionResult Manager(string returnUrl)
        {
            if (Session["LoginManager"] == null)
            {
                var obj = Session["CurrentWeiXin"];
                var currentWxUser = SerializeHelper.DeserializeToObject<UserInfoJson>(obj.ToString());
                var logModel = new LoginModel
                {
                    OpenId = currentWxUser.openid,
                    LoginLog = new LoginLog() { UserAgent = Request.UserAgent }
                };
                var result = _service.Login(logModel);
                if (result == null)
                {
                    return RedirectToAction("Index", "Binding");
                }
                Session["LoginManager"] = SerializeHelper.SerializeToString(result);
                //清空登陆日志缓存
                _signals.Trigger("LoginLog" + result.Id + ".Changed");
            }

            if (!string.IsNullOrWhiteSpace(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home", new { area = "Dashboards" });
        }

        public ActionResult Admin()
        {
            return View();
        }
        public ActionResult Validate(string code, string state)
        {
            if (string.IsNullOrEmpty(code))
            {
                return Content("授权拒绝");
            }
            //验证State
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
                //绑定页面
                return PartialView("Binding", weixin);
            }
            Session["LoginManager"] = SerializeHelper.SerializeToString(manager);
            //清空登陆日志缓存
            _signals.Trigger("LoginLog" + manager.Id + ".Changed");
            return RedirectToAction("Index", "Home", new { area = "Dashboards" });
        }
    }
}