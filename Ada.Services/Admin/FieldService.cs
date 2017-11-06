using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core;
using Ada.Core.Domain.Admin;
using Ada.Core.ViewModel.Admin;
using Action = Ada.Core.Domain.Admin.Action;

namespace Ada.Services.Admin
{
   public class FieldService : IFieldService
    {
        private readonly IDbContext _dbContext;
        private readonly IRepository<Field> _repository;
        public FieldService(IDbContext dbContext,
            IRepository<Field> repository)
        {
            _dbContext = dbContext;
            _repository = repository;
        }
        public IQueryable<Field> LoadEntitiesFilter(FieldView viewModel)
        {
            var allList = _repository.LoadEntities(d => d.IsDelete == false);
            //条件过滤
            if (!string.IsNullOrWhiteSpace(viewModel.search))
            {
                allList = allList.Where(d => d.Text.Contains(viewModel.search));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.FieldTypeId))
            {
                allList = allList.Where(d => d.FieldTypeId==viewModel.FieldTypeId);
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
        public void Add(Field entity)
        {
            _repository.Add(entity);
            _dbContext.SaveChanges();
        }

        public void Update(Field entity)
        {
            _repository.Update(entity);
            _dbContext.SaveChanges();
        }

        public void Delete(Field entity)
        {
            _repository.Delete(entity);
            _dbContext.SaveChanges();
        }
    }
}
