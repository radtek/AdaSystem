using System;
using System.Activities;
using System.Activities.DurableInstancing;
using System.Activities.XamlIntegration;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using System.Web;
using System.Xaml;
using log4net;

namespace WorkFlow.Template
{
    public class WorkFlowProvider : IWorkFlowProvider
    {
        private readonly string _connetStr;
        public ILog Log { get; set; }

        public WorkFlowProvider()
        {
            var connetString = ConfigurationManager.ConnectionStrings["ADADCS"].ConnectionString;
            if (string.IsNullOrWhiteSpace(connetString))
            {
                throw new ConfigurationErrorsException("未在WEB.CONFIG配置工作流连接字符串");
            }
            _connetStr = connetString;
        }
        public WorkflowApplication CreateWorkflowApp(string xamlPath, Dictionary<string, object> dicParam)
        {
            AutoResetEvent autoResetEvent = new AutoResetEvent(false);
            var settings = new XamlXmlReaderSettings() { LocalAssembly = typeof(SetStepActivity).Assembly };
            var reader = new XamlXmlReader(xamlPath, settings);
            Activity workflow = ActivityXamlServices.Load(reader);
            var wfApp = dicParam == null ? new WorkflowApplication(workflow) : new WorkflowApplication(workflow, dicParam);
            wfApp.Idle += a =>//当工作流停下来的时候，执行此事件响应方法。
            {
                //Console.WriteLine("工作流停下来了...");
                //Common.LogHelper.WriteLog("工作流停下来了");
            };

            //当咱们工作流停顿下来的时候，进行什么操作。如果返回是Unload。那么就卸载当前
            //工作流实例，持久化到数据库里面去。
            wfApp.PersistableIdle = e2 => PersistableIdleAction.Unload;

            wfApp.Unloaded += a =>
            {
                autoResetEvent.Set();
                //Console.WriteLine("工作流被卸载");
                //Common.LogHelper.WriteLog("工作流被卸载");
            };
            wfApp.OnUnhandledException += a =>
            {
                autoResetEvent.Set();
                Log.Error(wfApp.Id.ToString() + " 启动流程异常", a.UnhandledException);
                //Console.WriteLine("出现了异常..");
                //Common.LogHelper.WriteLog(a.UnhandledException.ToString());
                return UnhandledExceptionAction.Terminate;//当出现未处理的异常的时候，直接结束工作流
            };

            wfApp.Aborted += a =>
            {
                autoResetEvent.Set();
                //Console.WriteLine("Aborted");
            };

            //创建一个保存 工作流实例的sqlstore对象。
            SqlWorkflowInstanceStore store =
                new SqlWorkflowInstanceStore(_connetStr);

            //wfApp在进行持久化的时候，保存到上面对象配置的数据库里面去。
            wfApp.InstanceStore = store;
            wfApp.Run();
            return wfApp;
        }

        public WorkflowApplication ResumeBookMark(string xamlPath, Guid instanceId, string bookmarkName, object value)
        {
            AutoResetEvent autoResetEvent = new AutoResetEvent(false);
            var settings = new XamlXmlReaderSettings() { LocalAssembly = typeof(SetStepActivity).Assembly };
            var reader = new XamlXmlReader(xamlPath, settings);
            Activity workflow = ActivityXamlServices.Load(reader);
            WorkflowApplication wfApp = new WorkflowApplication(workflow);
            wfApp.Idle += a =>//当工作流停下来的时候，执行此事件响应方法。
            {
                //Console.WriteLine("工作流停下来了...");
            };

            //当咱们工作流停顿下来的时候，进行什么操作。如果返回是Unload。那么就卸载当前
            //工作流实例，持久化到数据库里面去。
            wfApp.PersistableIdle = e3 => PersistableIdleAction.Unload;

            wfApp.Unloaded += a =>
            {
                //Common.LogHelper.WriteLog("工作流被卸载");
                autoResetEvent.Set();
                //Console.WriteLine("工作流被卸载");
            };
            wfApp.OnUnhandledException += a =>
            {
                autoResetEvent.Set();
                //LogHelper.WriteLog(a.ExceptionSource.ToString());
                //Console.WriteLine("出现了异常..");
                Log.Error(wfApp.Id.ToString() + " 继续流程异常", a.UnhandledException);
                return UnhandledExceptionAction.Terminate;//当出现未处理的异常的时候，直接结束工作流
            };

            wfApp.Aborted += a =>
            {
                autoResetEvent.Set();
                //Console.WriteLine("Aborted");
            };

            //创建一个保存 工作流实例的sqlstore对象。
            SqlWorkflowInstanceStore store =
                new SqlWorkflowInstanceStore(_connetStr);

            //wfApp在进行持久化的时候，保存到上面对象配置的数据库里面去。
            wfApp.InstanceStore = store;

            //从数据库中加载当前工作流实例的状态。
            wfApp.Load(instanceId);

            //让工作流沿着 Demo书签位置继续往下执行。
            wfApp.ResumeBookmark(bookmarkName, value);

            return wfApp;
        }
    }
}