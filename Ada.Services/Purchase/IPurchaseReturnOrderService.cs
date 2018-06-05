using System.Linq;
using Ada.Core;
using Ada.Core.Domain.Purchase;
using Ada.Core.ViewModel.Purchase;

namespace Ada.Services.Purchase
{
    public interface IPurchaseReturnOrderService : IDependency
    {
        void Add(PurchaseReturnOrder entity);
        void Update(PurchaseReturnOrder entity);
        void Delete(PurchaseReturnOrder entity);
        IQueryable<PurchaseReturnOrder> LoadEntitiesFilter(PurchaseReturnOrderView viewModel);
    }
}
