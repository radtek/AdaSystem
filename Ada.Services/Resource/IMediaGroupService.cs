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
   public interface IMediaGroupService : IDependency
    {
        IQueryable<MediaGroup> LoadEntitiesFilter(MediaGroupView viewModel);
        void Add(MediaGroup entity);
        void Update(MediaGroup entity);
        void Delete(MediaGroup entity);
        void AddMedia(List<string> groupIds, Media media, short groupType);
        void RemoveMedia(string groupId, Media media);
    }
}
