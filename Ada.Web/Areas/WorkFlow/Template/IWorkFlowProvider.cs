using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core;

namespace WorkFlow.Template
{
   public interface IWorkFlowProvider:ISingleDependency
   {
       WorkflowApplication CreateWorkflowApp(string xamlPath, Dictionary<string, object> dicParam);
       WorkflowApplication ResumeBookMark(string xamlPath, Guid instanceId, string bookmarkName, object value);
   }
}
