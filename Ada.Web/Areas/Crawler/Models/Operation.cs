using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;

namespace Crawler.Models
{
    public class Operation
    {
        public Operation()
        {
            Timeout = 2000;
            SleepTime = 0;
        }
        /// <summary>
        /// 超时时间
        /// </summary>
        public int Timeout { get; set; }
        /// <summary>
        /// 超时时间
        /// </summary>
        public int SleepTime { get; set; }
        /// <summary>
        /// 其他参数
        /// </summary>
        public string Args { get; set; }
        /// <summary>
        /// 执行脚本
        /// </summary>
        public Action<IWebDriver> WebAction { get; set; }
        /// <summary>
        /// 执行状态
        /// </summary>
        public Func<IWebDriver, bool> Condition { get; set; }
    }
}
