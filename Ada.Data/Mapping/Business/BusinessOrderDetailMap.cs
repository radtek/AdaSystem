using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core.Domain.Business;

namespace Ada.Data.Mapping.Business
{
    public class BusinessOrderDetailMap : EntityTypeConfiguration<BusinessOrderDetail>
    {
        public BusinessOrderDetailMap()
        {
            //配置主键
            HasKey(s => s.Id);
            //配置字段
            Property(s => s.BusinessOrderId).HasMaxLength(32);
            Property(s => s.MediaPriceId).IsRequired().HasMaxLength(32);
            Property(s => s.PrePublishDate);
            Property(s => s.Tax);
            Property(s => s.DiscountRate);
            Property(s => s.TaxMoney);
            Property(s => s.DiscountMoney);
            Property(s => s.Money);
            Property(s => s.CostMoney);
            Property(s => s.SellMoney);
            Property(s => s.AdPositionName).HasMaxLength(32);
            Property(s => s.MediaTitle).HasMaxLength(512);
            Property(s => s.MediaTypeName).HasMaxLength(32);
            Property(s => s.MediaName).HasMaxLength(128);
            Property(s => s.MediaByPurchase).HasMaxLength(32);
            Property(s => s.MediaPriceId).HasMaxLength(32);


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
            ToTable("BusinessOrderDetail");
            //配置关系【一对多的配置，外键是UserId】 Withmany方法允许多个。HasForeignKey方法表示哪个属性是User表的外键，WillCascadeOnDelete方法用来配置是否级联删除
            HasRequired(s => s.BusinessOrder).WithMany(s => s.BusinessOrderDetails).HasForeignKey(s => s.BusinessOrderId).WillCascadeOnDelete(false);
            HasRequired(s => s.MediaPrice).WithMany(s => s.BusinessOrderDetails).HasForeignKey(s => s.MediaPriceId).WillCascadeOnDelete(false);
        }
    }
}
