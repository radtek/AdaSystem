using System.Data.Entity.ModelConfiguration;
using Ada.Core.Domain.Common;
using Ada.Core.Domain.Content;

namespace Ada.Data.Mapping.Common
{
    public class FansMap : EntityTypeConfiguration<Fans>
    {
        public FansMap()
        {
            //配置主键
            HasKey(s => s.Id);
            //配置字段

            Property(s => s.NickName).IsRequired().HasMaxLength(64);
            Property(s => s.Type).HasMaxLength(32);
            Property(s => s.Avatar).HasMaxLength(512);
            Property(s => s.Cover).HasMaxLength(512);
            Property(s => s.ParentId).HasMaxLength(32);
            Property(s => s.AvatarRange).HasMaxLength(32);
            Property(s => s.FansRange).HasMaxLength(32);

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
            ToTable("Fans");
        }
    }
}
