using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Ada.Core.Infrastructure;
using Ada.Services.WorkFlow;
using WorkFlow.Models;

namespace WorkFlow.Template
{

    public sealed class SetStepActivity : CodeActivity
    {
        // 定义一个字符串类型的活动输入参数
        public InArgument<string> StepName { get; set; }
        public InArgument<bool> IsEnd { get; set; }

        // 如果活动返回值，则从 CodeActivity<TResult>
        // 并从 Execute 方法返回该值。
        protected override void Execute(CodeActivityContext context)
        {
            string text = context.GetValue(this.StepName);
            bool end = context.GetValue(this.IsEnd);
            var workFlowId = context.WorkflowInstanceId.ToString();
            var service = EngineContext.Current.Resolve<IWorkFlowService>();
            var wf = service.GetRecordByWfInstanceId(workFlowId);
            if (wf == null)
            {
                throw new ApplicationException(workFlowId + "，此工作流记录不存在");
            }

            var status = (short) WorkFlowEnum.UnProecess;
            var detail = wf.WorkFlowRecordDetails.FirstOrDefault(d => d.Status == status);
            if (detail!=null)
            {
                detail.Name = text;
                detail.IsEnd = end;
                if (end)
                {
                    detail.ProcessResult = "审批结束";
                    wf.Status = (short)WorkFlowEnum.Processed;
                    service.UpdateRecord(wf);
                }
                service.UpdateDetail(detail);
            }
            
        }
    }
}
