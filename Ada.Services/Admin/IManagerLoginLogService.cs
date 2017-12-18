using System.Linq;
using Ada.Core;
using Ada.Core.Domain.Log;
using Ada.Core.ViewModel.Admin;

namespace Ada.Services.Admin
{
   public interface IManagerLoginLogService : IDependency
   {
       
       void Delete(ManagerLoginLog entity);
       IQueryable<ManagerLoginLog> LoadEntitiesFilter(ManagerLoginLogView viewModel);
   }
}
