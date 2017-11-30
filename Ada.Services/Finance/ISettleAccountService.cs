using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core;
using Ada.Core.Domain.Finance;

namespace Ada.Services.Finance
{
   public interface ISettleAccountService : IDependency
    {
        void Add(SettleAccount entity);
        void Update(SettleAccount entity);
        void Delete(SettleAccount entity);
        IQueryable<SettleAccount> LoadEntitiesFilter();
    }
}
