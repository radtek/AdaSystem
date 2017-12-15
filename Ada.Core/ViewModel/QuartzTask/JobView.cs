using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.ViewModel.QuartzTask
{
   public class JobView:BaseView
    {
        /// <summary>
        /// 群组
        /// </summary>
        [Display(Name = "群组")]
        public string GroupName { get; set; }
        /// <summary>
        /// 作业名称
        /// </summary>
        [Display(Name = "作业名称")]
        public string JobName { get; set; }
        /// <summary>
        /// 作业类名
        /// </summary>
        [Display(Name = "作业类名")]
        public string JobType { get; set; }
        /// <summary>
        /// 触发器名称
        /// </summary>
        [Display(Name = "触发器名称")]
        public string TriggerName { get; set; }
        /// <summary>
        /// 执行计划
        /// </summary>
        [Display(Name = "执行计划")]
        public string Cron { get; set; }
        /// <summary>
        /// 执行状态
        /// </summary>
        [Display(Name = "执行状态")]
        public string TriggerState { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        [Display(Name = "开始时间")]
        public DateTime? StartTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        [Display(Name = "结束时间")]
        public DateTime? EndTime { get; set; }
        /// <summary>
        /// 上次执行
        /// </summary>
        [Display(Name = "上次执行")]
        public DateTime? PreTime { get; set; }
        /// <summary>
        /// 下次执行
        /// </summary>
        [Display(Name = "下次执行")]
        public DateTime? NextTime { get; set; }
        /// <summary>
        /// 作业描述
        /// </summary>
        [Display(Name = "作业描述")]
        public string Remark { get; set; }
        /// <summary>
        /// AppId
        /// </summary>
        [Display(Name = "AppId")]
        public string AppId { get; set; }
        /// <summary>
        /// API地址
        /// </summary>
        [Display(Name = "API地址")]
        public string ApiUrl { get; set; }
        /// <summary>
        /// API参数
        /// </summary>
        [Display(Name = "API参数")]
        public string Params { get; set; }
        /// <summary>
        /// 令牌
        /// </summary>
        [Display(Name = "令牌")]
        public string Token { get; set; }
    }
}
