﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core;
using Ada.Core.Domain.Customer;
using Ada.Core.ViewModel.Customer;

namespace Ada.Services.Customer
{
    public class PayAccountService : IPayAccountService
    {
        private readonly IDbContext _dbContext;
        private readonly IRepository<PayAccount> _repository;
        public PayAccountService(IDbContext dbContext,
            IRepository<PayAccount> repository)
        {
            _dbContext = dbContext;
            _repository = repository;
        }
        public IQueryable<PayAccount> LoadEntitiesFilter(PayAccountView viewModel)
        {
            var allList = _repository.LoadEntities(d => d.IsDelete == false);
            //条件过滤
            if (!string.IsNullOrWhiteSpace(viewModel.search))
            {
                allList = allList.Where(d => d.LinkMan.Name.Contains(viewModel.search));
            }
            if (viewModel.Status != null)
            {
                allList = allList.Where(d => d.Status == viewModel.Status);
            }
            if (viewModel.IsBusiness == false)
            {
                allList = allList.Where(d => !d.LinkMan.Commpany.IsBusiness);
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
        public void Add(PayAccount entity)
        {
            _repository.Add(entity);
            _dbContext.SaveChanges();
        }

        public void Update(PayAccount entity)
        {
            _repository.Update(entity);
            _dbContext.SaveChanges();
        }

        public void Delete(PayAccount entity)
        {
            _repository.Remove(entity);
            _dbContext.SaveChanges();
        }
    }
}
