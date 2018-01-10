using System.Linq;
using Ada.Core;
using Ada.Core.Domain.Purchase;
using Ada.Core.ViewModel.Purchase;

namespace Ada.Services.Purchase
{
    public interface IPurchasePaymentService : IDependency
    {
        void Add(PurchasePayment entity);
        void Update(PurchasePayment entity);
        void Delete(PurchasePayment entity);
        IQueryable<PurchasePaymentView> LoadEntitiesFilter(PurchasePaymentView viewModel);
    }
}
