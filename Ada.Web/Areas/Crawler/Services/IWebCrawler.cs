using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core;
using Crawler.Models;

namespace Crawler.Services
{
   public interface IWebCrawler
    {
        /// <summary>
        /// 爬虫启动事件
        /// </summary>
        event EventHandler<OnStartEventArgs> OnStart;
        /// <summary>
        /// 爬虫完成事件
        /// </summary>
        event EventHandler<OnCompletedEventArgs> OnCompleted;
        /// <summary>
        /// 爬虫异常事件
        /// </summary>
        event EventHandler<OnErrorEventArgs> OnError;
        /// <summary>
        /// 启动爬虫进程
        /// </summary>
        /// <param name="uri">爬虫地址</param>
        /// <param name="script">执行脚本</param>
        /// <param name="operation">执行操作</param>
        /// <returns></returns>
        Task Start(Uri uri, Script script, Operation operation); 
    }
}
