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
    public class PurchasePaymentOrderDetailMap : EntityTypeConfiguration<PurchasePaymentOrderDetail>
    {
        public PurchasePaymentOrderDetailMap()
        {
            //配置主键
            HasKey(s => s.Id);
            //配置字段
            Property(s => s.PurchaseOrderDetailId).HasMaxLength(32);
            Property(s => s.PurchasePaymentId).HasMaxLength(32);
            


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
            ToTable("PurchasePaymentOrderDetail");
            HasRequired(s => s.PurchaseOrderDetail).WithMany(s => s.PurchasePaymentOrderDetails).HasForeignKey(s => s.PurchaseOrderDetailId).WillCascadeOnDelete(false);
            HasRequired(s => s.PurchasePayment).WithMany(s => s.PurchasePaymentOrderDetails).HasForeignKey(s => s.PurchasePaymentId).WillCascadeOnDelete(false);
        }
    }
}
