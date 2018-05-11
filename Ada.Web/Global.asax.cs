using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.Http;
using System.Web.Optimization;
using Ada.Core.Infrastructure;
using Ada.Framework.Filter;
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
            //初始化引擎启动
            EngineContext.Initialize(false);
            // 在应用程序启动时运行的代码
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            GlobalFilters.Filters.Add(new AdaExceptionAttribute());
            AppStart.Register();
        }
        protected void Application_End()
        {
            //IIS闲置会将定时任务 停止
            ////模拟执行一下某个API
            ////Log.Info("Application_End触发", "触发时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            //System.Threading.Thread.Sleep(5000);
            //using (var httpClient = new HttpClient())
            //{
            //    var task = httpClient.GetAsync("http://manage.jxweiguang.com/api/Wglh/GetTime");
            //    var result = task.Result.Content.ReadAsStringAsync().Result;
            //    LogClassModels.WriteServiceLog("触发时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "API结果为：" + result, "Application_End触发");
            //}
            ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            logger.Error("Application_End被触发");

        }
    }
}