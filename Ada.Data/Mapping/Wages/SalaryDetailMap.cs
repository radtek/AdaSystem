using System;
using System.Data.Entity.ModelConfiguration;
using Ada.Core.Domain.Resource;
using Ada.Core.Domain.Wages;

namespace Ada.Data.Mapping.Wages
{
   public class SalaryDetailMap : EntityTypeConfiguration<SalaryDetail>
    {
        public SalaryDetailMap()
        {
            //配置主键
            HasKey(s => s.Id);
            //配置字段
            Property(s => s.ManagerId).HasMaxLength(32);
            
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
            ToTable("SalaryDetail");
            HasRequired(s => s.Manager).WithMany(s => s.SalaryDetails).HasForeignKey(s => s.ManagerId).WillCascadeOnDelete(false);



        }
    }
}
