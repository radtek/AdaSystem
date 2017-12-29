using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core.Domain.Business;
using Ada.Core.Domain.Purchase;

namespace Ada.Data.Mapping.Purchase
{
    public class PurchasePaymentMap : EntityTypeConfiguration<PurchasePayment>
    {
        public PurchasePaymentMap()
        {
            //配置主键
            HasKey(s => s.Id);
            //配置字段
            Property(s => s.LinkManName).HasMaxLength(32);
            Property(s => s.LinkManId).HasMaxLength(32);
            Property(s => s.BillNum).HasMaxLength(32);
            Property(s => s.Transactor).HasMaxLength(32);
            Property(s => s.TransactorId).HasMaxLength(32);
            Property(s => s.InvoiceNum).HasMaxLength(32);
            Property(s => s.InvoiceTitle).HasMaxLength(32);
            Property(s => s.AuditBy).HasMaxLength(32);
            Property(s => s.AuditById).HasMaxLength(32);

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
            ToTable("PurchasePayment");
            HasRequired(s => s.LinkMan).WithMany(s => s.PurchasePayments).HasForeignKey(s => s.LinkManId).WillCascadeOnDelete(false);
        }
    }
}
