using System.Data.Entity.ModelConfiguration;
using Ada.Core.Domain.Finance;

namespace Ada.Data.Mapping.Finance
{
   public class ReceivablesMap : EntityTypeConfiguration<Receivables>
    {
        public ReceivablesMap()
        {
            //配置主键
            HasKey(s => s.Id);
            //配置字段
            Property(s => s.ReceivablesType).HasMaxLength(32);
            Property(s => s.AccountBank).HasMaxLength(32);
            Property(s => s.AccountName).HasMaxLength(32);
            Property(s => s.AccountNum).HasMaxLength(32);
            Property(s => s.Transactor).HasMaxLength(32);
            Property(s => s.TransactorId).HasMaxLength(32);
            Property(s => s.IncomeExpendId).HasMaxLength(32);
            Property(s => s.IncomeExpendName).HasMaxLength(32);
            Property(s => s.SettleAccountName).HasMaxLength(32);
            Property(s => s.SettleType).HasMaxLength(32);
            Property(s => s.SettleAccountId).HasMaxLength(32);
            Property(s => s.BillNum).HasMaxLength(32);
            Property(s => s.RelationshipNum).HasMaxLength(32);

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
            ToTable("Receivables");
            HasRequired(s => s.SettleAccount).WithMany(s => s.Receivableses).HasForeignKey(s => s.SettleAccountId).WillCascadeOnDelete(false);
            HasRequired(s => s.IncomeExpend).WithMany(s => s.Receivableses).HasForeignKey(s => s.IncomeExpendId).WillCascadeOnDelete(false);
        }
    }
}
