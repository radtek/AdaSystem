using System.Linq;
using Ada.Core;
using Ada.Core.Domain.Customer;
using Ada.Core.ViewModel.Customer;
using Ada.Core.ViewModel.WorkLog;

namespace Ada.Services.WorkLog
{
   public class WorkLogService : IWorkLogService
    {
        private readonly IDbContext _dbContext;
        private readonly IRepository<Core.Domain.Log.WorkLog> _repository;
        public WorkLogService(IDbContext dbContext,
            IRepository<Core.Domain.Log.WorkLog> repository)
        {
            _dbContext = dbContext;
            _repository = repository;
        }
        
        public void Add(Core.Domain.Log.WorkLog entity)
        {
            _repository.Add(entity);
            _dbContext.SaveChanges();
        }

        public void Update(Core.Domain.Log.WorkLog entity)
        {
            _repository.Update(entity);
            _dbContext.SaveChanges();
        }

        public void Delete(Core.Domain.Log.WorkLog entity)
        {
            _repository.Remove(entity);
            _dbContext.SaveChanges();
        }
        public IQueryable<Core.Domain.Log.WorkLog> LoadEntitiesFilter(WorkLogView viewModel)
        {
            var allList = _repository.LoadEntities(d => d.IsDelete == false);
            //条件过滤
            if (viewModel.Managers != null && viewModel.Managers.Count > 0)
            {
                allList = allList.Where(d => viewModel.Managers.Contains(d.TransactorId));
            }
            if (viewModel.Managers != null && viewModel.Managers.Count > 0)
            {
                allList = allList.Where(d => viewModel.Managers.Contains(d.TransactorId));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.search))
            {
                allList = allList.Where(d => d.Transactor.Contains(viewModel.search));
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
