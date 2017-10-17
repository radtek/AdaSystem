using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core.Domain.Admin;

namespace Ada.Data.Mapping.Admin
{
   public class ActionMap : EntityTypeConfiguration<Core.Domain.Admin.Action>
    {
        public ActionMap()
        {
            //配置主键
            HasKey(s => s.Id);

            ////给ID配置自动增长
            //this.Property(s => s.ID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            //配置字段
            Property(s => s.ActionName).IsRequired().HasMaxLength(32);
            Property(s => s.Area).HasMaxLength(16);
            Property(s => s.ControllerName).HasMaxLength(16);
            Property(s => s.MethodName).HasMaxLength(16);
            Property(s => s.LinkUrl).HasMaxLength(512);
            Property(s => s.HttpMethod).HasMaxLength(16);
            Property(s => s.TreePath).HasMaxLength(32);
            Property(s => s.ParentId).HasMaxLength(128);
            Property(s => s.IsLeaf);
            Property(s => s.IsButton);
            Property(s => s.Number);
            Property(s => s.Level);
            Property(s => s.IsMenu);
            Property(s => s.Image).HasMaxLength(512);
            Property(s => s.IconCls).HasMaxLength(32);
            Property(s => s.ActionType).HasMaxLength(32);



            Property(s => s.AddedDate);
            Property(s => s.AddedBy).HasMaxLength(32);
            Property(s => s.ModifiedDate);
            Property(s => s.ModifiedBy).HasMaxLength(32);
            Property(s => s.IsDelete).IsRequired();
            Property(s => s.DeletedDate);
            Property(s => s.DeletedBy).HasMaxLength(32);
            Property(s => s.IpAddress).HasMaxLength(32);
            Property(s => s.Taxis);
            Property(s => s.Remark).HasMaxLength(1024);

            //配置表
            ToTable("Action");

            //配置关系[多个角色，可以被多个用户选择]
            //多对多关系实现要领：hasmany,hasmany,然后映射生成第三个表，最后映射leftkey,rightkey
            HasMany(s => s.Roles).
                WithMany(s => s.Actions)
                .Map(s => s.ToTable("ActionRole").
                    MapLeftKey("ActionId").
                    MapRightKey("RoleId"));
        }
    }
}
