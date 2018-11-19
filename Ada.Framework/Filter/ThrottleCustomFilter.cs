using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Ada.Core.Tools;
using Ada.Core.ViewModel;
using log4net;
using MvcThrottle;
using Newtonsoft.Json;

namespace Ada.Framework.Filter
{
    public class ThrottleCustomFilter : ThrottlingFilter
    {
        protected override ActionResult QuotaExceededResult(RequestContext filterContext, string message, HttpStatusCode responseCode,
            string requestId)
        {

            //跳转到错误页面.
            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                return new JsonResult()
                {
                    Data = new { State = 0, Msg = "请求过于频繁，请稍后再试！" },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
            //根据http状态码 跳转到指定的异常页面
            return new ViewResult
            {
                ViewName = "Error", //错误页
                ViewData = new ViewDataDictionary(new HttpResultView() { HttpCode = HttpStatusCode.Forbidden.GetHashCode(), Msg = "请求过于频繁，请稍后再试！" }),       //指定模型

            };

        }
    }

    public class ThrottleCustomLogger : IThrottleLogger
    {
        public void Log(ThrottleLogEntry entry)
        {
            ILog logger = LogManager.GetLogger("Throttle限制模块");
            IDictionary<string, string> dc = new Dictionary<string, string>();
            foreach (var formAllKey in entry.Request.Form.AllKeys)
            {
                var temp = entry.Request.Form[formAllKey];
                dc.Add(formAllKey, temp);
            }
            var quary = new
            {
                date = entry.LogDate.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss"),
                requestUrl = entry.Request.Url?.AbsoluteUri,
                urlQuery = entry.Request.Url?.Query,
                postQuery = dc,
                requestId = entry.RequestId,
                limit = entry.RateLimit,
                limitPeriod = entry.RateLimitPeriod,
                total = entry.TotalRequests,
                ip = entry.ClientIp
            };
            var errMsg = JsonConvert.SerializeObject(quary);
            logger.Error(errMsg);
        }
    }

    public class ThrottleIpAddressParser : IpAddressParser
    {
        public override string GetClientIp(HttpRequestBase request)
        {
            return Utils.GetIpAddress();
        }
    }
}
