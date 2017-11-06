using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.ViewModel.Admin
{
  public  class SystemLogView: BaseView
    {
        /// <summary>
        /// 日志级别
        /// </summary>
        [Display(Name = "日志级别")]
        public string Level { get; set; }

        /// <summary>
        /// 日志模块
        /// </summary>
        [Display(Name = "日志模块")]
        public string Logger { get; set; }

        /// <summary>
        /// 日志描述
        /// </summary>
        [Display(Name = "日志描述")]
        public string Message { get; set; }
        /// <summary>
        /// 异常信息
        /// </summary>
        [Display(Name = "异常信息")]
        public string Exception { get; set; }
        /// <summary>
        /// 日志时间
        /// </summary>
        [Display(Name = "日志时间")]
        public string Date { get; set; }
    }
}
