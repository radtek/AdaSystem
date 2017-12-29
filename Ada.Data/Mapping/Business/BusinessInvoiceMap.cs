using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core.Domain.Business;

namespace Ada.Data.Mapping.Business
{
  public  class BusinessInvoiceMap: EntityTypeConfiguration<BusinessInvoice>
    {
        public BusinessInvoiceMap()
        {
            //配置主键
            HasKey(s => s.Id);
            //配置字段
            Property(s => s.InvoiceTitle).HasMaxLength(32);
            Property(s => s.Transactor).HasMaxLength(32);
            Property(s => s.TransactorId).HasMaxLength(32);
            Property(s => s.InvoiceType).HasMaxLength(32);
            Property(s => s.Company).HasMaxLength(32);
            Property(s => s.TaxNum).HasMaxLength(32);
            Property(s => s.Address).HasMaxLength(32);
            Property(s => s.Bank).HasMaxLength(32);
            Property(s => s.BankNum).HasMaxLength(32);
            Property(s => s.Phone).HasMaxLength(32);
            Property(s => s.LinkManName).HasMaxLength(32);
            Property(s => s.LinkManId).HasMaxLength(32);
            Property(s => s.ReceivableNum).HasMaxLength(32);
            Property(s => s.InvoiceNum).HasMaxLength(32);
            Property(s => s.AuditBy).HasMaxLength(32);
            Property(s => s.AuditById).HasMaxLength(32);

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
            ToTable("BusinessInvoice");
            HasRequired(s => s.LinkMan).WithMany(s => s.BusinessInvoices).HasForeignKey(s => s.LinkManId).WillCascadeOnDelete(false);
        }
    }
}
