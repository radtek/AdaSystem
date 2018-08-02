using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crawler.Models
{
    public class OnStartEventArgs
    {
        /// <summary>
        /// 爬虫URL地址
        /// </summary>
        public Uri Uri { get; set; }

        public OnStartEventArgs(Uri uri)
        {
            this.Uri = uri;
        }
    }
}
