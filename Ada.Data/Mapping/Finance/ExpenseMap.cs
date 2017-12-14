using System.Data.Entity.ModelConfiguration;
using Ada.Core.Domain.Finance;

namespace Ada.Data.Mapping.Finance
{
   public class ExpenseMap : EntityTypeConfiguration<Expense>
    {
        public ExpenseMap()
        {
            //配置主键
            HasKey(s => s.Id);
            //配置字段
            Property(s => s.BillNum).HasMaxLength(32);
            Property(s => s.LinkManName).HasMaxLength(32);
            Property(s => s.LinkManId).HasMaxLength(32);
            Property(s => s.AccountBank).HasMaxLength(32);
            Property(s => s.AccountName).HasMaxLength(32);
            Property(s => s.AccountNum).HasMaxLength(32);
            Property(s => s.Transactor).HasMaxLength(32);
            Property(s => s.TransactorId).HasMaxLength(32);
            Property(s => s.Employe).HasMaxLength(32);
            Property(s => s.EmployerId).HasMaxLength(32);
            Property(s => s.RequestNum).HasMaxLength(32);
            Property(s => s.Image).HasMaxLength(512);

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
            ToTable("Expense");
            
        }
    }
}
