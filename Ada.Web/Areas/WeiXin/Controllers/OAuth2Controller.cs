using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Core;
using Ada.Core.Domain.WeiXin;
using Senparc.Weixin;
using Senparc.Weixin.Exceptions;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.AdvancedAPIs.OAuth;

namespace WeiXin.Controllers
{
    public class OAuth2Controller : Controller
    {
        private readonly IRepository<WeiXinAccount> _repository;

        public OAuth2Controller(IRepository<WeiXinAccount> repository)
        {
            _repository = repository;
        }
        public ActionResult Index()
        {
            var account = _repository.LoadEntities(d => d.Status == true).FirstOrDefault();
            if (account == null)
            {
                return Content("暂无公众号信息");
            }

            var currentUrl = Request.Url.Host;
            ViewBag.AdminBinding = OAuthApi.GetAuthorizeUrl(account.AppId,
                currentUrl+Url.Action("UserInfoCallback", new { type = 1 }),
                account.Id, OAuthScope.snsapi_userinfo);
            ViewBag.AdminBinding = OAuthApi.GetAuthorizeUrl(account.AppId,
                currentUrl + Url.Action("UserInfoCallback", new { type = 0 }),
                account.Id, OAuthScope.snsapi_userinfo);
            return View();
        }

        /// <summary>
        /// OAuthScope.snsapi_userinfo方式回调
        /// </summary>
        /// <param name="code"></param>
        /// <param name="state"></param>
        /// <param name="type">用户最初尝试进入的页面</param>
        /// <returns></returns>
        public ActionResult UserInfoCallback(string code, string state, int type)
        {
            if (string.IsNullOrEmpty(code))
            {
                return RedirectToAction("Index");
            }
            //通过，用code换取access_token
            var account = _repository.LoadEntities(d => d.Id == state).FirstOrDefault();
            if (account == null)
            {
                return Content("非法请求");
            }
            var result = OAuthApi.GetAccessToken(account.AppId, account.AppSecret, code);
            if (result.errcode != ReturnCode.请求成功)
            {
                return Content("错误：" + result.errmsg);
            }

            if (type==1)
            {
                return RedirectToAction("Binding", "Login", new {area = "Admin", openid = result.openid});
            }
            return RedirectToAction("Index");
        }
    }
}