﻿using System.Linq;
using Ada.Core;
using Ada.Core.Domain.Customer;
using Ada.Core.ViewModel.Customer;

namespace Ada.Services.Customer
{
   public class CommpanyService : ICommpanyService
    {
        private readonly IDbContext _dbContext;
        private readonly IRepository<Commpany> _repository;
        public CommpanyService(IDbContext dbContext,
            IRepository<Commpany> repository)
        {
            _dbContext = dbContext;
            _repository = repository;
        }
        public IQueryable<Commpany> LoadEntitiesFilter(CommpanyView viewModel)
        {
            var allList = _repository.LoadEntities(d => d.IsDelete == false);
            //条件过滤
            if (!string.IsNullOrWhiteSpace(viewModel.search))
            {
                allList = allList.Where(d => d.Name.Contains(viewModel.search));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.CommpanyType))
            {
                allList = allList.Where(d => d.CommpanyType.Contains(viewModel.CommpanyType));
            }
            allList = allList.Where(d => d.IsBusiness==viewModel.IsBusiness);
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
        public void Add(Commpany entity)
        {
            _repository.Add(entity);
            _dbContext.SaveChanges();
        }

        public void Update(Commpany entity)
        {
            _repository.Update(entity);
            _dbContext.SaveChanges();
        }

        public void Delete(Commpany entity)
        {
            _repository.Delete(entity);
            _dbContext.SaveChanges();
        }
    }
}