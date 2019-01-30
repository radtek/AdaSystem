using System.Linq;
using Ada.Core;
using Ada.Core.Domain.Vote;
using Ada.Core.ViewModel.Vote;

namespace Ada.Services.Vote
{
   public class VoteItemService : IVoteItemService
    {
        private readonly IDbContext _dbContext;
        private readonly IRepository<VoteItem> _repository;
        public VoteItemService(IDbContext dbContext,
            IRepository<VoteItem> repository)
        {
            _dbContext = dbContext;
            _repository = repository;
        }
        
        public void Add(VoteItem entity)
        {
            _repository.Add(entity);
            _dbContext.SaveChanges();
        }

        public void Update(VoteItem entity)
        {
            _repository.Update(entity);
            _dbContext.SaveChanges();
        }

        public void Delete(VoteItem entity)
        {
            _repository.Remove(entity);
            _dbContext.SaveChanges();
        }
        public VoteItem GetById(string id)
        {
            return _repository.LoadEntities(d => d.Id == id).FirstOrDefault();

        }
        public IQueryable<VoteItem> LoadEntitiesFilter(VoteItemView viewModel)
        {
            var allList = _repository.LoadEntities(d => d.IsDelete == false);
            
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
    }
}
