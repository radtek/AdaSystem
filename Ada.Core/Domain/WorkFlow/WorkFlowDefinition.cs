using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.Domain.WorkFlow
{
   public class WorkFlowDefinition: BaseEntity
    {
        public WorkFlowDefinition()
        {
            Enabled = false;
            WorkFlowActivitys=new HashSet<WorkFlowActivity>();
            WorkFlowsTransitions=new HashSet<WorkFlowsTransition>();
            WorkFlowRecords=new HashSet<WorkFlowRecord>();
        }
        /// <summary>
        /// 流程名称
        /// </summary>
        [Display(Name = "流程名称")]
        public string Name { get; set; }
        /// <summary>
        /// 是否开启
        /// </summary>
        [Display(Name = "是否开启")]
        public bool Enabled { get; set; }
        /// <summary>
        /// 流程描述
        /// </summary>
        [Display(Name = "流程描述")]
        public string Description { get; set; }
        /// <summary>
        /// 流程模板
        /// </summary>
        [Display(Name = "流程模板")]
        public string TempForm { get; set; }
        /// <summary>
        /// 流程类型
        /// </summary>
        [Display(Name = "流程类型")]
        public string ActityType { get; set; }
        /// <summary>
        /// 流程类别
        /// </summary>
        [Display(Name = "流程类别")]
        public short? WFType { get; set; }

        public virtual ICollection<WorkFlowActivity> WorkFlowActivitys { get; set; }
        public virtual ICollection<WorkFlowsTransition> WorkFlowsTransitions { get; set; }
        public virtual ICollection<WorkFlowRecord> WorkFlowRecords { get; set; }
    }
}
