using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core;
using Ada.Core.Domain.Demand;
using Ada.Core.ViewModel.Demand;

namespace Ada.Services.Demand
{
    public class SubjectService : ISubjectService
    {
        private readonly IDbContext _dbContext;
        private readonly IRepository<Subject> _repository;
        public SubjectService(IDbContext dbContext,
            IRepository<Subject> repository)
        {
            _dbContext = dbContext;
            _repository = repository;
        }
        public void Add(Subject entity)
        {
            _repository.Add(entity);
            _dbContext.SaveChanges();
        }

        public void Delete(Subject entity)
        {
            _repository.Remove(entity);
            _dbContext.SaveChanges();
        }

        public IQueryable<Subject> LoadEntitiesFilter(SubjectView viewModel)
        {
            var allList = _repository.LoadEntities(d => d.IsDelete == false);
            //条件过滤
            if (viewModel.Managers != null && viewModel.Managers.Count > 0)
            {
                allList = allList.Where(d => viewModel.Managers.Contains(d.AddedById));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.search))
            {
                allList = allList.Where(d => d.Title.Contains(viewModel.search));
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

        public void Update(Subject entity)
        {
            _repository.Update(entity);
            _dbContext.SaveChanges();
        }
        public Subject GetById(string id)
        {
            return _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
        }
    }
}
