using System.Linq;
using Ada.Core;
using Ada.Core.Domain.Content;
using Ada.Core.ViewModel.Content;

namespace Ada.Services.Content
{
   public interface IArticleService : IDependency
   {
       void Add(Article entity);
       void Update(Article entity);
       void Delete(Article entity);
       IQueryable<Article> LoadEntitiesFilter(ArticleView viewModel);
   }
}
