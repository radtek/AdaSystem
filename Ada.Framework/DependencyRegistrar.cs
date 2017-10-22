using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Compilation;
using Ada.Core;
using Ada.Core.Infrastructure;
using Ada.Data;
using Autofac;
using Autofac.Integration.Mvc;

namespace Ada.Framework
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public int Order => 0;

        public void Register(ContainerBuilder builder)
        {
            //注册数据存储
            builder.Register<DbContext>(d => new AdaEFDbcontext()).InstancePerLifetimeScope();//EF上下文
            builder.RegisterGeneric(typeof(AdaEFRepository<>)).As(typeof(IRepository<>)).InstancePerLifetimeScope();//数据仓储

            //获取所有程序集
            var assemblys = BuildManager.GetReferencedAssemblies().Cast<Assembly>().ToArray();
            builder.RegisterControllers(assemblys);//注册控制器
            builder.RegisterAssemblyModules(assemblys);//所有继承module中的类都会被注册
            var dependencyType = typeof(IDependency);
            var singletonType = typeof(ISingleDependency);
            //注册依赖
            builder.RegisterAssemblyTypes(assemblys)
                .Where(t => dependencyType.IsAssignableFrom(t) && t != dependencyType && !t.IsAbstract)
                .AsImplementedInterfaces().InstancePerLifetimeScope();
            //注册单例
            builder.RegisterAssemblyTypes(assemblys)
                .Where(t => singletonType.IsAssignableFrom(t) && t != singletonType && !t.IsAbstract)
                .AsImplementedInterfaces().SingleInstance();
        }
    }
}
