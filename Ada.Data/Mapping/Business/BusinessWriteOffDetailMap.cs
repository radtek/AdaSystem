using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core.Domain.Business;

namespace Ada.Data.Mapping.Business
{
   public class BusinessWriteOffDetailMap: EntityTypeConfiguration<BusinessWriteOffDetail>
    {
        public BusinessWriteOffDetailMap()
        {
            //配置主键
            HasKey(s => s.Id);
            //配置字段
            Property(s => s.BusinessOrderDetailId).HasMaxLength(32);
            Property(s => s.BusinessWriteOffId).HasMaxLength(32);
            Property(s => s.BusinessOrderId).HasMaxLength(32);
            Property(s => s.MediaTypeId).HasMaxLength(32);

            Property(s => s.AddedBy).HasMaxLength(32);
            Property(s => s.AddedById).HasMaxLength(32);
            Property(s => s.ModifiedBy).HasMaxLength(32);
            Property(s => s.ModifiedById).HasMaxLength(32);
            Property(s => s.IsDelete).IsRequired();
            Property(s => s.DeletedBy).HasMaxLength(32);
            Property(s => s.DeletedById).HasMaxLength(32);
            Property(s => s.IpAddress).HasMaxLength(32);
            Property(s => s.Remark).HasMaxLength(1024);
            ToTable("BusinessWriteOffDetail");
            //配置关系【一对多的配置，外键是UserId】 Withmany方法允许多个。HasForeignKey方法表示哪个属性是User表的外键，WillCascadeOnDelete方法用来配置是否级联删除
            HasRequired(s => s.BusinessWriteOff).WithMany(s => s.BusinessWriteOffDetails).HasForeignKey(s => s.BusinessWriteOffId).WillCascadeOnDelete(true);
        }
    }
}
