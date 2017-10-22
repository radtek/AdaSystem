using Ada.Core;
using Ada.Core.Domain;
using Ada.Core.Domain.Admin;
using Ada.Core.Tools;

namespace Ada.Data.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Ada.Data.AdaEFDbcontext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = false;
        }

        protected override void Seed(Ada.Data.AdaEFDbcontext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
            var manager = context.Set<Manager>().FirstOrDefault(d => d.UserName == "adaxiong" && d.IsDelete == false);
            if (manager==null)
            {
                context.Set<Manager>().Add(new Manager()
                {
                    Id = IdBuilder.CreateIdNum(),
                    UserName = "adaxiong",
                    Password = Encrypt.Encode("123456"),
                    Status = Consts.StateNormal
                });
            }
            
        }
    }
}
