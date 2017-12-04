﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core;
using Ada.Core.Domain.Business;
using Ada.Core.ViewModel.Business;

namespace Ada.Services.Business
{
    public class BusinessOrderService : IBusinessOrderService
    {
        private readonly IDbContext _dbContext;
        private readonly IRepository<BusinessOrder> _repository;
        public BusinessOrderService(IDbContext dbContext,
            IRepository<BusinessOrder> repository)
        {
            _dbContext = dbContext;
            _repository = repository;
        }


        public IQueryable<BusinessOrder> LoadEntitiesFilter(BusinessOrderView viewModel)
        {
            var allList = _repository.LoadEntities(d => d.IsDelete == false);
            //条件过滤
            if (!string.IsNullOrWhiteSpace(viewModel.search))
            {
                allList = allList.Where(d => d.LinkManName.Contains(viewModel.search));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.Transactor))
            {
                allList = allList.Where(d => d.Transactor.Contains(viewModel.Transactor));
            }
            if (viewModel.Status!=null)
            {
                allList = allList.Where(d => d.Status==viewModel.Status);
            }
            if (viewModel.AuditStatus != null)
            {
                allList = allList.Where(d => d.AuditStatus == viewModel.AuditStatus);
            }
            if (!string.IsNullOrWhiteSpace(viewModel.OrderNum))
            {
                allList = allList.Where(d => d.OrderNum == viewModel.OrderNum);
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
    }
}