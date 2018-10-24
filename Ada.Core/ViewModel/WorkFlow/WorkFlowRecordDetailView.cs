using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.ViewModel.WorkFlow
{
   public class WorkFlowRecordDetailView : BaseView
    {
        /// <summary>
        /// 步骤名称
        /// </summary>
        [Display(Name = "步骤名称")]
        public string Name { get; set; }
        /// <summary>
        /// 审批人
        /// </summary>
        [Display(Name = "审批人")]
        public string ProcessBy { get; set; }
        /// <summary>
        /// 审批人
        /// </summary>
        [Display(Name = "审批人")]
        public string ProcessById { get; set; }
        /// <summary>
        /// 审批日期
        /// </summary>
        [Display(Name = "审批日期")]
        public DateTime? ProcessDate { get; set; }
        /// <summary>
        /// 审批结果
        /// </summary>
        [Display(Name = "审批结果")]
        public string ProcessResult { get; set; }
        /// <summary>
        /// 审批意见
        /// </summary>
        [Display(Name = "审批意见")]
        public string ProcessComment { get; set; }
        /// <summary>
        /// 审批状态
        /// </summary>
        [Display(Name = "审批状态")]
        public short? Status { get; set; }
        /// <summary>
        /// 是否起始
        /// </summary>
        [Display(Name = "是否起始")]
        public bool? IsStart { get; set; }
        /// <summary>
        /// 审批状态
        /// </summary>
        [Display(Name = "审批状态")]
        public bool? IsEnd { get; set; }
        /// <summary>
        /// 上级审批
        /// </summary>
        [Display(Name = "上级审批")]
        public string ParentDetailId { get; set; }
        /// <summary>
        /// 流程记录
        /// </summary>
        [Display(Name = "流程记录")]
        public string WorkFlowRecordId { get; set; }
        /// <summary>
        /// 流转至
        /// </summary>
        [Display(Name = "流转至")]
        public string FlowTo { get; set; }
    }
}
