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
   public interface IMediaPriceService : IDependency
    {
        IQueryable<MediaPrice> LoadEntitiesFilter(MediaView viewModel);

        int Update(Expression<Func<MediaPrice, bool>> whereLambda,
            Expression<Func<MediaPrice, MediaPrice>> updateLambda);
    }
}
