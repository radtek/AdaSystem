using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core;
using Ada.Core.Domain.Log;
using Ada.Core.ViewModel.Admin;

namespace Ada.Services.Admin
{
    public class SystemLogService : ISystemLogService
    {
        private readonly IDbContext _dbContext;
        private readonly IRepository<SystemLog> _repository;
        public SystemLogService(IRepository<SystemLog> repository, IDbContext dbContext)
        {
            _repository = repository;
            _dbContext = dbContext;
        }
        public void Delete(params string[] ids)
        {
            var list=new List<SystemLog>();
            foreach (var id in ids)
            {
                var logId = long.Parse(id);
                var log = _repository.LoadEntities(d => d.Id == logId).FirstOrDefault();
                list.Add(log);
            }
            _repository.Remove(list);
            _dbContext.SaveChanges();
        }

        public IQueryable<SystemLog> LoadEntitiesFilter(SystemLogView viewModel)
        {
            var allList = _repository.LoadEntities(d => true);
            if (!string.IsNullOrWhiteSpace(viewModel.Level))
            {
                allList = allList.Where(d => d.Level.Contains(viewModel.Level));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.Logger))
            {
                allList = allList.Where(d => d.Logger.Contains(viewModel.Logger));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.search))
            {
                allList = allList.Where(d => d.Message.Contains(viewModel.search));
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
