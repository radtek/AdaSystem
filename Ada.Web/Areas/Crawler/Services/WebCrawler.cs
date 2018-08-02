using System;
using System.Threading.Tasks;
using Crawler.Models;
using OpenQA.Selenium.PhantomJS;
using OpenQA.Selenium.Support.UI;

namespace Crawler.Services
{
    public class WebCrawler : IWebCrawler
    {
        public event EventHandler<OnStartEventArgs> OnStart;
        public event EventHandler<OnCompletedEventArgs> OnCompleted;
        public event EventHandler<OnErrorEventArgs> OnError;
        private const string UserAgent =
            "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/50.0.2661.102 Safari/537.36";
        /// <summary>
        /// 定义PhantomJS内核参数
        /// </summary>
        private readonly PhantomJSOptions _options;
        /// <summary>
        /// 定义Selenium驱动配置
        /// </summary>
        private readonly PhantomJSDriverService _service;
        public WebCrawler(string customUserAgent=null,string proxy=null)
        {
            _options = new PhantomJSOptions();
            _options.AddAdditionalCapability(@"phantomjs.page.settings.userAgent", UserAgent);
            if (string.IsNullOrWhiteSpace(customUserAgent))
            {
                customUserAgent = UserAgent;
            }
            _options.AddAdditionalCapability(@"phantomjs.page.customHeaders.User-Agent", customUserAgent);//spider
            _service = PhantomJSDriverService.CreateDefaultService();
            _service.IgnoreSslErrors = true;//忽略证书错误
            _service.WebSecurity = false;//禁用网页安全
            _service.HideCommandPromptWindow = true;//隐藏弹出窗口
            _service.LoadImages = false;//禁止加载图片
            _service.LocalToRemoteUrlAccess = true;//允许使用本地资源响应远程 URL
            if (string.IsNullOrWhiteSpace(proxy))
            {
                _service.Proxy = proxy;//代理IP及端口
            }
            else
            {
                _service.ProxyType = "none";//不使用代理
            }
        }
        public async Task Start(Uri uri, Script script, Operation operation)
        {
            await Task.Run(() =>
            {
                OnStart?.Invoke(this, new OnStartEventArgs(uri));
                try
                {
                    using (var driver = new PhantomJSDriver(_service, _options))
                    {
                        var watch = DateTime.Now;
                        driver.Navigate().GoToUrl(uri.ToString());//请求URL地址
                        if (script != null) driver.ExecuteScript(script.Code, script.Args);//执行Javascript代码
                        operation.WebAction?.Invoke(driver);
                        var driverWait = new WebDriverWait(driver, TimeSpan.FromMilliseconds(operation.Timeout));//设置超时时间为x毫秒
                        if (operation.Condition != null) driverWait.Until(operation.Condition);
                        var threadId = System.Threading.Thread.CurrentThread.ManagedThreadId;//获取当前任务线程ID
                        var milliseconds = DateTime.Now.Subtract(watch).Milliseconds;//获取请求执行时间;
                        var pageSource = driver.PageSource;//获取网页Dom结构
                        OnCompleted?.Invoke(this, new OnCompletedEventArgs(uri, threadId, milliseconds, pageSource, driver));
                    }
                }
                catch (Exception ex)
                {
                    OnError?.Invoke(this, new OnErrorEventArgs(uri, ex));
                }
            });
        }
    }
}
