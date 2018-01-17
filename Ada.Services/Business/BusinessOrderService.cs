using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core;
using Ada.Core.Domain;
using Ada.Core.Domain.Business;
using Ada.Core.Domain.Purchase;
using Ada.Core.ViewModel.Business;

namespace Ada.Services.Business
{
    public class BusinessOrderService : IBusinessOrderService
    {
        private readonly IDbContext _dbContext;
        private readonly IRepository<BusinessOrder> _repository;
        private readonly IRepository<BusinessOrderDetail> _detailRepository;
        private readonly IRepository<PurchaseOrder> _purchaseOrderRepository;
        public BusinessOrderService(IDbContext dbContext,
            IRepository<BusinessOrder> repository,
            IRepository<BusinessOrderDetail> detailRepository,
            IRepository<PurchaseOrder> purchaseOrderRepository)
        {
            _dbContext = dbContext;
            _repository = repository;
            _detailRepository = detailRepository;
            _purchaseOrderRepository = purchaseOrderRepository;
        }


        public IQueryable<BusinessOrder> LoadEntitiesFilter(BusinessOrderView viewModel)
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
            if (!string.IsNullOrWhiteSpace(viewModel.LinkManName))
            {
                allList = allList.Where(d => d.LinkManName.Contains(viewModel.LinkManName));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.LinkManId))
            {
                allList = allList.Where(d => d.LinkManId == viewModel.LinkManId);
            }
            if (!string.IsNullOrWhiteSpace(viewModel.Remark))
            {
                allList = allList.Where(d => d.Remark.Contains(viewModel.Remark));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.Transactor))
            {
                allList = allList.Where(d => d.Transactor.Contains(viewModel.Transactor));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.MediaName))
            {
                allList = from o in allList
                          from d in o.BusinessOrderDetails
                          where d.MediaName.Contains(viewModel.MediaName)
                          select o;
            }
            if (viewModel.OrderStatus != null)
            {
                if (viewModel.OrderStatus.Value)
                {
                    allList = from o in allList
                              where o.Status == Consts.StateNormal && o.BusinessOrderDetails.Count == o.BusinessOrderDetails.Count(b => b.Status == Consts.StateOK) && o.BusinessOrderDetails.Count > 0
                              select o;
                }
                else
                {
                    allList = from o in allList
                              where o.BusinessOrderDetails.Count != o.BusinessOrderDetails.Count(b => b.Status == Consts.StateOK) || o.BusinessOrderDetails.Count == 0
                              select o;
                }

            }

            if (viewModel.VerificationStatus != null)
            {
                if (viewModel.VerificationStatus.Value)
                {
                    allList = from o in allList
                        where o.BusinessOrderDetails.Count == o.BusinessOrderDetails.Count(b => b.VerificationStatus == Consts.StateNormal) && o.BusinessOrderDetails.Count > 0
                        select o;
                }
                else
                {
                    allList = from o in allList
                        where o.BusinessOrderDetails.Count != o.BusinessOrderDetails.Count(b => b.VerificationStatus == Consts.StateNormal) || o.BusinessOrderDetails.Count >0
                        select o;
                }
               
            }
            if (viewModel.AuditStatus != null)
            {
                allList = allList.Where(d => d.AuditStatus == viewModel.AuditStatus);
            }
            if (!string.IsNullOrWhiteSpace(viewModel.OrderNum))
            {
                allList = allList.Where(d => d.OrderNum == viewModel.OrderNum);
            }
            if (viewModel.IsInvoice == false)
            {
                allList = allList.Where(d => d.BusinessInvoiceDetails.Count == 0);
            }
            viewModel.total = allList.Count();
            viewModel.AllMoney = allList.Sum(d => d.BusinessOrderDetails.Where(a => a.Status == Consts.StateNormal).Sum(b => b.Money));
            viewModel.AllSellMoney = allList.Sum(d => d.BusinessOrderDetails.Where(a => a.Status == Consts.StateNormal).Sum(b => b.SellMoney));
            viewModel.AllTaxMoney = allList.Sum(d => d.BusinessOrderDetails.Where(a => a.Status == Consts.StateNormal).Sum(b => b.TaxMoney));
            var purchases = _purchaseOrderRepository.LoadEntities(d => d.IsDelete == false);
            viewModel.AllPurchaseMoney = (from b in allList
                                          from p in purchases
                                          where b.Id == p.BusinessOrderId
                                          select p).Sum(d => d.PurchaseOrderDetails.Sum(o => o.PurchaseMoney));
            int offset = viewModel.offset ?? 0;
            int rows = viewModel.limit ?? 10;
            string order = string.IsNullOrWhiteSpace(viewModel.order) ? "desc" : viewModel.order;
            if (order == "desc")
            {
                return allList.OrderByDescending(d => d.Id).Skip(offset).Take(rows);
            }
            return allList.OrderBy(d => d.Id).Skip(offset).Take(rows);
        }
        public void Add(BusinessOrder entity)
        {
            _repository.Add(entity);
            _dbContext.SaveChanges();
        }

        public void Update(BusinessOrder entity)
        {
            _repository.Update(entity);
            _dbContext.SaveChanges();
        }

        public void Delete(BusinessOrder entity)
        {
            _repository.Delete(entity);
            _dbContext.SaveChanges();
        }
        public void Remove(BusinessOrder entity)
        {
            if (entity.BusinessOrderDetails.Count > 0)
            {
                _detailRepository.Remove(entity.BusinessOrderDetails);
            }
            _repository.Remove(entity);
            _dbContext.SaveChanges();
        }
    }
}
