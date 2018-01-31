using System;
using System.Collections.Generic;
using System.Linq;
using Ada.Core;
using Ada.Core.Domain.API;
using Ada.Core.Domain.Resource;
using Ada.Core.ViewModel.API;
using Ada.Core.ViewModel.Resource;

namespace Ada.Services.API
{
    public class APIInterfacesService : IAPIInterfacesService
    {
        private readonly IDbContext _dbContext;
        private readonly IRepository<APIInterfaces> _repository;
        public APIInterfacesService(IDbContext dbContext,
            IRepository<APIInterfaces> repository)
        {
            _dbContext = dbContext;
            _repository = repository;
        }
        public IQueryable<APIInterfaces> LoadEntitiesFilter(APIInterfacesView viewModel)
        {
            var allList = _repository.LoadEntities(d => d.IsDelete == false);
            //条件过滤
            if (!string.IsNullOrWhiteSpace(viewModel.search))
            {
                allList = allList.Where(d => d.APIName.Contains(viewModel.search));
            }
            viewModel.total = allList.Count();
            int offset = viewModel.offset ?? 0;
            int rows = viewModel.limit ?? 10;
            string order = string.IsNullOrWhiteSpace(viewModel.order) ? "desc" : viewModel.order;
            if (order == "desc")
            {
                return allList.OrderByDescending(d => d.Taxis).Skip(offset).Take(rows);
            }
            return allList.OrderBy(d => d.Taxis).Skip(offset).Take(rows);
        }
        public void Add(APIInterfaces entity)
        {
            _repository.Add(entity);
            _dbContext.SaveChanges();
        }

        public void Update(APIInterfaces entity)
        {
            _repository.Update(entity);
            _dbContext.SaveChanges();
        }

        public void Delete(APIInterfaces entity)
        {
            _repository.Remove(entity);
            _dbContext.SaveChanges();
        }

        public APIInterfaces GetAPIInterfacesByCallIndex(string callIndex)
        {
            return _repository.LoadEntities(d => d.CallIndex.Equals(callIndex, StringComparison.CurrentCultureIgnoreCase))
                .FirstOrDefault();
           
        }
    }
}
