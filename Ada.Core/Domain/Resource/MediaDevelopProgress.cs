using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.Domain.Resource
{
   public class MediaDevelopProgress:BaseEntity
    {
        /// <summary>
        /// 进度内容
        /// </summary>
        [Display(Name = "进度内容")]
        public string ProgressContent { get; set; }
        /// <summary>
        /// 进度日期
        /// </summary>
        [Display(Name = "进度日期")]
        public DateTime? ProgressDate { get; set; }
        /// <summary>
        /// 媒体开发
        /// </summary>
        [Display(Name = "媒体开发")]
        public string MediaDevelopId { get; set; }
        public virtual MediaDevelop MediaDevelop { get; set; }
        
    }
}
