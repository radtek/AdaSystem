﻿using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core;
using Ada.Core.Domain.Business;
using Ada.Core.Domain.Purchase;
using Ada.Core.ViewModel.Business;
using Microsoft.SqlServer.Server;

namespace Ada.Services.Business
{
    public class BusinessWriteOffService : IBusinessWriteOffService
    {
        private readonly IDbContext _dbContext;
        private readonly IRepository<BusinessWriteOff> _repository;
        private readonly IRepository<PurchaseOrderDetail> _purchaseOrderDetailRepository;
        public BusinessWriteOffService(IDbContext dbContext,
            IRepository<BusinessWriteOff> repository, IRepository<PurchaseOrderDetail> purchaseOrderDetailRepository)
        {
            _dbContext = dbContext;
            _repository = repository;
            _purchaseOrderDetailRepository = purchaseOrderDetailRepository;
        }
        public void Add(BusinessWriteOff entity)
        {
            _repository.Add(entity);
            _dbContext.SaveChanges();
        }

        public void Delete(BusinessWriteOff entity)
        {
            _repository.Remove(entity);
            _dbContext.SaveChanges();
        }

        public IQueryable<BusinessWriteOff> LoadEntitiesFilter(BusinessWriteOffView viewModel)
        {
            var allList = _repository.LoadEntities(d => d.IsDelete == false);
            //条件过滤
            if (viewModel.Managers != null && viewModel.Managers.Count > 0)
            {
                allList = allList.Where(d => viewModel.Managers.Contains(d.TransactorId));
            }
            //if (!string.IsNullOrWhiteSpace(viewModel.search))
            //{
            //    allList = allList.Where(d => d.BusinessOrders.FirstOrDefault().LinkManName.Contains(viewModel.search));
            //}
            //var total = allList.Sum(d => d.BusinessOrderDetails.Sum(o => o.Money));//销售总额
            //var total2 = allList.Sum(d => d.BusinessPayees.Sum(p => p.Money));//领款总额
            //var total3 = allList.Sum(d => d.BusinessPayees.Sum(p => p.BusinessPayments.Sum(r => r.PayMoney)));//请款总额

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
        public IQueryable<BusinessWriteOffDetailView> LoadEntitiesFilter(BusinessWriteOffDetailView viewModel)
        {
            var allList = _repository.LoadEntities(d => d.IsDelete == false);
            //条件过滤
            if (viewModel.Managers != null && viewModel.Managers.Count > 0)
            {
                allList = allList.Where(d => viewModel.Managers.Contains(d.TransactorId));
            }

            var purchases = _purchaseOrderDetailRepository.LoadEntities(d => d.IsDelete == false);
            var temp = from h in allList
                       from o in h.BusinessOrderDetails
                       from p in purchases
                           //from b in h.BusinessPayees
                       where o.Id == p.BusinessOrderDetailId
                       select new BusinessWriteOffDetailView
                       {
                           Id = h.Id,
                           WriteOffDate = h.WriteOffDate,
                           PublishDate = p.PublishDate,
                           OrderNum=o.BusinessOrder.OrderNum,
                           Transactor = o.BusinessOrder.Transactor,
                           LinkManName = o.BusinessOrder.LinkManName,
                           OrderId = o.BusinessOrderId,
                           BusinessMoney = o.SellMoney,
                           PurchaseMoney = p.PurchaseMoney,
                           Profit = Math.Round((decimal) (o.SellMoney - p.PurchaseMoney),2),
                           ReturnDays = SqlFunctions.DateDiff("day", p.PublishDate,h.WriteOffDate),
                           Commission =Math.Round((decimal) ((o.SellMoney - p.PurchaseMoney) * 0.15M),2) 
                       };

            viewModel.TotalBusinessMoney = temp.Sum(d => d.BusinessMoney);
            viewModel.TotalCommission = temp.Sum(d => d.Commission);
            viewModel.TotalProfit = temp.Sum(d => d.Profit);
            viewModel.TotalPurchaseMoney = temp.Sum(d => d.PurchaseMoney);
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
        public void Update(BusinessWriteOff entity)
        {

            _repository.Update(entity);
            _dbContext.SaveChanges();
        }
    }
}
