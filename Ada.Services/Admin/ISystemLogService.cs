using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core;
using Ada.Core.Domain.Log;
using Ada.Core.ViewModel.Admin;

namespace Ada.Services.Admin
{
    public interface ISystemLogService : IDependency
    {
        IQueryable<SystemLog> LoadEntitiesFilter(SystemLogView viewModel);
        void Delete(params string[] ids);
    }

}
