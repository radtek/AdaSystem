using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
        IQueryable<MediaView> LoadEntitiesFilters(MediaView viewModel);
        IQueryable<MediaCommentView> LoadComments(string id,  int pageindex, int pagesize, out int total);
        IQueryable<MediaCommentView> LoadComments(MediaCommentView viewModel);
        IQueryable<Ada.Core.ViewModel.Statistics.MediaUpdate> GetMediaUpdatedInfo(MediaView view);
        void Add(Media entity);
        void Update(Media entity);
        void Update(Expression<Func<Media, bool>> whereLambda, Expression<Func<Media, Media>> updateLambda);
        void Delete(Media entity);
        void ClearMediaReferencePrices(string id);
    }
}
