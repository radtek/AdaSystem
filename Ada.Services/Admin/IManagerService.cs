using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core;
using Ada.Core.Domain.Admin;
using Ada.Core.ViewModel.Admin;

namespace Ada.Services.Admin
{
    public interface IManagerService: IDependency
    {
        IQueryable<Manager> LoadEntitiesFilter(ManagerView managerView);
        void Add(Manager entity, List<string> roleIds = null, List<string> organizationIds = null);
        void Update(Manager entity, List<string> roleIds = null, List<string> organizationIds = null);
        void Delete(Manager entity);
        bool SetAction(List<string> managerIds, List<string> actionIds);
    }
}
