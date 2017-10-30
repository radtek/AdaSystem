using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core;
using Ada.Core.Domain.Admin;

namespace Ada.Services.Admin
{
   public interface IRoleService:IDependency
   {
       void Add(Role role, string actionIds);
       void Update(Role role, string actionIds);
       void Delete(Role role);
    }
}
