using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core.Domain.Business;

namespace Ada.Data.Mapping.Business
{
  public  class BusinessInvoiceDetailMap : EntityTypeConfiguration<BusinessInvoiceDetail>
    {
        public BusinessInvoiceDetailMap()
        {
            //配置主键
            HasKey(s => s.Id);
            //配置字段
            Property(s => s.BusinessOrderId).HasMaxLength(32);
            Property(s => s.BusinessInvoiceId).HasMaxLength(32);
            


            Property(s => s.AddedBy).HasMaxLength(32);
            Property(s => s.AddedById).HasMaxLength(32);
            Property(s => s.ModifiedBy).HasMaxLength(32);
            Property(s => s.ModifiedById).HasMaxLength(32);
            Property(s => s.IsDelete).IsRequired();
            Property(s => s.DeletedBy).HasMaxLength(32);
            Property(s => s.DeletedById).HasMaxLength(32);
            Property(s => s.IpAddress).HasMaxLength(32);
            Property(s => s.Remark).HasMaxLength(1024);

            //配置表
            ToTable("BusinessInvoiceDetail");
            HasRequired(s => s.BusinessInvoice).WithMany(s => s.BusinessInvoiceDetails).HasForeignKey(s => s.BusinessInvoiceId).WillCascadeOnDelete(false);
            HasRequired(s => s.BusinessOrder).WithMany(s => s.BusinessInvoiceDetails).HasForeignKey(s => s.BusinessOrderId).WillCascadeOnDelete(false);
        }
    }
}
