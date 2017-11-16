﻿
using System.Data.Entity.ModelConfiguration;
using Ada.Core.Domain.Purchase;

namespace Ada.Data.Mapping.Purchase
{
    public class PurchaseOrderDetailMap : EntityTypeConfiguration<PurchaseOrderDetail>
    {
        public PurchaseOrderDetailMap()
        {
            //配置主键
            HasKey(s => s.Id);
            //配置字段
            Property(s => s.PurchaseOrderId).HasMaxLength(32);
            Property(s => s.BusinessOrderDetailId).HasMaxLength(32);
            Property(s => s.MediaPriceId).HasMaxLength(32);
            Property(s => s.PublishDate);
            Property(s => s.DiscountRate);
            Property(s => s.DiscountMoney);
            Property(s => s.Money);
            Property(s => s.TaxMoney);
            Property(s => s.Tax);
            Property(s => s.CostMoney);
            Property(s => s.PurchaseMoney);
            Property(s => s.AdPositionName).HasMaxLength(32);
            Property(s => s.MediaTitle).HasMaxLength(512);
            Property(s => s.MediaTypeName).HasMaxLength(32);
            Property(s => s.PublishLink).HasMaxLength(512);
            Property(s => s.MediaName).HasMaxLength(128);


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
            ToTable("PurchaseOrderDetail");
            //配置关系【一对多的配置，外键是UserId】 Withmany方法允许多个。HasForeignKey方法表示哪个属性是User表的外键，WillCascadeOnDelete方法用来配置是否级联删除
            HasRequired(s => s.PurchaseOrder).WithMany(s => s.PurchaseOrderDetails).HasForeignKey(s => s.PurchaseOrderId).WillCascadeOnDelete(true);
        }
    }
}
