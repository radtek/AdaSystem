using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core;
using Ada.Core.Domain.API;
using Ada.Core.Domain.Log;
using Ada.Core.ViewModel.Admin;
using Ada.Core.ViewModel.API;

namespace Ada.Services.API
{
    public interface IAPIRequestRecordService : IDependency
    {
        IQueryable<APIRequestRecord> LoadEntitiesFilter(APIRequestRecordView viewModel);
        void Delete(params string[] ids);
    }

}
