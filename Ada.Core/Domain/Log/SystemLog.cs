using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.Domain.Log
{
    public class SystemLog
    {
        /// <summary>
        /// 日志主键
        /// </summary>
        [Display(Name = "主键")]
        public long Id { get; set; }

        /// <summary>
        /// 日志时间
        /// </summary>
        [Display(Name = "日志时间")]
        public System.DateTime? Date { get; set; }

        /// <summary>
        /// 线程编号
        /// </summary>
        [Display(Name = "线程编号")]
        public string Thread { get; set; }

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
    }
}
