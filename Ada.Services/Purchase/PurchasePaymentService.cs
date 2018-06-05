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
        public IQueryable<PurchasePaymentView> LoadEntitiesFilter(PurchasePaymentView viewModel)
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
            if (!string.IsNullOrWhiteSpace(viewModel.BankAccount))
            {
                allList = allList.Where(d => d.PurchasePaymentDetails.Any(p=>p.AccountName.Contains(viewModel.BankAccount)));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.Transactor))
            {
                allList = allList.Where(d => d.Transactor.Contains(viewModel.Transactor));
            }
            if (viewModel.InvoiceStauts != null)
            {
                allList = allList.Where(d => d.InvoiceStauts == viewModel.InvoiceStauts);
            }
            if (!string.IsNullOrWhiteSpace(viewModel.InvoiceCompany))
            {
                allList = from p in allList
                          from d in p.PurchasePaymentDetails
                          where d.AccountName.Contains(viewModel.InvoiceCompany)
                          select p;
            }
            if (!string.IsNullOrWhiteSpace(viewModel.MediaName))
            {
                allList = from p in allList
                          from d in p.PurchasePaymentOrderDetails
                          where d.PurchaseOrderDetail.MediaName.Contains(viewModel.MediaName)
                          select p;
            }
            if (viewModel.IsInvoice != null)
            {
                allList = allList.Where(d => d.IsInvoice == viewModel.IsInvoice);
            }
            if (viewModel.InvoiceDateStart != null)
            {
                allList = allList.Where(d => d.InvoiceDate >= viewModel.InvoiceDateStart);
            }
            if (viewModel.InvoiceDateEnd != null)
            {
                var endDate = viewModel.InvoiceDateEnd.Value.AddDays(1);
                allList = allList.Where(d => d.InvoiceDate < endDate);
            }
            if (viewModel.BillDateStart != null)
            {
                allList = allList.Where(d => d.BillDate >= viewModel.BillDateStart);
            }
            if (viewModel.BillDateEnd != null)
            {
                var endDate = viewModel.BillDateEnd.Value.AddDays(1);
                allList = allList.Where(d => d.BillDate < endDate);
            }
            var temp = from a in allList
                       select new PurchasePaymentView
                       {
                           Id = a.Id,
                           RequstMoney = a.PurchasePaymentDetails.Sum(d => d.PayMoney),
                           LinkManName = a.LinkManName,
                           Transactor = a.Transactor,
                           BillDate = a.BillDate,
                           BillNum = a.BillNum,
                           PayMoney = a.PayMoney,
                           Tax = a.Tax,
                           DiscountMoney = a.DiscountMoney,
                           InvoiceTitle = a.InvoiceTitle,
                           InvoiceStauts = a.InvoiceStauts,
                           InvoiceDate = a.InvoiceDate,
                           InvoiceNum = a.InvoiceNum,
                           IsInvoice = a.IsInvoice,
                           TaxMoney = a.PurchasePaymentDetails.Sum(d => d.PayMoney) - a.PurchasePaymentDetails.Sum(d => d.PayMoney) / (1 + a.Tax / 100)

                       };
            viewModel.TotalRequestMoney = temp.Sum(d => d.RequstMoney);
            viewModel.TotalTaxMoney = temp.Sum(d=>d.TaxMoney);
           
            viewModel.total = temp.Count();
            int offset = viewModel.offset ?? 0;
            int rows = viewModel.limit ?? 10;
            string order = string.IsNullOrWhiteSpace(viewModel.order) ? "desc" : viewModel.order;
            if (order == "desc")
            {
                return temp.OrderByDescending(d => d.Id).Skip(offset).Take(rows);
            }
            return temp.OrderBy(d => d.Id).Skip(offset).Take(rows);
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
