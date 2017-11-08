using System;
using System.Data.Entity.ModelConfiguration;
using Ada.Core.Domain.Resource;

namespace Ada.Data.Mapping.Resource
{
   public class MediaMap : EntityTypeConfiguration<Media>
    {
        public MediaMap()
        {
            //配置主键
            HasKey(s => s.Id);
            //配置字段
            Property(s => s.MediaName).IsRequired().HasMaxLength(128);
            Property(s => s.MediaID).HasMaxLength(128);
            Property(s => s.MediaLink).HasMaxLength(512);
            Property(s => s.MediaLogo).HasMaxLength(512);
            Property(s => s.MediaQR).HasMaxLength(512);
            Property(s => s.IsAuthenticate);
            Property(s => s.IsOriginal);
            Property(s => s.IsComment);
            Property(s => s.FansNum);
            Property(s => s.LastReadNum);
            Property(s => s.AvgReadNum);
            Property(s => s.PublishFrequency);
            Property(s => s.LastPushDate);
            Property(s => s.Area).HasMaxLength(32);
            Property(s => s.AuthenticateType).HasMaxLength(32);
            Property(s => s.TransmitNum);
            Property(s => s.CommentNum);
            Property(s => s.LikesNum);
            Property(s => s.Content).HasColumnType("text");
            Property(s => s.ClickNum);
            Property(s => s.IsHot);
            Property(s => s.IsTop);
            Property(s => s.IsSlide);
            Property(s => s.IsRecommend);
            Property(s => s.Status);
            Property(s => s.ApiUpDate);
            Property(s => s.MediaTypeId).HasMaxLength(32);
            Property(s => s.LinkManId).HasMaxLength(32);


            Property(s => s.AddedDate);
            Property(s => s.AddedBy).HasMaxLength(32);
            Property(s => s.AddedById).HasMaxLength(32);
            Property(s => s.ModifiedDate);
            Property(s => s.ModifiedBy).HasMaxLength(32);
            Property(s => s.ModifiedById).HasMaxLength(32);
            Property(s => s.IsDelete).IsRequired();
            Property(s => s.DeletedDate);
            Property(s => s.DeletedBy).HasMaxLength(32);
            Property(s => s.DeletedById).HasMaxLength(32);
            Property(s => s.IpAddress).HasMaxLength(32);
            Property(s => s.Taxis);
            Property(s => s.Remark).HasMaxLength(1024);

            //配置表
            ToTable("Media");
            //配置关系【一对多的配置，外键是UserId】 Withmany方法允许多个。HasForeignKey方法表示哪个属性是User表的外键，WillCascadeOnDelete方法用来配置是否级联删除
            HasRequired(s => s.MediaType).WithMany(s => s.Medias).HasForeignKey(s => s.MediaTypeId).WillCascadeOnDelete(true);
            HasRequired(s => s.LinkMan).WithMany(s => s.Medias).HasForeignKey(s => s.LinkManId).WillCascadeOnDelete(true);

            //配置关系[多个角色，可以被多个用户选择]
            //多对多关系实现要领：hasmany,hasmany,然后映射生成第三个表，最后映射leftkey,rightkey
            HasMany(s => s.MediaTags).
                WithMany(s => s.Medias)
                .Map(s => s.ToTable("MediaAndTag").
                    MapLeftKey("MediaId").
                    MapRightKey("TagId"));
        }
    }
}
