using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core;
using Ada.Core.Domain.Finance;
using Ada.Core.ViewModel.Finance;

namespace Ada.Services.Finance
{
    public interface IBillPaymentService : IDependency
    {
        void Add(BillPayment entity);
        void Update(BillPayment entity);
        void Delete(BillPayment entity);
        IQueryable<BillPayment> LoadEntitiesFilter(BillPaymentView viewModel);
        BillPayment GetByRequestNum(string requestNum);
    }
}
