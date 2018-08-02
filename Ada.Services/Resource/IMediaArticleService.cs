using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core;
using Ada.Core.Domain.Resource;
using Ada.Core.ViewModel.Resource;

namespace Ada.Services.Resource
{
   public interface IMediaArticleService : IDependency
    {
        IQueryable<MediaArticle> LoadEntitiesFilter(MediaArticleView viewModel);
        void Delete(string[] ids);
        void Add(MediaArticle entity);
    }
}
