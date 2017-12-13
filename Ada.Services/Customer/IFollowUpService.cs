using System.Linq;
using Ada.Core;
using Ada.Core.Domain.Customer;
using Ada.Core.ViewModel.Customer;

namespace Ada.Services.Customer
{
   public interface IFollowUpService : IDependency
   {
       void Add(FollowUp entity);
       void Update(FollowUp entity);
       void Delete(FollowUp entity);
       IQueryable<FollowUp> LoadEntitiesFilter(FollowUpView viewModel);
   }
}
