using System.Data.Entity.ModelConfiguration;
using Ada.Core.Domain.Content;

namespace Ada.Data.Mapping.Content
{
    public class ArticleMap : EntityTypeConfiguration<Article>
    {
        public ArticleMap()
        {
            //配置主键
            HasKey(s => s.Id);
            //配置字段

            Property(s => s.Title).IsRequired().HasMaxLength(128);
            Property(s => s.Type).HasMaxLength(32);
            Property(s => s.ColumnId).HasMaxLength(32);
            Property(s => s.Summary).HasMaxLength(256);
            Property(s => s.Author).HasMaxLength(32);
            Property(s => s.Source).HasMaxLength(32);
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
            ToTable("Article");
            HasRequired(s => s.Column).WithMany(s => s.Articles).HasForeignKey(s => s.ColumnId).WillCascadeOnDelete(false);
            HasMany(s => s.Attachments).
                WithMany(s => s.Articles)
                .Map(s => s.ToTable("ArticleAttachment").
                    MapLeftKey("ArticleId").
                    MapRightKey("AttachmentId"));
        }
    }
}
