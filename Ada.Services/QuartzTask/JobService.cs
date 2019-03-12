using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core;
using Ada.Core.Domain.Finance;
using Ada.Core.Domain.QuartzTask;
using Ada.Core.ViewModel.Finance;
using Ada.Core.ViewModel.QuartzTask;

namespace Ada.Services.QuartzTask
{
    public class JobService : IJobService
    {
        private readonly IDbContext _dbContext;
        private readonly IRepository<Job> _repository;
        public JobService(IDbContext dbContext,
            IRepository<Job> repository)
        {
            _dbContext = dbContext;
            _repository = repository;
        }
        public void Add(Job entity)
        {
            _repository.Add(entity);
            _dbContext.SaveChanges();
        }

        public void Update(Job entity)
        {
            _repository.Update(entity);
            _dbContext.SaveChanges();
        }

        public void Delete(Job entity)
        {
            _repository.Remove(entity);
            _dbContext.SaveChanges();
        }

        public IQueryable<Job> LoadEntitiesFilter(JobView viewModel)
        {
            var allList = _repository.LoadEntities(d => d.IsDelete == false);
            //条件过滤
            if (!string.IsNullOrWhiteSpace(viewModel.search))
            {
                allList = allList.Where(d => d.JobName.Contains(viewModel.search));
            }

            if (viewModel.Type!=null)
            {
                allList = allList.Where(d => d.Type == viewModel.Type);
            }
            viewModel.total = allList.Count();
            int offset = viewModel.offset ?? 0;
            int rows = viewModel.limit ?? 10;
            string order = string.IsNullOrWhiteSpace(viewModel.order) ? "desc" : viewModel.order;
            if (order == "desc")
            {
                return allList.OrderByDescending(d => d.GroupName).Skip(offset).Take(rows);
            }
            return allList.OrderBy(d => d.GroupName).Skip(offset).Take(rows);
        }
        public Job GetByJobKey(string name, string group)
        {
            return _repository.LoadEntities(d => d.JobName == name && d.GroupName == group).FirstOrDefault();
        }
    }
}
