using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Compilation;
using System.Web.Mvc;
using Ada.Core.Infrastructure.Dependency;
using Autofac;
using Autofac.Integration.Mvc;

namespace Ada.Core.Infrastructure
{
    public class AdaEngine : IEngine
    {
        private ContainerManager _containerManager;
        /// <summary>
        /// Container manager
        /// </summary>
        public virtual ContainerManager ContainerManager => _containerManager;

        public T Resolve<T>() where T : class
        {
            return ContainerManager.Resolve<T>();
        }

        public object Resolve(Type type)
        {
            return ContainerManager.Resolve(type);
        }

        public T[] ResolveAll<T>()
        {
            return ContainerManager.ResolveAll<T>();
        }
        /// <summary>
        /// 初始化
        /// </summary>
        public void Initialize()
        {
            //register dependencies
            RegisterDependencies();
        }

        /// <summary>
        /// 依赖注入
        /// </summary>
        protected virtual void RegisterDependencies()
        {
            var builder = new ContainerBuilder();
            builder.RegisterInstance(this).As<IEngine>().SingleInstance();
            //获取所有程序集
            var assemblys = BuildManager.GetReferencedAssemblies().Cast<Assembly>().ToArray();
            //获取有注册依赖接口的实现对象
            var drInstances = new List<IDependencyRegistrar>();
            foreach (var assembly in assemblys)
            {
                var types = assembly.GetTypes();
                foreach (Type type in types)
                {
                    // 判断是否为注册接口的实现类（必须实现IMoudle）
                    if (type.IsClass
                        && !type.IsAbstract
                        && !type.IsInterface
                        && typeof(IDependencyRegistrar).IsAssignableFrom(type))
                    {
                       drInstances.Add((IDependencyRegistrar)Activator.CreateInstance(type));
                    }
                }
            }
            drInstances = drInstances.AsQueryable().OrderBy(t => t.Order).ToList();
            foreach (var dependencyRegistrar in drInstances)
                dependencyRegistrar.Register(builder);

            var container = builder.Build();
            this._containerManager = new ContainerManager(container);
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}
