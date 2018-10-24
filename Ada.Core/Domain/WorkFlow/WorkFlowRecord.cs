using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core.Domain.Common;

namespace Ada.Core.Domain.WorkFlow
{
   public class WorkFlowRecord : BaseEntity
    {
        public WorkFlowRecord()
        {
            WorkFlowRecordDetails=new HashSet<WorkFlowRecordDetail>();
            Attachments = new HashSet<Attachment>();
        }
        /// <summary>
        /// 申请主题
        /// </summary>
        [Display(Name = "申请主题")]
        public string Title { get; set; }
        /// <summary>
        /// 申请内容
        /// </summary>
        [Display(Name = "申请内容")]
        public string Content { get; set; }
        /// <summary>
        /// 紧急程度
        /// </summary>
        [Display(Name = "紧急程度")]
        public short? Level { get; set; }
        /// <summary>
        /// 流程状态
        /// </summary>
        [Display(Name = "流程状态")]
        public short? Status { get; set; }
        /// <summary>
        /// 流程实例ID
        /// </summary>
        [Display(Name = "流程实例ID")]
        public string WfInstanceId { get; set; }
        /// <summary>
        /// 工作流程
        /// </summary>
        [Display(Name = "工作流程")]
        public string WorkFlowDefinitionId { get; set; }
        public virtual WorkFlowDefinition WorkFlowDefinition { get; set; }
        public virtual ICollection<WorkFlowRecordDetail> WorkFlowRecordDetails { get; set; }
        public virtual ICollection<Attachment> Attachments { get; set; }
    }
}
