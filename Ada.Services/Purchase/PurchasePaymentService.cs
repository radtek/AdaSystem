using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core;
using Ada.Core.Domain.Business;
using Ada.Core.Domain.Purchase;
using Ada.Core.ViewModel.Business;
using Ada.Core.ViewModel.Purchase;

namespace Ada.Services.Purchase
{
    public class PurchasePaymentService : IPurchasePaymentService
    {
        private readonly IDbContext _dbContext;
        private readonly IRepository<PurchasePayment> _repository;
        private readonly IRepository<PurchasePaymentDetail> _purchasePaymentDetailRepository;
        private readonly IRepository<PurchasePaymentOrderDetail> _purchasePaymentOrderDetailRepository;
        public PurchasePaymentService(IDbContext dbContext,
            IRepository<PurchasePayment> repository,
            IRepository<PurchasePaymentDetail> purchasePaymentDetailRepository,
            IRepository<PurchasePaymentOrderDetail> purchasePaymentOrderDetailRepository)
        {
            _dbContext = dbContext;
            _repository = repository;
            _purchasePaymentDetailRepository = purchasePaymentDetailRepository;
            _purchasePaymentOrderDetailRepository = purchasePaymentOrderDetailRepository;
        }
        public IQueryable<PurchasePayment> LoadEntitiesFilter(PurchasePaymentView viewModel)
        {
            var allList = _repository.LoadEntities(d => d.IsDelete == false);
            //条件过滤
            if (viewModel.Managers != null && viewModel.Managers.Count > 0)
            {
                allList = allList.Where(d => viewModel.Managers.Contains(d.TransactorId));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.search))
            {
                allList = allList.Where(d => d.LinkManName.Contains(viewModel.search));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.Transactor))
            {
                allList = allList.Where(d => d.Transactor.Contains(viewModel.Transactor));
            }
            if (viewModel.IsInvoice!=null)
            {
                allList = allList.Where(d => d.IsInvoice==viewModel.IsInvoice);
            }
            viewModel.total = allList.Count();
            int offset = viewModel.offset ?? 0;
            int rows = viewModel.limit ?? 10;
            string order = string.IsNullOrWhiteSpace(viewModel.order) ? "desc" : viewModel.order;
            if (order == "desc")
            {
                return allList.OrderByDescending(d => d.Id).Skip(offset).Take(rows);
            }
            return allList.OrderBy(d => d.Id).Skip(offset).Take(rows);
        }

        public void Add(PurchasePayment entity)
        {
            _repository.Add(entity);
            _dbContext.SaveChanges();
        }

        public void Update(PurchasePayment entity)
        {
            _repository.Update(entity);
            _dbContext.SaveChanges();
        }

        public void Delete(PurchasePayment entity)
        {
            _purchasePaymentDetailRepository.Remove(entity.PurchasePaymentDetails);
            _purchasePaymentOrderDetailRepository.Remove(entity.PurchasePaymentOrderDetails);
            _repository.Remove(entity);
            _dbContext.SaveChanges();
        }
    }
}
