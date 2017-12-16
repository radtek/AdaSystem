using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core
{
    /// <summary>
    /// 全局启动时，初始化数据的接口
    /// </summary>
    public interface IAppStart
    {
        void Register();
    }
}
