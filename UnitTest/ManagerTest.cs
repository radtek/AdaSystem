using System;
using System.Data.Entity;
using Ada.Core;
using Ada.Core.Domain.Admin;
using Ada.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest
{
    [TestClass]
    public class ManagerTest
    {
        [TestMethod]
        public void Test()

        {
            //配置数据库初始化策略
            Database.SetInitializer<AdaEFDbcontext>(new CreateDatabaseIfNotExists<AdaEFDbcontext>());
            using (var db = new AdaEFDbcontext())
            {
                //创建数据库
                db.Database.Create();

               Manager manager=new Manager()
               {
                   Id = IdBuilder.CreateIdNum(),
                   UserName = "xjb",
                   Password = "123456"

               };
                //设置用户示例状态为Added

                db.Entry(manager).State = System.Data.Entity.EntityState.Added;
                //保存到数据库中
                db.SaveChanges();
            }
        }
    }
}
