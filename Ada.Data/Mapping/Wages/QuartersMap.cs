using System;
using System.Data.Entity.ModelConfiguration;
using Ada.Core.Domain.Resource;
using Ada.Core.Domain.Wages;

namespace Ada.Data.Mapping.Wages
{
   public class QuartersMap : EntityTypeConfiguration<Quarters>
    {
        public QuartersMap()
        {
            //配置主键
            HasKey(s => s.Id);
            //配置字段
            Property(s => s.Title).IsRequired().HasMaxLength(128);
            
            Property(s => s.AddedBy).HasMaxLength(32);
            Property(s => s.AddedById).HasMaxLength(32);
            Property(s => s.ModifiedBy).HasMaxLength(32);
            Property(s => s.ModifiedById).HasMaxLength(32);
            Property(s => s.IsDelete).IsRequired();
            Property(s => s.DeletedBy).HasMaxLength(32);
            Property(s => s.DeletedById).HasMaxLength(32);
            Property(s => s.IpAddress).HasMaxLength(32);
            Property(s => s.Remark).HasMaxLength(1024);
           
            //配置表
            ToTable("Quarters");
            
           

            
        }
    }
}
