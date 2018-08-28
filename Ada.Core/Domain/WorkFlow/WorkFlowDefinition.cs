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
        }
        /// <summary>
        /// 工作流名称
        /// </summary>
        [Display(Name = "工作流名称")]
        public string Name { get; set; }
        /// <summary>
        /// 是否开启
        /// </summary>
        [Display(Name = "是否开启")]
        public bool Enabled { get; set; }

        public virtual ICollection<WorkFlowActivity> WorkFlowActivitys { get; set; }
        public virtual ICollection<WorkFlowsTransition> WorkFlowsTransitions { get; set; }
    }
}
