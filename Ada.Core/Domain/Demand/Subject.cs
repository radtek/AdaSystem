using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core.Domain.Common;

namespace Ada.Core.Domain.Demand
{
    public class Subject : BaseEntity
    {
        public Subject()
        {
            Attachments = new HashSet<Attachment>();
            SubjectDetails=new HashSet<SubjectDetail>();
            Offer = 10;
        }
        /// <summary>
        /// 需求名称
        /// </summary>
        [Display(Name = "需求名称")]
        public string Title { get; set; }
        /// <summary>
        /// 需求内容
        /// </summary>
        [Display(Name = "需求内容")]
        public string Content { get; set; }
        /// <summary>
        /// 需求类型
        /// </summary>
        [Display(Name = "需求类型")]
        public string Type { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        [Display(Name = "状态")]
        public short? Status { get; set; }
        /// <summary>
        /// 需求报价
        /// </summary>
        [Display(Name = "需求报价")]
        public decimal Offer { get; set; }
        public virtual ICollection<Attachment> Attachments { get; set; }
        public virtual ICollection<SubjectDetail> SubjectDetails { get; set; }
    }
}
