using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Ada.Core;
using Ada.Core.Domain;
using Ada.Core.Domain.Admin;
using Ada.Core.Domain.Business;
using Ada.Core.Domain.Purchase;
using Ada.Core.ViewModel.Admin;
using Ada.Core.ViewModel.Business;
using Ada.Core.ViewModel.Statistics;

namespace Ada.Services.Business
{
    public class BusinessOrderDetailService : IBusinessOrderDetailService
    {

        private readonly IRepository<BusinessOrderDetail> _repository;
        private readonly IDbContext _dbContext;
        private readonly IRepository<Manager> _managerRepository;
        private readonly IRepository<Organization> _organizationRepository;
        private readonly IRepository<PurchaseOrderDetail> _purchaseOrderDetailRepository;
        public BusinessOrderDetailService(
            IDbContext dbContext,
            IRepository<BusinessOrderDetail> repository,
            IRepository<PurchaseOrderDetail> purchaseOrderDetailRepository,
            IRepository<Manager> managerRepository,
            IRepository<Organization> organizationRepository
            )
        {
            _repository = repository;
            _purchaseOrderDetailRepository = purchaseOrderDetailRepository;
            _dbContext = dbContext;
            _managerRepository = managerRepository;
            _organizationRepository = organizationRepository;
        }


