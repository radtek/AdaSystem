using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core.Domain.Business;

namespace Ada.Data.Mapping.Business
{
    public class BusinessPaymentMap : EntityTypeConfiguration<BusinessPayment>
    {
        public BusinessPaymentMap()
        {
            //配置主键
            HasKey(s => s.Id);
            //配置字段
            Property(s => s.AccountBank).HasMaxLength(32);
            Property(s => s.AccountName).HasMaxLength(32);
            Property(s => s.AccountNum).HasMaxLength(32);
            Property(s => s.AuditBy).HasMaxLength(32);
            Property(s => s.AuditById).HasMaxLength(32);
            Property(s => s.CancelBy).HasMaxLength(32);
            Property(s => s.CancelById).HasMaxLength(32);
            Property(s => s.ApplicationNum).HasMaxLength(32);
            Property(s => s.PaymentType).HasMaxLength(32);
            Property(s => s.Transactor).HasMaxLength(32);
            Property(s => s.TransactorId).HasMaxLength(32);
            Property(s => s.Image).HasMaxLength(512);
            Property(s => s.BusinessPayeeId).HasMaxLength(32);
            

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
            ToTable("BusinessPayment");
            HasRequired(s => s.BusinessPayee).WithMany(s => s.BusinessPayments).HasForeignKey(s => s.BusinessPayeeId).WillCascadeOnDelete(false);

        }
    }
}
