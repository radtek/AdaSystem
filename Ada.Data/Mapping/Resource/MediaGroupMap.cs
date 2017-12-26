using System.Data.Entity.ModelConfiguration;
using Ada.Core.Domain.Resource;

namespace Ada.Data.Mapping.Resource
{
   public class MediaGroupMap : EntityTypeConfiguration<MediaGroup>
    {
        public MediaGroupMap()
        {
            //配置主键
            HasKey(s => s.Id);
            //配置字段
            Property(s => s.GroupName).HasMaxLength(32);

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
            ToTable("MediaGroup");
            //配置关系[多个角色，可以被多个用户选择]
            //多对多关系实现要领：hasmany,hasmany,然后映射生成第三个表，最后映射leftkey,rightkey
            HasMany(s => s.Medias).
                WithMany(s => s.MediaGroups)
                .Map(s => s.ToTable("MediaAndGroup").
                    MapLeftKey("MediaId").
                    MapRightKey("MediaGroupId"));
        }
    }
}