        public IQueryable<BusinessOrderDetail> LoadEntitiesFilter(BusinessOrderDetailView viewModel)
        {
            var purchaseOrderDetails = _purchaseOrderDetailRepository.LoadEntities(d => d.IsDelete == false);
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
            if (!string.IsNullOrWhiteSpace(viewModel.LinkManName))
            {
                allList = allList.Where(d => d.BusinessOrder.LinkManName.Contains(viewModel.LinkManName));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.CompanyName))
            {
                allList = allList.Where(d => d.BusinessOrder.LinkMan.Commpany.Name.Contains(viewModel.CompanyName));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.LinkManId))
            {
                allList = allList.Where(d => d.BusinessOrder.LinkManId == viewModel.LinkManId);
            }
            if (!string.IsNullOrWhiteSpace(viewModel.OrderRemark))
            {
                allList = allList.Where(d => d.BusinessOrder.Remark.Contains(viewModel.OrderRemark));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.BusinessOrderId))
            {
                var ids = viewModel.BusinessOrderId.Split(',').ToList();
                allList = allList.Where(d => ids.Contains(d.BusinessOrderId));
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
            if (!string.IsNullOrWhiteSpace(viewModel.MediaTypeId))
            {
                allList = allList.Where(d => d.MediaPrice.Media.MediaTypeId == viewModel.MediaTypeId);
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
            if (viewModel.PrePublishDateStart != null)
            {
                allList = allList.Where(d => d.PrePublishDate >= viewModel.PrePublishDateStart);
            }
            if (viewModel.PrePublishDateEnd != null)
            {
                var endDate = viewModel.PrePublishDateEnd.Value.AddDays(1);
                allList = allList.Where(d => d.PrePublishDate < endDate);
            }
            if (viewModel.StartSellMoney != null)
            {
                allList = allList.Where(d => d.SellMoney >= viewModel.StartSellMoney);
            }
            if (viewModel.EndSellMoney != null)
            {
                allList = allList.Where(d => d.SellMoney <= viewModel.EndSellMoney);
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
            if (viewModel.IsOrderLater == true)
            {

                allList = from b in allList
                          from p in purchaseOrderDetails
                          where b.Id == p.BusinessOrderDetailId && p.Status != Consts.PurchaseStatusFail && p.PublishDate < p.AddedDate && p.MediaPrice.Media.MediaType.IsComment == true
                          select b;
            }
            if (!string.IsNullOrWhiteSpace(viewModel.MediaByPurchase))
            {

                allList = from b in allList
                          from p in purchaseOrderDetails
                          where b.Id == p.BusinessOrderDetailId && p.Transactor.Contains(viewModel.MediaByPurchase)
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
            if (viewModel.IsComment != null)
            {
                if (viewModel.IsComment.Value)
                {
                    allList = allList.Where(d => d.OrderDetailComments.Count > 0 || d.MediaPrice.Media.MediaType.IsComment == false);
                }

            }
            viewModel.total = allList.Count();
            viewModel.TotalMoney = allList.Sum(d => d.Money);
            viewModel.TotalSellMoney = allList.Sum(d => d.SellMoney);
            viewModel.TotalPurchaseMoney = (from b in allList
                                            from p in purchaseOrderDetails
                                            where b.Id == p.BusinessOrderDetailId
                                            select p).Sum(d => d.PurchaseMoney);
            var returnMoney = (from b in allList
                               from p in purchaseOrderDetails
                               where b.Id == p.BusinessOrderDetailId
                               select p).Sum(d => d.PurchaseReturenOrderDetails.Sum(p => p.Money));
            viewModel.TotalPurchaseMoney = viewModel.TotalPurchaseMoney - (returnMoney ?? 0);
            int offset = viewModel.offset ?? 0;
            int rows = viewModel.limit ?? 10;
            if (!string.IsNullOrWhiteSpace(viewModel.PublishDateOrder))
            {
                if (viewModel.PublishDateOrder.Trim().ToLower() == "asc")
                {
                    return (from b in allList
                            from p in purchaseOrderDetails
                            where b.Id == p.BusinessOrderDetailId
                            orderby p.PublishDate
                            select b).Skip(offset).Take(rows);
                }
                if (viewModel.PublishDateOrder.Trim().ToLower() == "desc")
                {
                    return (from b in allList
                            from p in purchaseOrderDetails
                            where b.Id == p.BusinessOrderDetailId
                            orderby p.PublishDate descending
                            select b).Skip(offset).Take(rows);
                }
            }
            string order = string.IsNullOrWhiteSpace(viewModel.order) ? "desc" : viewModel.order;
            if (order == "desc")
            {
                switch (viewModel.sort)
                {
                    case "SellMoney":
                        return allList.OrderByDescending(d => d.SellMoney).Skip(offset).Take(rows);
                    case "Money":
                        return allList.OrderByDescending(d => d.Money).Skip(offset).Take(rows);
                    case "PrePublishDate":
                        return allList.OrderByDescending(d => d.PrePublishDate).Skip(offset).Take(rows);
                    default:
                        return allList.OrderByDescending(d => d.Id).Skip(offset).Take(rows);
                }

            }

            switch (viewModel.sort)
            {
                case "SellMoney":
                    return allList.OrderBy(d => d.SellMoney).Skip(offset).Take(rows);
                case "Money":
                    return allList.OrderBy(d => d.Money).Skip(offset).Take(rows);
                case "PrePublishDate":
                    return allList.OrderBy(d => d.PrePublishDate).Skip(offset).Take(rows);
                default:
                    return allList.OrderBy(d => d.Id).Skip(offset).Take(rows);
            }

        }
        /// <summary>
        /// 销售业绩统计(按经办人分组)
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        public IQueryable<BusinessPerformance> BusinessPerformanceGroupByUser(BusinessOrderDetailView viewModel)
        {
            //var purchaseOrders = _purchaseOrderDetailRepository.LoadEntities(d => d.IsDelete == false && d.PurchaseOrder.IsDelete == false&&d.TransactorId!= "X1712181402100028");
            var purchaseOrders = _purchaseOrderDetailRepository.LoadEntities(d => d.IsDelete == false && d.PurchaseOrder.IsDelete == false);
            var businessOrders = _repository.LoadEntities(d => d.IsDelete == false && d.BusinessOrder.IsDelete == false);
            //过滤日期
            if (viewModel.PublishDateStart != null && viewModel.PublishDateEnd != null)
            {
                var endDay = viewModel.PublishDateEnd.Value.AddDays(1);
                purchaseOrders = purchaseOrders.Where(d =>
                    d.PublishDate >= viewModel.PublishDateStart && d.PublishDate < endDay);
            }
            if (!string.IsNullOrWhiteSpace(viewModel.MediaTypeId))
            {
                purchaseOrders =
                    purchaseOrders.Where(d => d.MediaPrice.Media.MediaType.CallIndex == viewModel.MediaTypeId);
            }
            //过滤部门
            if (!string.IsNullOrWhiteSpace(viewModel.OrganizationName))
            {
                var organization = _organizationRepository
                    .LoadEntities(d => d.OrganizationName == viewModel.OrganizationName).FirstOrDefault();
                if (organization != null)
                {
                    var mangers = _managerRepository.LoadEntities(d => d.IsDelete == false);
                    businessOrders = from b in businessOrders
                                     from m in mangers
                                     where b.BusinessOrder.TransactorId == m.Id && m.Organizations.Any(o => o.TreePath.Contains(organization.Id))
                                     select b;
                }

            }
            
            return (from b in businessOrders
                    from p in purchaseOrders
                        //双方都是已完成的状态
                    where p.Status == Consts.PurchaseStatusSuccess &&
                          b.Status == Consts.StateOK &&
                          b.Id == p.BusinessOrderDetailId
                    select new BusinessOrderDetailView
                    {
                        SellMoney = b.SellMoney,
                        VerificationMoney = b.VerificationMoney,
                        ConfirmVerificationMoney = b.ConfirmVerificationMoney,
                        PurchaseMoney = p.PurchaseMoney - (p.PurchaseReturenOrderDetails.Where(a => a.PurchaseReturnOrder.AuditStatus == Consts.StateNormal).Sum(d => d.Money) ?? 0),
                        ProfitMoney = b.SellMoney - (p.PurchaseMoney - (p.PurchaseReturenOrderDetails.Where(a => a.PurchaseReturnOrder.AuditStatus == Consts.StateNormal).Sum(d => d.Money) ?? 0)),
                        Transactor = b.BusinessOrder.Transactor,
                        TransactorId = b.BusinessOrder.TransactorId
                    }).GroupBy(d => d.Transactor).Select(d => new BusinessPerformance
                    {
                        Transactor = d.Key,
                        TotalSellMoney = d.Sum(o => o.SellMoney),
                        TotalVerificationMoney = d.Sum(o => o.VerificationMoney),
                        TotalConfirmVerificationMoney = d.Sum(o => o.ConfirmVerificationMoney),
                        TotalPurchaseMoney = d.Sum(o => o.PurchaseMoney),
                        TotalProfitMoney = d.Sum(o => o.ProfitMoney),
                        Profit = d.Sum(o => o.SellMoney) == 0 ? 0 : d.Sum(o => o.ProfitMoney) / d.Sum(o => o.SellMoney) * 100,

                    });
        }
        /// <summary>
        /// 销售业绩统计(按月份分组)
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        public IQueryable<BusinessPerformance> BusinessPerformanceGroupByDate(BusinessOrderDetailView viewModel)
        {
            var purchaseOrders = _purchaseOrderDetailRepository.LoadEntities(d => d.IsDelete == false && d.PurchaseOrder.IsDelete == false);
            var businessOrders = _repository.LoadEntities(d => d.IsDelete == false && d.BusinessOrder.IsDelete == false);
            if (!string.IsNullOrWhiteSpace(viewModel.TransactorId))
            {
                businessOrders = businessOrders.Where(d => d.BusinessOrder.TransactorId == viewModel.TransactorId);
            }
            return (from b in businessOrders
                    from p in purchaseOrders
                        //双方都是已完成的状态
                    where p.Status == Consts.PurchaseStatusSuccess &&
                          b.Status == Consts.StateOK &&
                          b.Id == p.BusinessOrderDetailId&&
                          p.PublishDate!=null
                    select new BusinessOrderDetailView
                    {
                        SellMoney = b.SellMoney,
                        VerificationMoney = b.VerificationMoney,
                        ConfirmVerificationMoney = b.ConfirmVerificationMoney,
                        PurchaseMoney = p.PurchaseMoney - (p.PurchaseReturenOrderDetails.Where(a => a.PurchaseReturnOrder.AuditStatus == Consts.StateNormal).Sum(d => d.Money) ?? 0),
                        ProfitMoney = b.SellMoney - (p.PurchaseMoney - (p.PurchaseReturenOrderDetails.Where(a => a.PurchaseReturnOrder.AuditStatus == Consts.StateNormal).Sum(d => d.Money) ?? 0)),
                        PublishDateStr = SqlFunctions.DateName("yyyy", p.PublishDate)
                                         + "年" +
                                         SqlFunctions.StringConvert((decimal)SqlFunctions.DatePart("mm", p.PublishDate)).Trim() + "月",
                        //PublishDate = p.PublishDate

                    }).GroupBy(d => d.PublishDateStr).Select(d => new BusinessPerformance
                    {
                        Month = d.Key,
                        TotalSellMoney = d.Sum(o => o.SellMoney),
                        TotalVerificationMoney = d.Sum(o => o.VerificationMoney),
                        TotalConfirmVerificationMoney = d.Sum(o => o.ConfirmVerificationMoney),
                        TotalPurchaseMoney = d.Sum(o => o.PurchaseMoney),
                        TotalProfitMoney = d.Sum(o => o.ProfitMoney)

                    });
        }

        /// <summary>
        /// 销售业绩统计(按媒体类型分组)
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        public IQueryable<BusinessPerformance> BusinessPerformanceGroupByMediaType(BusinessOrderDetailView viewModel)
        {
            //var purchaseOrders = _purchaseOrderDetailRepository.LoadEntities(d => d.IsDelete == false && d.PurchaseOrder.IsDelete == false&&d.TransactorId!= "X1712181402100028");
            var purchaseOrders = _purchaseOrderDetailRepository.LoadEntities(d => d.IsDelete == false && d.PurchaseOrder.IsDelete == false);
            var businessOrders = _repository.LoadEntities(d => d.IsDelete == false && d.BusinessOrder.IsDelete == false);
            //过滤日期
            if (viewModel.PublishDateStart != null && viewModel.PublishDateEnd != null)
            {
                var endDay = viewModel.PublishDateEnd.Value.AddDays(1);
                purchaseOrders = purchaseOrders.Where(d =>
                    d.PublishDate >= viewModel.PublishDateStart && d.PublishDate < endDay);
            }
            return (from b in businessOrders
                    from p in purchaseOrders
                        //双方都是已完成的状态
                    where p.Status == Consts.PurchaseStatusSuccess &&
                          b.Status == Consts.StateOK &&
                          b.Id == p.BusinessOrderDetailId
                    select new BusinessOrderDetailView
                    {
                        SellMoney = b.SellMoney,
                        VerificationMoney = b.VerificationMoney,
                        ConfirmVerificationMoney = b.ConfirmVerificationMoney,
                        PurchaseMoney = p.PurchaseMoney - (p.PurchaseReturenOrderDetails.Where(a => a.PurchaseReturnOrder.AuditStatus == Consts.StateNormal).Sum(d => d.Money) ?? 0),
                        ProfitMoney = b.SellMoney - (p.PurchaseMoney - (p.PurchaseReturenOrderDetails.Where(a => a.PurchaseReturnOrder.AuditStatus == Consts.StateNormal).Sum(d => d.Money) ?? 0)),
                        MediaTypeName = b.MediaTypeName
                    }).GroupBy(d => d.MediaTypeName).Select(d => new BusinessPerformance
                    {
                        MediaTypeName = d.Key,
                        TotalSellMoney = d.Sum(o => o.SellMoney),
                        TotalVerificationMoney = d.Sum(o => o.VerificationMoney),
                        TotalConfirmVerificationMoney = d.Sum(o => o.ConfirmVerificationMoney),
                        TotalPurchaseMoney = d.Sum(o => o.PurchaseMoney),
                        TotalProfitMoney = d.Sum(o => o.ProfitMoney),
                        Profit = d.Sum(o => o.SellMoney) == 0 ? 0 : d.Sum(o => o.ProfitMoney) / d.Sum(o => o.SellMoney) * 100,
                        OrderCount = d.Count()
                    });
        }
        /// <summary>
        /// 销售业绩统计(按媒体分组)
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        public IQueryable<BusinessPerformance> BusinessPerformanceGroupByMedia(BusinessOrderDetailView viewModel)
        {
            //var purchaseOrders = _purchaseOrderDetailRepository.LoadEntities(d => d.IsDelete == false && d.PurchaseOrder.IsDelete == false&&d.TransactorId!= "X1712181402100028");
            var purchaseOrders = _purchaseOrderDetailRepository.LoadEntities(d => d.IsDelete == false && d.PurchaseOrder.IsDelete == false);
            var businessOrders = _repository.LoadEntities(d => d.IsDelete == false && d.BusinessOrder.IsDelete == false);
            //过滤日期
            if (viewModel.PublishDateStart != null && viewModel.PublishDateEnd != null)
            {
                var endDay = viewModel.PublishDateEnd.Value.AddDays(1);
                purchaseOrders = purchaseOrders.Where(d =>
                    d.PublishDate >= viewModel.PublishDateStart && d.PublishDate < endDay);
            }

            if (!string.IsNullOrWhiteSpace(viewModel.MediaTypeId))
            {
                purchaseOrders =
                    purchaseOrders.Where(d => d.MediaPrice.Media.MediaType.CallIndex == viewModel.MediaTypeId);
            }
            return (from b in businessOrders
                    from p in purchaseOrders
                        //双方都是已完成的状态
                    where p.Status == Consts.PurchaseStatusSuccess &&
                          b.Status == Consts.StateOK &&
                          b.Id == p.BusinessOrderDetailId
                    select new BusinessOrderDetailView
                    {
                        SellMoney = b.SellMoney,
                        VerificationMoney = b.VerificationMoney,
                        ConfirmVerificationMoney = b.ConfirmVerificationMoney,
                        PurchaseMoney = p.PurchaseMoney - (p.PurchaseReturenOrderDetails.Where(a => a.PurchaseReturnOrder.AuditStatus == Consts.StateNormal).Sum(d => d.Money) ?? 0),
                        ProfitMoney = b.SellMoney - (p.PurchaseMoney - (p.PurchaseReturenOrderDetails.Where(a => a.PurchaseReturnOrder.AuditStatus == Consts.StateNormal).Sum(d => d.Money) ?? 0)),
                        MediaId = b.MediaPrice.Media.Id,
                        MediaName = b.MediaPrice.Media.MediaName
                    }).GroupBy(d => new { d.MediaId, d.MediaName }).Select(d => new BusinessPerformance
                    {
                        MediaName = d.Key.MediaName,
                        TotalSellMoney = d.Sum(o => o.SellMoney),
                        TotalVerificationMoney = d.Sum(o => o.VerificationMoney),
                        TotalConfirmVerificationMoney = d.Sum(o => o.ConfirmVerificationMoney),
                        TotalPurchaseMoney = d.Sum(o => o.PurchaseMoney),
                        TotalProfitMoney = d.Sum(o => o.ProfitMoney),
                        Profit = d.Sum(o => o.SellMoney) == 0 ? 0 : d.Sum(o => o.ProfitMoney) / d.Sum(o => o.SellMoney) * 100,
                        OrderCount = d.Count()
                    });
        }
        public void Update(BusinessOrderDetail entity)
        {
            _repository.Update(entity);
            _dbContext.SaveChanges();
        }
        public void Update(List<BusinessOrderDetail> entitys)
        {
            foreach (var item in entitys)
            {
                _repository.Update(item);
            }
            _dbContext.SaveChanges();
        }
        public void Update(Expression<Func<BusinessOrderDetail, bool>> whereLambda, Expression<Func<BusinessOrderDetail, BusinessOrderDetail>> updateLambda)
        {
            _repository.Update(whereLambda, updateLambda);
            _dbContext.SaveChanges();
        }
        public void Delete(BusinessOrderDetail entity)
        {
            _repository.Remove(entity);
            _dbContext.SaveChanges();
        }

    }
}
