using System.Linq;
using Ada.Core;
using Ada.Core.Domain.Purchase;
using Ada.Core.ViewModel.Purchase;

namespace Ada.Services.Purchase
{
    public interface IPurchasePaymentDetailService : IDependency
    {
        void Update(PurchasePaymentDetail entity);
        void Delete(PurchasePaymentDetail entity);
        IQueryable<PurchasePaymentDetail> LoadEntitiesFilter(PurchasePaymentDetailView viewModel);
    }
}
