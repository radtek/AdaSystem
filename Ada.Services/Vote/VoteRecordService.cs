using System.Linq;
using Ada.Core;
using Ada.Core.Domain.Vote;
using Ada.Core.ViewModel.Vote;

namespace Ada.Services.Vote
{
   public class VoteRecordService : IVoteRecordService
    {
        private readonly IDbContext _dbContext;
        private readonly IRepository<VoteItemRecord> _repository;
        private readonly IRepository<VoteItem> _voteItemRepository;
        public VoteRecordService(IDbContext dbContext,
            IRepository<VoteItemRecord> repository,
            IRepository<VoteItem> voteItemRepository)
        {
            _dbContext = dbContext;
            _repository = repository;
            _voteItemRepository = voteItemRepository;
        }
        
        

        public void Delete(VoteItemRecord entity)
        {
            var item = _voteItemRepository.LoadEntities(d => d.Id == entity.VoteItemId).FirstOrDefault();
            var score = entity.Score;
            item.TotalCount = item.TotalCount - score;
            _repository.Remove(entity);
            _dbContext.SaveChanges();
        }
        public VoteItemRecord GetById(string id)
        {
            return _repository.LoadEntities(d => d.Id == id).FirstOrDefault();

        }
        public IQueryable<VoteItemRecord> LoadEntitiesFilter(VoteItemRecordView viewModel)
        {
            var allList = _repository.LoadEntities(d => d.IsDelete == false);
            
            if (!string.IsNullOrWhiteSpace(viewModel.search))
            {
                allList = allList.Where(d => d.Name.Contains(viewModel.search)||d.VoteItem.Title.Contains(viewModel.search));
            }

            if (!string.IsNullOrWhiteSpace(viewModel.VoteItemId))
            {
                allList = allList.Where(d => d.VoteItemId == viewModel.VoteItemId);
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
