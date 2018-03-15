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
   public interface IMediaDevelopService : IDependency
    {
        IQueryable<MediaDevelop> LoadEntitiesFilter(MediaDevelopView viewModel);
        void Add(MediaDevelop entity);
        void AddRange(IList<MediaDevelop> entities);
        void Update(MediaDevelop entity);
        void Delete(MediaDevelop entity);
    }
}
