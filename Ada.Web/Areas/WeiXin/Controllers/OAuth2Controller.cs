using System.Linq;
using System.Web.Mvc;
using Ada.Core;
using Ada.Core.Domain.WeiXin;
using Ada.Core.Tools;
using Senparc.Weixin;
using Senparc.Weixin.MP.AdvancedAPIs;

namespace WeiXin.Controllers
{
    public class OAuth2Controller : Controller
    {
        private readonly IRepository<WeiXinAccount> _repository;

        public OAuth2Controller(IRepository<WeiXinAccount> repository)
        {
            _repository = repository;
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
        
    }
}