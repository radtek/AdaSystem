using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;

namespace Ada.Core.Infrastructure
{
    /// <summary>
    /// 初始化依赖注入接口
    /// </summary>
    public interface IDependencyRegistrar
    {
        void Register(ContainerBuilder builder);

        /// <summary>
        /// 顺序
        /// </summary>
        int Order { get; }
    }
}
