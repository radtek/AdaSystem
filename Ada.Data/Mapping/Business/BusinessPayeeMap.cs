using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core.Domain.Business;

namespace Ada.Data.Mapping.Business
{
    public class BusinessPayeeMap : EntityTypeConfiguration<BusinessPayee>
    {
        public BusinessPayeeMap()
        {
            //配置主键
            HasKey(s => s.Id);
            //配置字段
            Property(s => s.Transactor).HasMaxLength(32);
            Property(s => s.TransactorId).HasMaxLength(32);
            Property(s => s.ReceivablesId).HasMaxLength(32);
            Property(s => s.LinkManId).HasMaxLength(32);
            Property(s => s.LinkManName).HasMaxLength(32);
            
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
            ToTable("BusinessPayee");
            HasRequired(s => s.Receivables).WithMany(s => s.BusinessPayees).HasForeignKey(s => s.ReceivablesId).WillCascadeOnDelete(false);
            HasRequired(s => s.LinkMan).WithMany(s => s.BusinessPayees).HasForeignKey(s => s.LinkManId).WillCascadeOnDelete(false);

        }
    }
}
