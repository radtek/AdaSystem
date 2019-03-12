using System;
using System.ComponentModel.DataAnnotations;

namespace Ada.Core.Domain.QuartzTask
{
   public class JobDetail: BaseEntity
    {
        public JobDetail()
        {
            Num1 = 0;
            Num2 = 0;
            Num3 = 0;
        }
        /// <summary>
        /// 结果参数1
        /// </summary>
        [Display(Name = "结果参数1")]
        public int Num1 { get; set; }
        /// <summary>
        /// 结果参数2
        /// </summary>
        [Display(Name = "结果参数2")]
        public int Num2 { get; set; }
        /// <summary>
        /// 结果参数3
        /// </summary>
        [Display(Name = "结果参数3")]
        public int Num3 { get; set; }
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
        /// 返回信息
        /// </summary>
        [Display(Name = "返回信息")]
        public string Retmsg { get; set; }
        /// <summary>
        /// 是否成功
        /// </summary>
        [Display(Name = "是否成功")]
        public bool? IsSuccess { get; set; }
        /// <summary>
        /// 工作任务
        /// </summary>
        [Display(Name = "工作任务")]
        public virtual Job Job { get; set; }
        /// <summary>
        /// 工作任务
        /// </summary>
        [Display(Name = "工作任务")]
        public string JobId { get; set; }
    }
}
