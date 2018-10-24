using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core;
using Ada.Core.Domain.WorkFlow;

namespace Ada.Services.WorkFlow
{
   public interface IWorkFlowService: IDependency
   {
       WorkFlowRecord GetRecordByWfInstanceId(string wfInstanceId);
       void UpdateRecord(WorkFlowRecord workFlowRecord);
       void UpdateDetail(WorkFlowRecordDetail detail);
       WorkFlowRecordDetail GetDetailById(string id);
   }
}
