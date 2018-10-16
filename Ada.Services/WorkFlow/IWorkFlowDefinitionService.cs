using System.Linq;
using Ada.Core;
using Ada.Core.Domain.WorkFlow;
using Ada.Core.ViewModel.WorkFlow;


namespace Ada.Services.WorkFlow
{
   public interface IWorkFlowDefinitionService : IDependency
   {
       void Add(WorkFlowDefinition entity);
       void Update(WorkFlowDefinition entity);
       void Delete(WorkFlowDefinition entity);
       IQueryable<WorkFlowDefinition> LoadEntitiesFilter(WorkFlowDefinitionView viewModel);
   }
}
