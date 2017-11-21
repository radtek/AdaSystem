using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core;
using Ada.Core.Domain.Business;
using Ada.Core.Domain.Purchase;
using Ada.Core.ViewModel.Business;

namespace Ada.Services.Purchase
{
    public interface IPurchaseOrderService : IDependency
    {
        void Add(PurchaseOrder entity);
        void Update(PurchaseOrder entity);
        void Delete(PurchaseOrder entity);
      
    }
}
