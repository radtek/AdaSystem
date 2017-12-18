using System.Linq;
using Ada.Core;
using Ada.Core.Domain.Customer;
using Ada.Core.Domain.Log;
using Ada.Core.ViewModel.Admin;
using Ada.Core.ViewModel.Customer;
using Ada.Core.ViewModel.WorkLog;

namespace Ada.Services.Admin
{
   public class ManagerLoginLogService : IManagerLoginLogService
    {
        private readonly IDbContext _dbContext;
        private readonly IRepository<ManagerLoginLog> _repository;
        public ManagerLoginLogService(IDbContext dbContext,
            IRepository<ManagerLoginLog> repository)
        {
            _dbContext = dbContext;
            _repository = repository;
        }
        
       
        public void Delete(ManagerLoginLog entity)
        {
            _repository.Remove(entity);
            _dbContext.SaveChanges();
        }
        public IQueryable<ManagerLoginLog> LoadEntitiesFilter(ManagerLoginLogView viewModel)
        {
            var allList = _repository.LoadEntities(d => d.IsDelete == false);
            //条件过滤
            
            if (!string.IsNullOrWhiteSpace(viewModel.search))
            {
                allList = allList.Where(d => d.Manager.UserName.Contains(viewModel.search));
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
