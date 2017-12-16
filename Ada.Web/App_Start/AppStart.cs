using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Compilation;
using Ada.Core;

namespace Ada.Web
{
    public class AppStart
    {
        public static void Register()
        {
            // 找到所有引用的程序集
            var assemblies = BuildManager.GetReferencedAssemblies();
            foreach (Assembly assembly in assemblies)
            {
                var types = assembly.GetTypes();
                foreach (Type type in types)
                {
                    // 判断是否为注册接口的实现类（必须实现IMoudle）
                    if (type.IsClass
                        && !type.IsAbstract
                        && !type.IsInterface
                        && typeof(IAppStart).IsAssignableFrom(type))
                    {
                        //获取实例，并执行注册方法
                        var moudle = Activator.CreateInstance(type) as IAppStart;
                        moudle?.Register();
                    }
                }
            }
        }
    }
}