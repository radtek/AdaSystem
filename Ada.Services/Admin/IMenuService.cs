using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core;
using Ada.Core.Domain.Admin;

namespace Ada.Services.Admin
{
    public interface IMenuService : IDependency
    {
        void Add(Menu entity);
        void Update(Menu entity);
        void Delete(Menu entity);
    }
}
