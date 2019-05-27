using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core.Domain.Common;

namespace Ada.Core.Domain.Demand
{
   public class SubjectDetailProgress : BaseEntity
    {
        public SubjectDetailProgress()
        {
            Attachments = new HashSet<Attachment>();
        }
        /// <summary>
        /// 上传日期
        /// </summary>
        [Display(Name = "上传日期")]
        public DateTime? UploadDate { get; set; }
        /// <summary>
        /// 需求明细
        /// </summary>
        [Display(Name = "需求明细")]
        public string SubjectDetailId { get; set; }
        public virtual SubjectDetail SubjectDetail { get; set; }
        public virtual ICollection<Attachment> Attachments { get; set; }
    }
}
