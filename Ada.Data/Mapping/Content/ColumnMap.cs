using System.Data.Entity.ModelConfiguration;
using Ada.Core.Domain.Content;

namespace Ada.Data.Mapping.Content
{
    public class ColumnMap : EntityTypeConfiguration<Column>
    {
        public ColumnMap()
        {
            //配置主键
            HasKey(s => s.Id);
            //配置字段

            Property(s => s.Title).IsRequired().HasMaxLength(32);
            Property(s => s.Type).HasMaxLength(32);
            Property(s => s.CallIndex).HasMaxLength(32);
            Property(s => s.ParentId).HasMaxLength(32);
            Property(s => s.TreePath).HasMaxLength(1024);
            Property(s => s.CoverPic).HasMaxLength(512);
            Property(s => s.Url).HasMaxLength(512);

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
            ToTable("Column");
        }
    }
}
