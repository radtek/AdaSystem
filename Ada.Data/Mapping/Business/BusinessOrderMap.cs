using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core.Domain.Business;

namespace Ada.Data.Mapping.Business
{
    public class BusinessOrderMap : EntityTypeConfiguration<BusinessOrder>
    {
        public BusinessOrderMap()
        {
            //配置主键
            HasKey(s => s.Id);
            //配置字段
            Property(s => s.OrderNum).IsRequired().HasMaxLength(32);
            Property(s => s.BusinessType).HasMaxLength(32);
            Property(s => s.TotalMoney).IsRequired();
            Property(s => s.VerificationMoney);
            Property(s => s.ConfirmVerificationMoney);
            Property(s => s.VerificationStatus);
            Property(s => s.Status);
            Property(s => s.Tax);
            Property(s => s.DiscountRate);
            Property(s => s.AuditDate);
            Property(s => s.AuditStatus);
            Property(s => s.CancelDate);
            Property(s => s.OrderDate);
            Property(s => s.SettlementType).HasMaxLength(32);
            Property(s => s.Transactor).HasMaxLength(32);
            Property(s => s.TransactorId).HasMaxLength(32);
            Property(s => s.AuditBy).HasMaxLength(32);
            Property(s => s.AuditById).HasMaxLength(32);
            Property(s => s.CancelBy).HasMaxLength(32);
            Property(s => s.CancelById).HasMaxLength(32);
            Property(s => s.LinkManName).HasMaxLength(32);
            Property(s => s.LinkManId).HasMaxLength(32);


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
            ToTable("BusinessOrder");
        }
    }
}
