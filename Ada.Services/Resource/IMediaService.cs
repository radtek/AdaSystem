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
   public interface IMediaService: IDependency
    {
        IQueryable<Media> LoadEntitiesFilter(MediaView viewModel);
        IQueryable<MediaCommentView> LoadComments(string id, int page = 1);
        void Add(Media entity);
        void Update(Media entity);
        void Delete(Media entity);
    }
}
