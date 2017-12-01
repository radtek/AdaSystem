using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core;
using Ada.Core.Domain.Business;
using Ada.Core.ViewModel.Business;

namespace Ada.Services.Business
{
   public interface IBusinessPayeeService:IDependency
    {
        void Add(BusinessPayee entity);
        void Update(BusinessPayee entity);
        void Delete(BusinessPayee entity);
        IQueryable<BusinessPayee> LoadEntitiesFilter(BusinessPayeeView viewModel);
    }
}
