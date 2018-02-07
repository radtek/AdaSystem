using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.ViewModel.API.iDataAPI
{
   public class WeiXinProParams:BaseParams
    {
       
        /// <summary>
        /// 文章链接(需base64处理，最多5条链接，并且同为一个公众号)
        /// </summary>
        [Display(Name = "文章链接")]
        public string ArticleLinks { get; set; }
        /// <summary>
        /// 时间范围
        /// </summary>
        [Display(Name = "时间范围")]
        public string Range { get; set; }
        
        /// <summary>
        /// 开始日期
        /// </summary>
        [Display(Name = "开始日期")]
        public DateTime? StartDate { get; set; }
        /// <summary>
        /// 结束日期
        /// </summary>
        [Display(Name = "结束日期")]
        public DateTime? EndDate { get; set; }
        
    }
}
