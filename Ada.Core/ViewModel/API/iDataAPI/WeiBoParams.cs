using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.ViewModel.API.iDataAPI
{
   public class WeiBoParams:BaseParams
    {
        /// <summary>
        /// 查询日期 与uid参数一起使用，用户微博查询日期，支持到某一个月，为空则为查询该用户最新微博。如 201801 表示2018年1月
        /// </summary>
        [Display(Name = "查询日期")]
        public string Date { get; set; }
        /// <summary>
        /// 类型 关键词搜索请求类型，默认为空，type=hot返回热门微博，type=original返回原创微博。
        /// </summary>
        [Display(Name = "类型")]
        public string Type { get; set; }
    }
}
