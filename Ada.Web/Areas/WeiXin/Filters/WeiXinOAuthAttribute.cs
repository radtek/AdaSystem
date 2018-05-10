using System;
using System.Web;
using System.Web.Mvc;
using Ada.Core.Infrastructure;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.AdvancedAPIs;
using WeiXin.Services;

namespace WeiXin.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class WeiXinOAuthAttribute : FilterAttribute, IAuthorizationFilter
    {

        protected string AppId { get; set; }
        protected string OauthCallbackUrl { get; set; }
        protected OAuthScope OauthScope { get; set; }

        public WeiXinOAuthAttribute(string oauthCallbackUrl, string appIdOrWeiXinAccountId = null, OAuthScope oauthScope = OAuthScope.snsapi_userinfo)
        {
            var service = EngineContext.Current.Resolve<IWeiXinService>();
            AppId = service.GetWeiXinAccount(appIdOrWeiXinAccountId).AppId;
            OauthCallbackUrl = oauthCallbackUrl;
            OauthScope = oauthScope;
        }
        public virtual bool IsLogined(HttpContextBase httpContext)
        {
            return httpContext?.Session["CurrentWeiXin"] != null;
        }
        protected virtual bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }
            return IsLogined(httpContext);
        }
        private void CacheValidateHandler(HttpContext context, object data, ref HttpValidationStatus validationStatus)
        {
            validationStatus = OnCacheAuthorization(new HttpContextWrapper(context));
        }
        public virtual void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException(nameof(filterContext));
            }
            if (AuthorizeCore(filterContext.HttpContext))
            {
                HttpCachePolicyBase cachePolicy = filterContext.HttpContext.Response.Cache;
                cachePolicy.SetProxyMaxAge(new TimeSpan(0));
                cachePolicy.AddValidationCallback(CacheValidateHandler, null /* data */);
            }
            else
            {
                if (IsLogined(filterContext.HttpContext))
                {

                }
                else
                {
                    var callbackUrl = Senparc.Weixin.HttpUtility.UrlUtility.GenerateOAuthCallbackUrl(filterContext.HttpContext, OauthCallbackUrl);
                    var state = AppId;
                    var url = OAuthApi.GetAuthorizeUrl(AppId, callbackUrl, state, OauthScope);
                    filterContext.Result = new RedirectResult(url);
                }
            }
        }
        protected virtual HttpValidationStatus OnCacheAuthorization(HttpContextBase httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            bool isAuthorized = AuthorizeCore(httpContext);
            return isAuthorized ? HttpValidationStatus.Valid : HttpValidationStatus.IgnoreThisRequest;
        }
    }
}