using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core.Domain.Business;

namespace Ada.Data.Mapping.Business
{
    public class BusinessWriteOffMap : EntityTypeConfiguration<BusinessWriteOff>
    {
        public BusinessWriteOffMap()
        {
            //配置主键
            HasKey(s => s.Id);
            //配置字段
            Property(s => s.Transactor).HasMaxLength(32);
            Property(s => s.TransactorId).HasMaxLength(32);
            
            //Property(s => s.AddedDate);
            Property(s => s.AddedBy).HasMaxLength(32);
            Property(s => s.AddedById).HasMaxLength(32);
            //Property(s => s.ModifiedDate);
            Property(s => s.ModifiedBy).HasMaxLength(32);
            Property(s => s.ModifiedById).HasMaxLength(32);
            Property(s => s.IsDelete).IsRequired();
            //Property(s => s.DeletedDate);
            Property(s => s.DeletedBy).HasMaxLength(32);
            Property(s => s.DeletedById).HasMaxLength(32);
            Property(s => s.IpAddress).HasMaxLength(32);
            //Property(s => s.Taxis);
            Property(s => s.Remark).HasMaxLength(1024);

            //配置表
            ToTable("BusinessWriteOff");
            HasMany(s => s.BusinessOrders).
                WithMany(s => s.BusinessWriteOffs)
                .Map(s => s.ToTable("BusinessOrderWriteOff").
                    MapLeftKey("BusinessWriteOffId").
                    MapRightKey("BusinessOrderId"));
            HasMany(s => s.BusinessPayees).
                WithMany(s => s.BusinessWriteOffs)
                .Map(s => s.ToTable("BusinessPayeeWriteOff").
                    MapLeftKey("BusinessWriteOffId").
                    MapRightKey("BusinessPayeeId"));

        }
    }
}
