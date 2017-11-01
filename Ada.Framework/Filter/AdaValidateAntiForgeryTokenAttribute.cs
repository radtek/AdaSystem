using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace Ada.Framework.Filter
{
    /// <summary>
    /// 用于AJAX的CSRF防伪验证方法
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public  class AdaValidateAntiForgeryTokenAttribute: FilterAttribute, IAuthorizationFilter
    {
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
            //从cookies 和 Headers 中 验证防伪标记
            //这里可以加try-catch
            AntiForgery.Validate(cookieValue, request.Headers["__RequestVerificationToken"]);
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
}
