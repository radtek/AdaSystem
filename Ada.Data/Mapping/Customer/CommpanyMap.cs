using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core.Domain.Admin;
using Ada.Core.Domain.Customer;

namespace Ada.Data.Mapping.Customer
{
   public class CommpanyMap : EntityTypeConfiguration<Commpany>
    {
        public CommpanyMap()
        {
            //配置主键
            HasKey(s => s.Id);
            //配置字段
            Property(s => s.Name).IsRequired().HasMaxLength(32);
            Property(s => s.City).HasMaxLength(32);
            Property(s => s.CommpanyType).HasMaxLength(32);
            Property(s => s.CommpanyGrade).HasMaxLength(32);
            Property(s => s.Address).HasMaxLength(128);
            Property(s => s.Phone).HasMaxLength(64);
            Property(s => s.IsBusiness).IsRequired();
            Property(s => s.Transactor).HasMaxLength(32);
            Property(s => s.TransactorId).HasMaxLength(32);

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
            ToTable("Commpany");
        }
    }
}
