using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core.Domain.Admin;
using Ada.Core.ViewModel.Admin;

namespace Ada.Services.Admin
{
    public interface IManagerService
    {
        IQueryable<Manager> LoadEntitiesFilter(ManagerView managerView);
        bool SetRole(List<string> managerIds, List<string> roleIds);
        bool SetOrganization(List<string> managerIds, List<string> organizationIds);
        bool SetAction(List<string> managerIds, List<string> actionIds);
    }
}
