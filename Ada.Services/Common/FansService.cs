using System.Linq;
using Ada.Core;
using Ada.Core.Domain.Common;
using Ada.Core.ViewModel.Common;

namespace Ada.Services.Common
{
   public class FansService : IFansService
    {
        private readonly IDbContext _dbContext;
        private readonly IRepository<Fans> _repository;
        public FansService(IDbContext dbContext,
            IRepository<Fans> repository)
        {
            _dbContext = dbContext;
            _repository = repository;
        }
        
        public void Add(Fans entity)
        {
            _repository.Add(entity);
            _dbContext.SaveChanges();
        }

        public void Update(Fans entity)
        {
            _repository.Update(entity);
            _dbContext.SaveChanges();
        }

        public void Delete(Fans entity)
        {
            _repository.Remove(entity);
            _dbContext.SaveChanges();
        }
        public IQueryable<Fans> LoadEntitiesFilter(FansView viewModel)
        {
            var allList = _repository.LoadEntities(d => d.IsDelete == false);
            //条件过滤
            if (!string.IsNullOrWhiteSpace(viewModel.search))
            {
                allList = allList.Where(d => d.NickName.Contains(viewModel.search));
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
