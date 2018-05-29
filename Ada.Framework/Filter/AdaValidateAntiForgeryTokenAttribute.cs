using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using log4net;

namespace Ada.Framework.Filter
{
    /// <summary>
    /// 用于AJAX的CSRF防伪验证方法
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public  class AdaValidateAntiForgeryTokenAttribute: FilterAttribute, IAuthorizationFilter
    {
        //private readonly ILog _logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private void ValidateRequestHeader(HttpRequestBase request)
        {
            //string cookieToken = string.Empty;
            //string formToken = string.Empty;
            //string tokenValue = request.Headers["RequestVerificationToken"];
            //if (!string.IsNullOrEmpty(tokenValue))
            //{
            //    string[] tokens = tokenValue.Split(':');
            //    if (tokens.Length == 2)
            //    {
            //        cookieToken = tokens[0].Trim();
            //        formToken = tokens[1].Trim();
            //    }
            //}
            //AntiForgery.Validate(cookieToken, formToken);
            var antiForgeryCookie = request.Cookies[AntiForgeryConfig.CookieName];
            var cookieValue = antiForgeryCookie?.Value;
            var token = request.Headers["__RequestVerificationToken"];
            //从cookies 和 Headers 中 验证防伪标记
            //这里可以加try-catch
            if (!string.IsNullOrWhiteSpace(cookieValue)&&!string.IsNullOrWhiteSpace(token))
            {
                AntiForgery.Validate(cookieValue,token);
            }
            else
            {
                AntiForgery.Validate();
            }
            
        }

        public void OnAuthorization(AuthorizationContext filterContext)
        {

            try
            {
                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    ValidateRequestHeader(filterContext.HttpContext.Request);
                }
                else
                {
                    AntiForgery.Validate();
                }
            }
            catch (HttpAntiForgeryException)
            {
                throw new HttpAntiForgeryException("防伪验证失败");
            }
        }
    }

    /// <inheritdoc />
    /// <summary>
    /// 页面禁止缓存过滤器
    /// </summary>
    public class NoCacheAttribute : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            filterContext.HttpContext.Response.Cache.SetCacheability(System.Web.HttpCacheability.NoCache);
            filterContext.HttpContext.Response.Cache.SetNoStore();
        }
    }
}
