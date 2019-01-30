using System.Linq;
using Ada.Core;
using Ada.Core.Domain.Vote;
using Ada.Core.ViewModel.Vote;

namespace Ada.Services.Vote
{
    public class VoteThemeService : IVoteThemeService
    {
        private readonly IDbContext _dbContext;
        private readonly IRepository<VoteTheme> _repository;
        public VoteThemeService(IDbContext dbContext,
            IRepository<VoteTheme> repository)
        {
            _dbContext = dbContext;
            _repository = repository;
        }

        public void Add(VoteTheme entity)
        {
            _repository.Add(entity);
            _dbContext.SaveChanges();
        }

        public void Update(VoteTheme entity)
        {
            _repository.Update(entity);
            _dbContext.SaveChanges();
        }

        public void Delete(VoteTheme entity)
        {
            _repository.Remove(entity);
            _dbContext.SaveChanges();
        }
        public VoteTheme GetById(string id)
        {
            return _repository.LoadEntities(d => d.Id == id).FirstOrDefault();

        }
        public IQueryable<VoteTheme> LoadEntitiesFilter(VoteThemeView viewModel)
        {
            var allList = _repository.LoadEntities(d => d.IsDelete == false);

            if (!string.IsNullOrWhiteSpace(viewModel.search))
            {
                allList = allList.Where(d => d.Title.Contains(viewModel.search));
            }

            if (viewModel.Status)
            {
                allList = allList.Where(d => d.Status);
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
