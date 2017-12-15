using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core;
using Ada.Core.ViewModel.Admin;

namespace Ada.Services.Admin
{
    public interface IPermissionService : IDependency
    {
        bool Authorize(Core.Domain.Admin.Action action, string managerId, string roleId="");
        List<MenuView> AuthorizeMenu(string managerId, string roleId="");
        List<string> AuthorizeData(string managerId, string roleId);
    }

}
