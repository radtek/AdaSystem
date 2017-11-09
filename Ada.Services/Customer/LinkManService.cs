using System.Linq;
using Ada.Core;
using Ada.Core.Domain.Customer;
using Ada.Core.ViewModel.Customer;

namespace Ada.Services.Customer
{
   public class LinkManService : ILinkManService
    {
        private readonly IDbContext _dbContext;
        private readonly IRepository<LinkMan> _repository;
        public LinkManService(IDbContext dbContext,
            IRepository<LinkMan> repository)
        {
            _dbContext = dbContext;
            _repository = repository;
        }
        public IQueryable<LinkMan> LoadEntitiesFilter(LinkManView viewModel)
        {
            var allList = _repository.LoadEntities(d => d.IsDelete == false);
            //条件过滤
            if (!string.IsNullOrWhiteSpace(viewModel.search))
            {
                allList = allList.Where(d => d.Name.Contains(viewModel.search));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.CommpanyName))
            {
                allList = allList.Where(d => d.Commpany.Name.Contains(viewModel.CommpanyName));
            }
            allList = allList.Where(d => d.Commpany.IsBusiness==viewModel.IsBusiness);
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
        public void Add(LinkMan entity)
        {
            _repository.Add(entity);
            _dbContext.SaveChanges();
        }

        public void Update(LinkMan entity)
        {
            _repository.Update(entity);
            _dbContext.SaveChanges();
        }

        public void Delete(LinkMan entity)
        {
            _repository.Delete(entity);
            _dbContext.SaveChanges();
        }
    }
}
