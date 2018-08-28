using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.Domain.WorkFlow
{
   public class WorkFlowActivity : BaseEntity
    {
        public WorkFlowActivity()
        {
            IsStart = false;
        }
        /// <summary>
        /// 节点名称
        /// </summary>
        [Display(Name = "节点名称")]
        public string Name { get; set; }
        /// <summary>
        /// X
        /// </summary>
        [Display(Name = "X")]
        public int? X { get; set; }
        /// <summary>
        /// Y
        /// </summary>
        [Display(Name = "Y")]
        public int? Y { get; set; }
        /// <summary>
        /// 节点参数
        /// </summary>
        [Display(Name = "节点参数")]
        public string Parameter { get; set; }
        /// <summary>
        /// 是否起点
        /// </summary>
        [Display(Name = "是否起点")]
        public bool IsStart { get; set; }
        /// <summary>
        /// 工作流
        /// </summary>
        [Display(Name = "工作流")]
        public string WorkFlowDefinitionId { get; set; }
        public virtual WorkFlowDefinition WorkFlowDefinition { get; set; }

    }
}
