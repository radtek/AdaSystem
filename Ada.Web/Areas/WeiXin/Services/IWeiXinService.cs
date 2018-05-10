using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core;
using Ada.Core.Domain.WeiXin;

namespace WeiXin.Services
{
    public interface IWeiXinService : IDependency
    {
        WeiXinAccount GetWeiXinAccount(string appIdOrAccountId = null);
    }
}
