using System.Data.Entity.ModelConfiguration;
using Ada.Core.Domain.Common;
using Ada.Core.Domain.Content;

namespace Ada.Data.Mapping.Common
{
    public class AttachmentMap : EntityTypeConfiguration<Attachment>
    {
        public AttachmentMap()
        {
            //配置主键
            HasKey(s => s.Id);
            //配置字段

            Property(s => s.Title).HasMaxLength(128);
            Property(s => s.Describe).HasMaxLength(512);
            Property(s => s.Path).HasMaxLength(512);
            Property(s => s.ThumbPath).HasMaxLength(512);
            Property(s => s.FileExt).HasMaxLength(8);

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
            ToTable("Attachment");
        }
    }
}
