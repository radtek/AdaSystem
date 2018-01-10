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
   public interface IBusinessPaymentService:IDependency
    {
        void Add(BusinessPayment entity);
        void Update(BusinessPayment entity);
        void Delete(BusinessPayment entity);
        IQueryable<BusinessPayment> LoadEntitiesFilter(BusinessPaymentView viewModel);
        
    }
}
