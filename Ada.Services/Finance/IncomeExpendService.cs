using System;
using System.Linq;
using Ada.Core;
using Ada.Core.Domain.Finance;
using Ada.Core.ViewModel.Finance;

namespace Ada.Services.Finance
{
    public class IncomeExpendService : IIncomeExpendService
    {
        private readonly IDbContext _dbContext;
        private readonly IRepository<IncomeExpend> _repository;
        public IncomeExpendService(IDbContext dbContext,
            IRepository<IncomeExpend> repository)
        {
            _dbContext = dbContext;
            _repository = repository;
        }
        public void Add(IncomeExpend entity)
        {
            _repository.Add(entity);
            _dbContext.SaveChanges();
        }

        public void Update(IncomeExpend entity)
        {
            _repository.Update(entity);
            _dbContext.SaveChanges();
        }

        public void Delete(IncomeExpend entity)
        {
            _repository.Delete(entity);
            _dbContext.SaveChanges();
        }
        public IQueryable<IncomeExpend> LoadEntitiesFilter(IncomeExpendView viewModel)
        {
            var allList = _repository.LoadEntities(d => d.IsDelete == false);
            //条件过滤
            if (viewModel.IsMain != null)
            {
                allList = allList.Where(d => d.IsMain == viewModel.IsMain);
            }
            if (viewModel.SubjectType != null)
            {
                allList = allList.Where(d => d.SubjectType == viewModel.SubjectType);
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
    }
}
