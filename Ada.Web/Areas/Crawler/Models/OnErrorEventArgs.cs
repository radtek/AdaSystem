using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crawler.Models
{
   public class OnErrorEventArgs
    {
        /// <summary>
        /// 爬虫地址
        /// </summary>
        public Uri Uri { get; set; }
        /// <summary>
        /// 异常
        /// </summary>
        public Exception Exception { get; set; }

        public OnErrorEventArgs(Uri uri, Exception exception)
        {
            Uri = uri;
            Exception = exception;
        }
    }
}
