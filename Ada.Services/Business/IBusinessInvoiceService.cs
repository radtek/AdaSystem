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
   public interface IBusinessInvoiceService : IDependency
    {
        void Add(BusinessInvoice entity);
        void Update(BusinessInvoice entity);
        void Delete(BusinessInvoice entity);
        IQueryable<BusinessInvoice> LoadEntitiesFilter(BusinessInvoiceView viewModel);
        bool WriteOff(IEnumerable<string> businessInvoicesIds, IEnumerable<string> receivaluesIds);
        void CancleWriteOff(string id);
    }
}
