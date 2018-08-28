using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.Domain.WorkFlow
{
   public class WorkFlowsTransition:BaseEntity
    {
        /// <summary>
        /// 起始端点
        /// </summary>
        [Display(Name = "起始端点")]
        public string SourceEndpoint { get; set; }
        /// <summary>
        /// 目的端点
        /// </summary>
        [Display(Name = "目的端点")]
        public string DestinationEndpoint { get; set; }
        /// <summary>
        /// 起始节点
        /// </summary>
        [Display(Name = "起始节点")]
        public string SourceActivityId { get; set; }
        /// <summary>
        /// 目的节点
        /// </summary>
        [Display(Name = "目的节点")]
        public string DestinationActivityId { get; set; }
        /// <summary>
        /// 工作流
        /// </summary>
        [Display(Name = "工作流")]
        public string WorkFlowDefinitionId { get; set; }
        public virtual WorkFlowDefinition WorkFlowDefinition { get; set; }
    }
}
