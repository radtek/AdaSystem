using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core.Domain.Vote;

namespace Ada.Data.Mapping.Vote
{
   public class VoteItemMap : EntityTypeConfiguration<VoteItem>
    {
        public VoteItemMap()
        {
            //配置主键
            HasKey(s => s.Id);
            //配置字段
            Property(s => s.Title).IsRequired().HasMaxLength(128);
            Property(s => s.Url).HasMaxLength(512);
            Property(s => s.Image).HasMaxLength(512);
            Property(s => s.VoteThemeId).HasMaxLength(32);
            Property(s => s.Abstract).HasMaxLength(128);


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
            ToTable("VoteItem");
            //配置关系【一对多的配置，外键是FKId】 Withmany方法允许多个。HasForeignKey方法表示哪个属性是外键表的外键，WillCascadeOnDelete方法用来配置是否级联删除
            HasRequired(s => s.VoteTheme).WithMany(s => s.VoteItems).HasForeignKey(s => s.VoteThemeId).WillCascadeOnDelete(true);
        }
    }
}
