using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core;
using Ada.Core.Domain;
using Ada.Core.Domain.Admin;
using Ada.Core.Domain.Business;
using Ada.Core.Domain.Purchase;
using Ada.Core.ViewModel.Business;

namespace Ada.Services.Business
{
    public class BusinessOrderDetailService : IBusinessOrderDetailService
    {

        private readonly IRepository<BusinessOrderDetail> _repository;
        private readonly IDbContext _dbContext;
        private readonly IRepository<PurchaseOrderDetail> _purchaseOrderDetailRepository;
        public BusinessOrderDetailService(
            IDbContext dbContext,
            IRepository<BusinessOrderDetail> repository,
            IRepository<PurchaseOrderDetail> purchaseOrderDetailRepository
            )
        {
            _repository = repository;
            _purchaseOrderDetailRepository = purchaseOrderDetailRepository;
            _dbContext = dbContext;
        }


        public IQueryable<BusinessOrderDetail> LoadEntitiesFilter(BusinessOrderDetailView viewModel)
        {
            var purchaseOrderDetails = _purchaseOrderDetailRepository.LoadEntities(d => d.IsDelete == false && d.PurchaseOrder.IsDelete == false);
            var allList = _repository.LoadEntities(d => d.IsDelete == false && d.BusinessOrder.IsDelete == false);
            //条件过滤
            if (viewModel.Managers != null && viewModel.Managers.Count > 0)
            {
                allList = allList.Where(d => viewModel.Managers.Contains(d.BusinessOrder.TransactorId));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.Transactor))
            {
                allList = allList.Where(d => d.BusinessOrder.Transactor.Contains(viewModel.Transactor));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.OrderRemark))
            {
                allList = allList.Where(d => d.BusinessOrder.Remark.Contains(viewModel.OrderRemark));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.search))
            {
                allList = allList.Where(d => d.MediaName.Contains(viewModel.search));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.MediaName))
            {
                allList = allList.Where(d => d.MediaName.Contains(viewModel.MediaName));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.MediaTypeName))
            {
                allList = allList.Where(d => d.MediaTypeName.Contains(viewModel.MediaTypeName));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.AdPositionName))
            {
                allList = allList.Where(d => d.AdPositionName.Contains(viewModel.AdPositionName));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.OrderNum))
            {
                allList = allList.Where(d => d.BusinessOrder.OrderNum == viewModel.OrderNum);
            }
            if (viewModel.Status != null)
            {
                allList = allList.Where(d => d.Status == viewModel.Status);
            }
            if (viewModel.VerificationStatus != null)
            {
                allList = allList.Where(d => d.VerificationStatus == viewModel.VerificationStatus);
            }
            if (viewModel.PurchaseStatus != null)
            {

                allList = from b in allList
                          from p in purchaseOrderDetails
                          where b.Id == p.BusinessOrderDetailId && p.Status == viewModel.PurchaseStatus
                          select b;
            }

            if (viewModel.PublishDateStart != null)
            {
                allList = from b in allList
                          from p in purchaseOrderDetails
                          where b.Id == p.BusinessOrderDetailId && p.PublishDate >= viewModel.PublishDateStart
                          select b;
            }
            if (viewModel.PublishDateEnd != null)
            {
                var endDate = viewModel.PublishDateEnd.Value.AddDays(1);
                allList = from b in allList
                          from p in purchaseOrderDetails
                          where b.Id == p.BusinessOrderDetailId && p.PublishDate < endDate
                          select b;
            }
            if (viewModel.AuditStatus != null)
            {
                allList = allList.Where(d => d.AuditStatus == viewModel.AuditStatus);
            }
            viewModel.total = allList.Count();
            viewModel.TotalMoney = allList.Sum(d => d.Money);
            viewModel.TotalSellMoney = allList.Sum(d => d.SellMoney);
            viewModel.TotalPurchaseMoney = (from b in allList
                                            from p in purchaseOrderDetails
                                            where b.Id == p.BusinessOrderDetailId
                                            select p).Sum(d => d.PurchaseMoney);
            int offset = viewModel.offset ?? 0;
            int rows = viewModel.limit ?? 10;
            string order = string.IsNullOrWhiteSpace(viewModel.order) ? "desc" : viewModel.order;
            if (order == "desc")
            {
                return allList.OrderByDescending(d => d.Id).Skip(offset).Take(rows);
            }
            return allList.OrderBy(d => d.Id).Skip(offset).Take(rows);
        }
        /// <summary>
        /// 根据人员统计业务订单
        /// </summary>
        /// <param name="managers"></param>
        /// <returns></returns>
        public List<BusinessOrderDetailView> BusinessPerformance(List<Manager> managers)
        {
            List<BusinessOrderDetailView> list = new List<BusinessOrderDetailView>();
            foreach (var manager in managers)
            {
                var item = BusinessPerformance(manager.Id);
                item.Transactor = manager.UserName;
                list.Add(item);
            }
            return list;
        }
        /// <summary>
        /// 根据个人
        /// </summary>
        /// <param name="transactorId"></param>
        /// <returns></returns>
        public BusinessOrderDetailView BusinessPerformance(string transactorId)
        {
            var orders = GetBusinessOrderDetails();
            BusinessOrderDetailView item = new BusinessOrderDetailView
            {
                TransactorId = transactorId,
                TotalSellMoney = orders.Where(d => d.TransactorId == transactorId).Sum(d => d.SellMoney),
                TotalVerificationMoney =
                    orders.Where(d => d.TransactorId == transactorId).Sum(d => d.VerificationMoney),
                TotalPurchaseMoney = orders.Where(d => d.TransactorId == transactorId).Sum(d => d.PurchaseMoney)
            };
            item.Profit = ((item.TotalSellMoney - item.TotalPurchaseMoney) / item.TotalSellMoney) * 100;
            return item;
        }
        private IQueryable<BusinessOrderDetailView> GetBusinessOrderDetails()
        {
            var purchaseOrders = _purchaseOrderDetailRepository.LoadEntities(d => d.IsDelete == false && d.PurchaseOrder.IsDelete == false);
            var businessOrders = _repository.LoadEntities(d => d.IsDelete == false && d.BusinessOrder.IsDelete == false);
            return from b in businessOrders
                   from p in purchaseOrders
                   //双方都是已完成的状态
                   where p.Status == Consts.PurchaseStatusSuccess && b.Status == Consts.StateOK && b.Id == p.BusinessOrderDetailId
                   select new BusinessOrderDetailView
                   {
                       SellMoney = b.SellMoney,
                       VerificationMoney = b.VerificationMoney,
                       PurchaseMoney = p.PurchaseMoney,
                       Transactor = b.BusinessOrder.Transactor,
                       TransactorId = b.BusinessOrder.TransactorId

                   };
        }
        public void Update(BusinessOrderDetail entity)
        {
            _repository.Update(entity);
            _dbContext.SaveChanges();
        }

    }
}
