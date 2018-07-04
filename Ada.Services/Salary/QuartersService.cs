using System.Linq;
using Ada.Core;
using Ada.Core.Domain.Wages;
using Ada.Core.ViewModel.Wages;

namespace Ada.Services.Salary
{
    public class QuartersService : IQuartersService
    {
        private readonly IDbContext _dbContext;
        private readonly IRepository<Quarters> _repository;
        public QuartersService(IDbContext dbContext,
            IRepository<Quarters> repository)
        {
            _dbContext = dbContext;
            _repository = repository;
        }
        public IQueryable<Quarters> LoadEntitiesFilter(QuartersView viewModel)
        {
            var allList = _repository.LoadEntities(d => d.IsDelete == false);
            //条件过滤
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
                return allList.OrderByDescending(d => d.Taxis).Skip(offset).Take(rows);
            }
            return allList.OrderBy(d => d.Taxis).Skip(offset).Take(rows);
        }
        public void Add(Quarters entity)
        {
            _repository.Add(entity);
            _dbContext.SaveChanges();
        }

        public void Update(Quarters entity)
        {
            _repository.Update(entity);
            _dbContext.SaveChanges();
        }

        public void Delete(Quarters entity)
        {
            _repository.Delete(entity);
            _dbContext.SaveChanges();
        }

        
    }
}
