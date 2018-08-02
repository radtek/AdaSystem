using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;

namespace Crawler.Models
{
   public class OnCompletedEventArgs
    {
        /// <summary>
        /// 爬虫URL地址
        /// </summary>
        public Uri Uri { get; }
        /// <summary>
        /// 任务线程ID
        /// </summary>
        public int ThreadId { get; }
        /// <summary>
        /// 页面源代码
        /// </summary>
        public string PageSource { get; }
        public IWebDriver WebDriver { get; }
        /// <summary>
        /// 爬虫请求执行事件
        /// </summary>
        public long Milliseconds { get; }
        public OnCompletedEventArgs(Uri uri, int threadId, long milliseconds, string pageSource, IWebDriver driver)
        {
            Uri = uri;
            ThreadId = threadId;
            Milliseconds = milliseconds;
            PageSource = pageSource;
            WebDriver = driver;
        }
    }
}
