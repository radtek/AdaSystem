using System;
using System.Web.Mvc;
using Ada.Core.Tools;
using Ada.Core.ViewModel;
using Ada.Core.ViewModel.Admin;
using Ada.Framework.Caching;
using Ada.Services.Admin;
using Senparc.Weixin;
using Senparc.Weixin.MP.AdvancedAPIs.User;
using Senparc.Weixin.Open;
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

        public ActionResult OpenVIP()
        {
            var guid = Guid.NewGuid().ToString("N");
            Session["LoginOpenVIPState"] = guid;
            var retureUrl= Request.Url.Scheme + "://" + Request.Url.Authority + Url.Action("OpenVIP", "OAuth2");
            var returnOpenUrl =
                QRConnectAPI.GetQRConnectUrl("wx4cc90f050274ab2e", retureUrl, guid, new[] {OAuthScope.snsapi_login});
            return Redirect(returnOpenUrl);
        }
       
        
    }
}