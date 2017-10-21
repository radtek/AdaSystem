using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core;

namespace Ada.Services.Admin
{
    public interface IPermissionService : IDependency
    {
        bool Authorize(Core.Domain.Admin.Action action, string managerId);
    }

}
