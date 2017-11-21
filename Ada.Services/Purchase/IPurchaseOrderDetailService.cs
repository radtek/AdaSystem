using System.Linq;
using Ada.Core;
using Ada.Core.Domain.Purchase;
using Ada.Core.ViewModel.Purchase;

namespace Ada.Services.Purchase
{
    public interface IPurchaseOrderDetailService : IDependency
    {
        void Add(PurchaseOrderDetail entity);
        void Update(PurchaseOrderDetail entity);
        void Delete(PurchaseOrderDetail entity);
        IQueryable<PurchaseOrderDetail> LoadEntitiesFilter(PurchaseOrderDetailView viewModel);
    }
}
