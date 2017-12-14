using System.Data.Entity.ModelConfiguration;
using Ada.Core.Domain.Finance;

namespace Ada.Data.Mapping.Finance
{
   public class ExpenseDetailMap : EntityTypeConfiguration<ExpenseDetail>
    {
        public ExpenseDetailMap()
        {
            //配置主键
            HasKey(s => s.Id);
            //配置字段
            Property(s => s.IncomeExpendId).HasMaxLength(32);
            Property(s => s.IncomeExpendName).HasMaxLength(32);
            Property(s => s.SettleAccountName).HasMaxLength(32);
            Property(s => s.SettleType).HasMaxLength(32);
            Property(s => s.SettleAccountId).HasMaxLength(32);
            Property(s => s.SettleNum).HasMaxLength(32);
            Property(s => s.ExpenseId).HasMaxLength(32);
           

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
            ToTable("ExpenseDetail");
            //配置关系【一对多的配置，外键是UserId】 Withmany方法允许多个。HasForeignKey方法表示哪个属性是User表的外键，WillCascadeOnDelete方法用来配置是否级联删除
            HasRequired(s => s.Expense).WithMany(s => s.ExpenseDetails).HasForeignKey(s => s.ExpenseId).WillCascadeOnDelete(true);
            HasRequired(s => s.SettleAccount).WithMany(s => s.ExpenseDetails).HasForeignKey(s => s.SettleAccountId).WillCascadeOnDelete(false);
            HasRequired(s => s.IncomeExpend).WithMany(s => s.ExpenseDetails).HasForeignKey(s => s.IncomeExpendId).WillCascadeOnDelete(false);
        }
    }
}
