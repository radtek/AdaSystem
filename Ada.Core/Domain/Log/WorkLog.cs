using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.Domain.Log
{
    /// <summary>
    /// 工作日志
    /// </summary>
    public class WorkLog : BaseEntity
    {
        /// <summary>
        /// 日志时间
        /// </summary>
        [Display(Name = "日志时间")]
        public System.DateTime? Date { get; set; }

        /// <summary>
        /// 工作主题
        /// </summary>
        [Display(Name = "工作主题")]
        public string Title { get; set; }

        /// <summary>
        /// 工作内容
        /// </summary>
        [Display(Name = "工作内容")]
        public string Content { get; set; }

        /// <summary>
        /// 主管寄语
        /// </summary>
        [Display(Name = "主管寄语")]
        public string Manager { get; set; }

        /// <summary>
        /// 总监寄语
        /// </summary>
        [Display(Name = "总监寄语")]
        public string Director { get; set; }

        /// <summary>
        /// BOSS寄语
        /// </summary>
        [Display(Name = "BOSS寄语")]
        public string Boss { get; set; }
        /// <summary>
        /// 日志人员
        /// </summary>
        [Display(Name = "日志人员")]
        public string Transactor { get; set; }
        /// <summary>
        /// 日志人员
        /// </summary>
        [Display(Name = "日志人员")]
        public string TransactorId { get; set; }
    }
}
