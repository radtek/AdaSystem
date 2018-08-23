using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.Domain.Common
{
    public class Feedback : BaseEntity
    {
        /// <summary>
        /// 主题
        /// </summary>
        [Display(Name = "主题")]
        public string Title { get; set; }

        /// <summary>
        /// 提交内容
        /// </summary>
        [Display(Name = "提交内容")]
        public string Content { get; set; }
        /// <summary>
        /// 联系方式
        /// </summary>
        [Display(Name = "联系方式")]
        public string Contact { get; set; }
        /// <summary>
        /// 提交人
        /// </summary>
        [Display(Name = "提交人")]
        public string Name { get; set; }
        /// <summary>
        /// 提交时间
        /// </summary>
        [Display(Name = "提交时间")]
        public DateTime? SubTime { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        [Display(Name = "类型")]
        public string Type { get; set; }
    }
}
