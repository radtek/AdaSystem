using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core;
using Ada.Core.Domain.Admin;

namespace Ada.Services.Admin
{
    public interface IOrganizationService : IDependency
    {
        void Add(Organization entity);
        void Update(Organization entity);
        void Delete(Organization entity);
    }
}
