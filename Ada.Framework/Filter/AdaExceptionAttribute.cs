using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using Ada.Core.Tools;
using Ada.Core.ViewModel;
using Ada.Core.ViewModel.Admin;
using log4net;
using Newtonsoft.Json;

namespace Ada.Framework.Filter
{
    public class AdaExceptionAttribute : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            //base.OnException(filterContext);
            if (filterContext.ExceptionHandled)
            {
                return;
            }
            Exception ex = filterContext.Exception;
            //写日志
            //写到队列(如果是写入文件，就要考虑并发写入队列)
            //ExecptionQueue.Enqueue(ex);
            var controller = (string)filterContext.RouteData.Values["controller"];//控制器名称
            var action = (string)filterContext.RouteData.Values["action"];
            var area = (string)filterContext.RouteData.Values["area"];
            var httpmethod = filterContext.HttpContext.Request.HttpMethod;
            ILog logger = LogManager.GetLogger($"{httpmethod}  {area}/{controller}/{action}");//MethodBase.GetCurrentMethod().DeclaringType
            var managerSession = filterContext.HttpContext.Session["LoginManager"];
            string errMsg = JsonConvert.SerializeObject(new { message = "系统异常" });
            try
            {
                var userInfo = string.Empty;
                if (managerSession != null)
                {
                    var manager = SerializeHelper.DeserializeToObject<ManagerView>(managerSession.ToString());
                    userInfo = manager.UserName + " [" + manager.RoleName + "];";
                }
                IDictionary<string, string> dc = new Dictionary<string, string>();
                foreach (var formAllKey in filterContext.HttpContext.Request.Form.AllKeys)
                {
                    var temp = filterContext.HttpContext.Request.Form[formAllKey];
                    dc.Add(formAllKey, temp);
                }

                var quary = new
                {
                    message = "系统异常，详见异常信息",
                    method = httpmethod,
                    urlParams = filterContext.HttpContext.Request.Url?.Query,
                    formParams = dc,
                    userInfo = userInfo,
                    ip = Utils.GetIpAddress()
                };
                errMsg = JsonConvert.SerializeObject(quary);
            }
            catch
            {
                //
            }
            logger.Error(errMsg, ex);
            //跳转到错误页面.
            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                filterContext.Result = new JsonResult()
                {
                    Data = new { State = 0, Msg = ex.Message },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
            else
            {
                //根据http状态码 跳转到指定的异常页面
                var httpException = new HttpException(null, ex);
                ViewResult result = new ViewResult
                {
                    ViewName = "Error", //错误页
                    ViewData = new ViewDataDictionary(new HttpResultView() { HttpCode = httpException.GetHttpCode(), Msg = ex.Message }),       //指定模型
                    TempData = filterContext.Controller.TempData
                };
                filterContext.Result = result;
            }

            //设置异常已经处理,否则会被其他异常过滤器覆盖
            filterContext.ExceptionHandled = true;
            //在派生类中重写时，获取或设置一个值，该值指定是否禁用IIS自定义错误。
            filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
        }
    }
}
