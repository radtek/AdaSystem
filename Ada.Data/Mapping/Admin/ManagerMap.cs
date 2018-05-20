using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core.Domain.Admin;

namespace Ada.Data.Mapping.Admin
{
   public class ManagerMap: EntityTypeConfiguration<Manager>
    {
        public ManagerMap()
        {
            //配置主键
            HasKey(s => s.Id);

            ////给ID配置自动增长
            //this.Property(s => s.ID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            //配置字段
            Property(s => s.UserName).IsRequired().HasMaxLength(32);
            Property(s => s.RealName).HasMaxLength(32);
            Property(s => s.Phone).HasMaxLength(16);
            Property(s => s.Password).IsRequired().HasMaxLength(32);
            Property(s => s.OpenId).HasMaxLength(64);
            Property(s => s.Status);
            Property(s => s.Image).HasMaxLength(512);
            Property(s => s.Theme).HasMaxLength(32);
            Property(s => s.ExpireTime);
            Property(s => s.IdCard).HasMaxLength(32);
            Property(s => s.UnionId).HasMaxLength(64);
            Property(s => s.AddedDate);
            Property(s => s.AddedBy).HasMaxLength(32);
            Property(s => s.AddedById).HasMaxLength(32);
            Property(s => s.ModifiedDate);
            Property(s => s.ModifiedBy).HasMaxLength(32);
            Property(s => s.ModifiedById).HasMaxLength(32);
            Property(s => s.IsDelete).IsRequired();
            Property(s => s.DeletedDate);
            Property(s => s.DeletedBy).HasMaxLength(32);
            Property(s => s.DeletedById).HasMaxLength(32);
            Property(s => s.IpAddress).HasMaxLength(32);
            Property(s => s.Taxis);
            Property(s => s.Remark).HasMaxLength(1024);

            //配置表
            ToTable("Manager");

            //配置关系[多个角色，可以被多个用户选择]
            //多对多关系实现要领：hasmany,hasmany,然后映射生成第三个表，最后映射leftkey,rightkey
            HasMany(s => s.Roles).
                WithMany(s => s.Managers)
                .Map(s => s.ToTable("ManagerRole").
                    MapLeftKey("ManagerId").
                    MapRightKey("RoleId"));

            HasMany(s => s.Organizations).
                WithMany(s => s.Managers)
                .Map(s => s.ToTable("ManagerOrganization").
                    MapLeftKey("ManagerId").
                    MapRightKey("OrganizationId"));

           
        }
    }
}
