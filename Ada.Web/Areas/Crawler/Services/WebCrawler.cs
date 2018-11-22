using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Crawler.Models;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace Crawler.Services
{
    public class WebCrawler : IWebCrawler
    {
        public event EventHandler<OnStartEventArgs> OnStart;
        public event EventHandler<OnCompletedEventArgs> OnCompleted;
        public event EventHandler<OnErrorEventArgs> OnError;
        public string Proxy { get; set; }
        /// <summary>
        /// 定义PhantomJS内核参数
        /// </summary>
        private readonly ChromeOptions _options;
        /// <summary>
        /// 定义Selenium驱动配置
        /// </summary>
        private readonly ChromeDriverService _service;
        public WebCrawler()
        {
            _service = ChromeDriverService.CreateDefaultService();
            _service.HideCommandPromptWindow = true;

            _options = new ChromeOptions();

            //option.AddArgument("disable-infobars"); //隐藏 自动化标题
            _options.AddArgument("headless"); //隐藏 chorme浏览器
            //option.AddArgument("--incognito");//隐身模式

            if (!string.IsNullOrWhiteSpace(Proxy))
            {
                string proxyIpAndPort = Proxy;
                Proxy proxy = new Proxy
                {
                    HttpProxy = proxyIpAndPort,
                    SslProxy = proxyIpAndPort,
                    FtpProxy = proxyIpAndPort,
                    IsAutoDetect = false
                };
                _options.Proxy = proxy;
            }
        }
        public async Task Start(Uri uri, Operation operation=null, Script script = null)
        {
            await Task.Run(() =>
            {
                OnStart?.Invoke(this, new OnStartEventArgs(uri));
                try
                {
                    using (var driver = new ChromeDriver(_service, _options))
                    {
                        Stopwatch watcher = new Stopwatch();
                        watcher.Start();
                        driver.Url = uri.ToString();
                        if (script != null) driver.ExecuteScript(script.Code, script.Args);//执行Javascript代码
                        if (operation != null)
                        {
                            operation.WebAction?.Invoke(driver);
                            
                            if (operation.Condition != null)
                            {
                                var wait = new WebDriverWait(driver, TimeSpan.FromMilliseconds(operation.Timeout));
                                wait.Until(operation.Condition);
                            }
                        }
                        var threadId = Thread.CurrentThread.ManagedThreadId;//获取当前任务线程ID
                        var pageSource = driver.PageSource;//获取网页Dom结构
                        watcher.Stop();
                        OnCompleted?.Invoke(this, new OnCompletedEventArgs(uri, threadId, watcher.ElapsedMilliseconds, pageSource, driver));
                    }
                }
                catch (Exception ex)
                {
                    OnError?.Invoke(this, new OnErrorEventArgs(uri, ex));
                }
            });
        }

        public string FindElementByXpath(OnCompletedEventArgs e, string xpath)
        {
            try
            {
                return e.WebDriver.FindElement(By.XPath(xpath)).Text;
            }
            catch 
            {
                return null;
            }
            
        }
        public string FindElementByClassName(OnCompletedEventArgs e, string className)
        {
            
            try
            {
                return e.WebDriver.FindElement(By.ClassName(className)).Text;
            }
            catch
            {
                return null;
            }
        }
        public string FindElementByClassName(OnCompletedEventArgs e, string className,string attribute)
        {
            
            try
            {
                return e.WebDriver.FindElement(By.ClassName(className)).GetAttribute(attribute);
            }
            catch
            {
                return null;
            }
        }
    }
}
