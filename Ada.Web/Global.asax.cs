using System;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Http;
using System.Web.Optimization;
using Ada.Core.Infrastructure;
using Ada.Core.Tools;
using log4net;

namespace Ada.Web
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            MvcHandler.DisableMvcResponseHeader = true;
            //全局过滤器
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            //初始化引擎启动
            EngineContext.Initialize(false);
            // 在应用程序启动时运行的代码
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AppStart.Register();
        }
        //protected void Application_End()
        //{
        //    ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        //    logger.Error("Application_End被触发");


        //}
        protected void Application_Error()
        {
            var ex = Server.GetLastError();
            ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            logger.Error("Application_Error触发，请求IP：" + Utils.GetIpAddress(), ex);
            Server.ClearError();
            Response.Redirect("~/404.html", true);
        }
       
    }
}