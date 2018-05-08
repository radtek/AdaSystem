using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core;
using Ada.Core.Domain.WeiXin;
using Ada.Core.ViewModel.WeiXin;

namespace Ada.Services.WeiXin
{
   public interface IWeiXinAccountService : IDependency
    {
        void Add(WeiXinAccount entity);
        void Update(WeiXinAccount entity);
        void Delete(WeiXinAccount entity);
        IQueryable<WeiXinAccount> LoadEntitiesFilter(WeiXinAccountView viewModel);
    }
}
