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
   public interface IMediaCommentService: IDependency
    {
        IQueryable<MediaComment> LoadEntitiesFilter(MediaCommentView viewModel);
       
        void Delete(string[] ids);
    }
}
