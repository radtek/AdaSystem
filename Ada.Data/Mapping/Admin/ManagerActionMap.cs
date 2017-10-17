using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core.Domain.Admin;

namespace Ada.Data.Mapping.Admin
{
   public class ManagerActionMap : EntityTypeConfiguration<ManagerAction>
    {
        public ManagerActionMap()
        {
            //配置主键
            HasKey(s => s.Id);

            ////给ID配置自动增长
            //this.Property(s => s.ID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            //配置字段
           
            Property(s => s.IsPass);
            Property(s => s.ManagerId).HasMaxLength(128);
            Property(s => s.ActionInfoId).HasMaxLength(128);
            



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
            ToTable("ManagerAction");

            //配置关系【一对多的配置，外键是UserId】 Withmany方法允许多个。HasForeignKey方法表示哪个属性是User表的外键，WillCascadeOnDelete方法用来配置是否级联删除
            HasRequired(s => s.Manager).WithMany(s => s.ManagerActions).HasForeignKey(s => s.ManagerId).WillCascadeOnDelete(true);
            HasRequired(s => s.Action).WithMany(s => s.ManagerActions).HasForeignKey(s => s.ActionInfoId).WillCascadeOnDelete(true);
        }
    }
}
