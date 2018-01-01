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
    public class PurchaseOrderMap : EntityTypeConfiguration<PurchaseOrder>
    {
        public PurchaseOrderMap()
        {
            //配置主键
            HasKey(s => s.Id);
            //配置字段
            Property(s => s.OrderNum).IsRequired().HasMaxLength(32);
            Property(s => s.BusinessOrderId).HasMaxLength(32);
            Property(s => s.TotalMoney);
            Property(s => s.TotalDiscountMoney);
            Property(s => s.TotalPurchaseMoney);
            Property(s => s.TotalTaxMoney);
            Property(s => s.Status);
            Property(s => s.OrderDate);
            Property(s => s.TotalBargainMoney);
            Property(s => s.BusinessBy).HasMaxLength(32);
            Property(s => s.BusinessById).HasMaxLength(32);


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
            ToTable("PurchaseOrder");
        }
    }
}
