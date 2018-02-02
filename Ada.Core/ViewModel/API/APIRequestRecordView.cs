using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.ViewModel.API
{
   public class APIRequestRecordView:BaseView
    {
        /// <summary>
        /// 请求参数
        /// </summary>
        [Display(Name = "请求参数")]
        public string RequestParameters { get; set; }
        /// <summary>
        /// 请求时间
        /// </summary>
        [Display(Name = "请求时间")]
        public DateTime? RequestDate { get; set; }
        /// <summary>
        /// 响应时间
        /// </summary>
        [Display(Name = "响应时间")]
        public DateTime? ReponseDate { get; set; }
        /// <summary>
        /// 响应内容
        /// </summary>
        [Display(Name = "响应内容")]
        public string ReponseContent { get; set; }
        /// <summary>
        /// 返回状态码
        /// </summary>
        [Display(Name = "返回状态码")]
        public string Retcode { get; set; }
        /// <summary>
        /// 错误信息
        /// </summary>
        [Display(Name = "错误信息")]
        public string Retmsg { get; set; }
        /// <summary>
        /// 是否成功
        /// </summary>
        [Display(Name = "是否成功")]
        public bool? IsSuccess { get; set; }
        /// <summary>
        /// API名称
        /// </summary>
        [Display(Name = "API名称")]
        public string APIName { get; set; }
    }
}
