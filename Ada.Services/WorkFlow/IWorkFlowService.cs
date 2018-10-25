using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core;
using Ada.Core.Domain.WorkFlow;
using Ada.Core.ViewModel.WorkFlow;

namespace Ada.Services.WorkFlow
{
   public interface IWorkFlowService: IDependency
   {
       WorkFlowRecord GetRecordByWfInstanceId(string wfInstanceId);
       WorkFlowRecord GetRecordById(string id);
       void UpdateRecord(WorkFlowRecord workFlowRecord);
       void UpdateDetail(WorkFlowRecordDetail detail);
       void DeleteRecord(string id);
       WorkFlowRecordDetail GetDetailById(string id);
       IQueryable<WorkFlowRecord> LoadRecordsFilter(WorkFlowRecordView viewModel);
   }
}
