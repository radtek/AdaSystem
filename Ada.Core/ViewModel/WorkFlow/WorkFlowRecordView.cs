using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.ViewModel.WorkFlow
{
   public class WorkFlowRecordView: BaseView
    {
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
        /// 流程实例ID
        /// </summary>
        [Display(Name = "流程实例ID")]
        public string WfInstanceId { get; set; }
        /// <summary>
        /// 工作流程
        /// </summary>
        [Display(Name = "工作流程")]
        public string WorkFlowDefinitionId { get; set; }
        /// <summary>
        /// 流程类型
        /// </summary>
        [Display(Name = "流程类型")]
        public string WorkFlowDefinitionName { get; set; }
        /// <summary>
        /// 流程说明
        /// </summary>
        [Display(Name = "流程说明")]
        public string WorkFlowDefinitionDescription { get; set; }
        /// <summary>
        /// 流转至
        /// </summary>
        [Display(Name = "流转至")]
        public string FlowTo { get; set; }
    }
}
