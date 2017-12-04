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
    public class BusinessPayeeService : IBusinessPayeeService
    {
        private readonly IDbContext _dbContext;
        private readonly IRepository<BusinessPayee> _repository;
        public BusinessPayeeService(IDbContext dbContext,
            IRepository<BusinessPayee> repository)
        {
            _dbContext = dbContext;
            _repository = repository;
        }
        public void Add(BusinessPayee entity)
        {
            _repository.Add(entity);
            _dbContext.SaveChanges();
        }

        public void Delete(BusinessPayee entity)
        {
            _repository.Remove(entity);
            _dbContext.SaveChanges();
        }

        public IQueryable<BusinessPayee> LoadEntitiesFilter(BusinessPayeeView viewModel)
        {
            var allList = _repository.LoadEntities(d => d.IsDelete == false);
            //条件过滤
            if (!string.IsNullOrWhiteSpace(viewModel.search))
            {
                allList = allList.Where(d => d.LinkManName.Contains(viewModel.search));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.LinkManId))
            {
                allList = allList.Where(d => d.LinkManId==viewModel.LinkManId);
            }
            if (viewModel.VerificationStatus != null)
            {
                allList = allList.Where(d => d.VerificationStatus == viewModel.VerificationStatus);
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

        public void Update(BusinessPayee entity)
        {

            _repository.Update(entity);
            _dbContext.SaveChanges();
        }
    }
}
