using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Crawler.Models;
using Microsoft.Ajax.Utilities;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.IE;

namespace Crawler.Services
{
    public class WebCrawler : IWebCrawler
    {
        public event EventHandler<OnStartEventArgs> OnStart;
        public event EventHandler<OnCompletedEventArgs> OnCompleted;
        public event EventHandler<OnErrorEventArgs> OnError;
        public string Proxy { get; set; }
        public async Task Start(Uri uri, Script script=null, Operation operation=null)
        {
            await Task.Run(() =>
            {
                OnStart?.Invoke(this, new OnStartEventArgs(uri));
                try
                {
                    var service = ChromeDriverService.CreateDefaultService();
                    service.HideCommandPromptWindow = true;

                    var option = new ChromeOptions();

                    //option.AddArgument("disable-infobars"); //隐藏 自动化标题
                    option.AddArgument("headless"); //隐藏 chorme浏览器
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
                        option.Proxy = proxy;
                    }
                    using (var driver = new ChromeDriver(service, option))
                    {
                        Stopwatch watcher = new Stopwatch();
                        watcher.Start();
                        driver.Navigate().GoToUrl(uri.ToString());
                        if (script != null) driver.ExecuteScript(script.Code, script.Args);//执行Javascript代码
                        operation?.WebAction?.Invoke(driver);
                        var threadId = System.Threading.Thread.CurrentThread.ManagedThreadId;//获取当前任务线程ID
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
