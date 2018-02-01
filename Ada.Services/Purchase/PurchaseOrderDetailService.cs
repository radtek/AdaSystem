using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core;
using Ada.Core.Domain;
using Ada.Core.Domain.Business;
using Ada.Core.Domain.Purchase;
using Ada.Core.ViewModel.Admin;
using Ada.Core.ViewModel.Business;
using Ada.Core.ViewModel.Purchase;
using Ada.Core.ViewModel.Statistics;

namespace Ada.Services.Purchase
{
    public class PurchaseOrderDetailService : IPurchaseOrderDetailService
    {
        private readonly IDbContext _dbContext;
        private readonly IRepository<PurchaseOrderDetail> _repository;
        private readonly IRepository<BusinessOrderDetail> _businessRepository;
        private readonly IRepository<PurchasePaymentDetail> _purchasePaymentDetailRepository;
        private readonly IRepository<PurchasePaymentOrderDetail> _purchasePaymentRepository;
        public PurchaseOrderDetailService(IDbContext dbContext,
            IRepository<PurchaseOrderDetail> repository,
            IRepository<BusinessOrderDetail> businessRepository,
            IRepository<PurchasePaymentDetail> purchasePaymentDetailRepository,
            IRepository<PurchasePaymentOrderDetail> purchasePaymentRepository)
        {
            _dbContext = dbContext;
            _repository = repository;
            _businessRepository = businessRepository;
            _purchasePaymentDetailRepository = purchasePaymentDetailRepository;
            _purchasePaymentRepository = purchasePaymentRepository;
        }
        public IQueryable<PurchaseOrderDetail> LoadEntitiesFilter(PurchaseOrderDetailView viewModel)
        {
            var allList = _repository.LoadEntities(d => d.IsDelete == false);
            var business =
                _businessRepository.LoadEntities(d => d.IsDelete == false && d.BusinessOrder.IsDelete == false);
            //条件过滤
            if (viewModel.Managers != null && viewModel.Managers.Count > 0)
            {
                allList = allList.Where(d => viewModel.Managers.Contains(d.TransactorId));
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
            if (!string.IsNullOrWhiteSpace(viewModel.BusinessRemark))
            {
                allList = from p in allList
                          from b in business
                          where p.BusinessOrderDetailId == b.Id && b.BusinessOrder.Remark.Contains(viewModel.BusinessRemark)
                          select p;
            }
            if (!string.IsNullOrWhiteSpace(viewModel.BusinessBy))
            {
                allList = allList.Where(d => d.PurchaseOrder.BusinessBy.Contains(viewModel.BusinessBy));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.LinkManName))
            {
                allList = allList.Where(d => d.LinkManName.Contains(viewModel.LinkManName));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.LinkManId))
            {
                allList = allList.Where(d => d.LinkManId.Contains(viewModel.LinkManId));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.Transactor))
            {
                allList = allList.Where(d => d.Transactor.Contains(viewModel.Transactor));
            }
            if (viewModel.Status != null)
            {
                allList = allList.Where(d => d.Status == viewModel.Status);
            }
            if (viewModel.AuditStatus != null)
            {
                allList = allList.Where(d => d.AuditStatus == viewModel.AuditStatus);
            }
            if (viewModel.IsPayment == false)//过滤没有请款的
            {
                allList = allList.Where(d => d.PurchasePaymentOrderDetails.Count == 0);
            }
            if (viewModel.PublishDateStart != null)
            {
                allList = allList.Where(d => d.PublishDate >= viewModel.PublishDateStart);
            }
            if (viewModel.PublishDateEnd != null)
            {
                var endDate = viewModel.PublishDateEnd.Value.AddDays(1);
                allList = allList.Where(d => d.PublishDate < endDate);
            }
            viewModel.total = allList.Count();
            viewModel.TotalMoney = allList.Sum(d => d.Money);
            viewModel.TotalPurchaseMoney = allList.Sum(d => d.PurchaseMoney);
            viewModel.TotalTaxMoney = allList.Sum(d => d.TaxMoney);
            viewModel.TotalDiscountMoney = allList.Sum(d => d.DiscountMoney);
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
        /// 业绩统计
        /// </summary>
        /// <param name="managers"></param>
        /// <returns></returns>
        public IEnumerable<PurchaseAchievement> PurchasePerformance(List<ManagerView> managers)
        {
            List<PurchaseAchievement> list = new List<PurchaseAchievement>();
            foreach (var managerView in managers)
            {
                PurchaseAchievement purchaseAchievement = new PurchaseAchievement();
                purchaseAchievement.Transactor = managerView.UserName;
                var purchases = _repository.LoadEntities(d => d.IsDelete == false && d.TransactorId == managerView.Id);
                purchaseAchievement.OrderCount = purchases.Count();
                //需付款的金额
                purchaseAchievement.TotalMoney = purchases.Sum(d => d.Money);
                //实际付款金额
                var payment = _purchasePaymentDetailRepository.LoadEntities(d => d.PurchasePayment.IsDelete == false &&
                    d.IsDelete == false && d.AuditStatus == Consts.StateNormal && d.PurchasePayment.TransactorId == managerView.Id);
                purchaseAchievement.PayMoney = payment.Sum(d => d.PayMoney);
                //压款金额
                purchaseAchievement.Unpaid = (purchaseAchievement.TotalMoney ?? 0) - (purchaseAchievement.PayMoney ?? 0);
                //节省金额
                purchaseAchievement.Money = _purchasePaymentRepository.LoadEntities(d => d.IsDelete == false && d.PurchasePayment.IsDelete == false && d.PurchaseOrderDetail.IsDelete == false && d.PurchaseOrderDetail.TransactorId == managerView.Id).Sum(d => d.PurchaseOrderDetail.Money);
                //var paymentPurchases = from p in payment
                //                       from o in p.PurchasePayment.PurchasePaymentOrderDetails
                //                       where o.PurchaseOrderDetail.Status == Consts.PurchaseStatusSuccess
                //                       select p;
                //var totalSuccessPayMoney = paymentPurchases.Distinct().Sum(d => d.PayMoney);
                purchaseAchievement.Economize = (purchaseAchievement.Money ?? 0) - (purchaseAchievement.PayMoney ?? 0);
                list.Add(purchaseAchievement);
            }
            return list;
        }
        public void Add(PurchaseOrderDetail entity)
        {
            _repository.Add(entity);
            _dbContext.SaveChanges();
        }

        public void Update(PurchaseOrderDetail entity)
        {
            _repository.Update(entity);
            _dbContext.SaveChanges();
        }

        public void Delete(PurchaseOrderDetail entity)
        {
            _repository.Remove(entity);
            _dbContext.SaveChanges();
        }
    }
}
