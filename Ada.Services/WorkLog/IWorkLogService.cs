using System.Linq;
using Ada.Core;
using Ada.Core.Domain.Customer;
using Ada.Core.ViewModel.Customer;
using Ada.Core.ViewModel.WorkLog;

namespace Ada.Services.WorkLog
{
   public interface IWorkLogService : IDependency
   {
       void Add(Core.Domain.Log.WorkLog entity);
       void Update(Core.Domain.Log.WorkLog entity);
       void Delete(Core.Domain.Log.WorkLog entity);
       IQueryable<Core.Domain.Log.WorkLog> LoadEntitiesFilter(WorkLogView viewModel);
   }
}
