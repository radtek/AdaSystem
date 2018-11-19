using System.Linq;
using Ada.Core;
using Ada.Core.Domain.Customer;
using Ada.Core.ViewModel.Customer;

namespace Ada.Services.Customer
{
   public class FollowUpService : IFollowUpService
    {
        private readonly IDbContext _dbContext;
        private readonly IRepository<FollowUp> _repository;
        public FollowUpService(IDbContext dbContext,
            IRepository<FollowUp> repository)
        {
            _dbContext = dbContext;
            _repository = repository;
        }
        
        public void Add(FollowUp entity)
        {
            _repository.Add(entity);
            _dbContext.SaveChanges();
        }

        public void Update(FollowUp entity)
        {
            _repository.Update(entity);
            _dbContext.SaveChanges();
        }

        public void Delete(FollowUp entity)
        {
            _repository.Remove(entity);
            _dbContext.SaveChanges();
        }
        public IQueryable<FollowUp> LoadEntitiesFilter(FollowUpView viewModel)
        {
            var allList = _repository.LoadEntities(d => d.IsDelete == false);
            //条件过滤
            if (viewModel.Managers != null && viewModel.Managers.Count > 0)
            {
                allList = allList.Where(d => viewModel.Managers.Contains(d.LinkMan.TransactorId));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.search))
            {
                allList = allList.Where(d => d.LinkMan.LoginName.Contains(viewModel.search)
                                             ||d.LinkMan.Name.Contains(viewModel.search)
                                             ||d.IpAddress.Contains(viewModel.search));
            }
            if (!string.IsNullOrWhiteSpace(viewModel.LinkManId))
            {
                allList = allList.Where(d => d.LinkManId==viewModel.LinkManId);
            }
            if (!string.IsNullOrWhiteSpace(viewModel.CompanyName))
            {
                allList = allList.Where(d => d.LinkMan.Commpany.Name.Contains(viewModel.CompanyName));
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
