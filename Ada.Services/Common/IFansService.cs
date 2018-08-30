using System.Linq;
using Ada.Core;
using Ada.Core.Domain.Common;
using Ada.Core.ViewModel.Common;

namespace Ada.Services.Common
{
   public interface IFansService : IDependency
   {
       void Add(Fans entity);
       void Update(Fans entity);
       void Delete(Fans entity);
       IQueryable<Fans> LoadEntitiesFilter(FansView viewModel);
   }
}
