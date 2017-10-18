using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Data
{
    /// <summary>
    /// EF上下文
    /// </summary>
    public class AdaEFDbcontext : DbContext
    {
        public AdaEFDbcontext() : base("name=ADADCS")
        {
            //Enable-Migrations 开启数据迁移
            //对 Configuration.cs 修改 AutomaticMigrationsEnabled = true; AutomaticMigrationDataLossAllowed = false;//不允许数据丢失
            //Add-Migration UserProfile 对某一实体类进行数据更新
            //Update-Database -Verbose 更新数据库
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //通过反射获得Map实体类
            var typesToRegister = Assembly.GetExecutingAssembly().GetTypes()
                .Where(type => !string.IsNullOrEmpty(type.Namespace))
                .Where(type => type.BaseType != null && type.BaseType.IsGenericType
                               && type.BaseType.GetGenericTypeDefinition() == typeof(EntityTypeConfiguration<>));
            foreach (var type in typesToRegister)
            {
                dynamic configurationInstance = Activator.CreateInstance(type);
                modelBuilder.Configurations.Add(configurationInstance);
            }
            base.OnModelCreating(modelBuilder);
        }

    }
}
