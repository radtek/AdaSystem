using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core;
using Ada.Core.Domain.Business;
using Ada.Core.ViewModel.Business;

namespace Ada.Services.Business
{
    public class BusinessWriteOffService : IBusinessWriteOffService
    {
        private readonly IDbContext _dbContext;
        private readonly IRepository<BusinessWriteOff> _repository;
        public BusinessWriteOffService(IDbContext dbContext,
            IRepository<BusinessWriteOff> repository)
        {
            _dbContext = dbContext;
            _repository = repository;
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
        //public IQueryable<BusinessOrderDetail> LoadEntitiesFilters(BusinessWriteOffView viewModel)
        //{
        //    var allList = _repository.LoadEntities(d => d.IsDelete == false);
        //    //条件过滤
        //    if (viewModel.Managers != null && viewModel.Managers.Count > 0)
        //    {
        //        allList = allList.Where(d => viewModel.Managers.Contains(d.TransactorId));
        //    }

        //    var temp = allList.Select(d => d.BusinessOrderDetails.FirstOrDefault());
            
        //    viewModel.total = allList.Count();
        //    int offset = viewModel.offset ?? 0;
        //    int rows = viewModel.limit ?? 10;
        //    string order = string.IsNullOrWhiteSpace(viewModel.order) ? "desc" : viewModel.order;
        //    if (order == "desc")
        //    {
        //        return allList.OrderByDescending(d => d.Id).Skip(offset).Take(rows);
        //    }
        //    return allList.OrderBy(d => d.Id).Skip(offset).Take(rows);
        //}
        public void Update(BusinessWriteOff entity)
        {

            _repository.Update(entity);
            _dbContext.SaveChanges();
        }
    }
}
