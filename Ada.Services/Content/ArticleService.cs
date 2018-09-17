using System.Linq;
using Ada.Core;
using Ada.Core.Domain.Content;
using Ada.Core.ViewModel.Content;

namespace Ada.Services.Content
{
   public class ArticleService : IArticleService
    {
        private readonly IDbContext _dbContext;
        private readonly IRepository<Article> _repository;
        public ArticleService(IDbContext dbContext,
            IRepository<Article> repository)
        {
            _dbContext = dbContext;
            _repository = repository;
        }
        
        public void Add(Article entity)
        {
            _repository.Add(entity);
            _dbContext.SaveChanges();
        }

        public void Update(Article entity)
        {
            _repository.Update(entity);
            _dbContext.SaveChanges();
        }

        public void Delete(Article entity)
        {
            _repository.Remove(entity);
            _dbContext.SaveChanges();
        }
        public IQueryable<Article> LoadEntitiesFilter(ArticleView viewModel)
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
                return allList.OrderByDescending(d => d.Id).Skip(offset).Take(rows);
            }
            return allList.OrderBy(d => d.Id).Skip(offset).Take(rows);
        }
    }
}
