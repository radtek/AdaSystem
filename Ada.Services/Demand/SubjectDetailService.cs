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
    public class SubjectDetailService : ISubjectDetailService
    {
        private readonly IDbContext _dbContext;
        private readonly IRepository<SubjectDetail> _repository;
        public SubjectDetailService(IDbContext dbContext,
            IRepository<SubjectDetail> repository)
        {
            _dbContext = dbContext;
            _repository = repository;
        }
        public void Delete(SubjectDetail entity)
        {
            _repository.Remove(entity);
            _dbContext.SaveChanges();
        }

        public SubjectDetail GetById(string id)
        {
            return _repository.LoadEntities(d => d.Id == id).FirstOrDefault();
        }

        public IQueryable<SubjectDetail> LoadEntitiesFilter(SubjectDetailView viewModel)
        {
            var allList = _repository.LoadEntities(d => d.IsDelete == false);
            //条件过滤
            if (viewModel.Managers != null && viewModel.Managers.Count > 0)
            {
                if (viewModel.IsDo==true)
                {
                    allList = allList.Where(d => viewModel.Managers.Contains(d.TransactorId));
                }
                if (viewModel.IsProducer == true)
                {
                    allList = allList.Where(d => viewModel.Managers.Contains(d.ProducerById));
                }
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

        public void Update(SubjectDetail entity)
        {
            _repository.Update(entity);
            _dbContext.SaveChanges();
        }
    }
}
