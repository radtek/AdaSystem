using System.Data.Entity.ModelConfiguration;
using Ada.Core.Domain.Finance;

namespace Ada.Data.Mapping.Finance
{
   public class SettleAccountMap : EntityTypeConfiguration<SettleAccount>
    {
        public SettleAccountMap()
        {
            //配置主键
            HasKey(s => s.Id);
            //配置字段
            Property(s => s.SettleName).HasMaxLength(32);
            Property(s => s.AccountBank).HasMaxLength(32);
            Property(s => s.AccountName).HasMaxLength(32);
            Property(s => s.AccountNum).HasMaxLength(32);


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
            ToTable("SettleAccount");
        }
    }
}
