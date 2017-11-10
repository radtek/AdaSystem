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
   public interface IMediaTagService: IDependency
    {
        IQueryable<MediaTag> LoadEntitiesFilter(MediaTagView viewModel);
        void Add(MediaTag entity);
        void Update(MediaTag entity);
        void Delete(MediaTag entity);
        List<MediaTagView> GetTags();
    }
}
