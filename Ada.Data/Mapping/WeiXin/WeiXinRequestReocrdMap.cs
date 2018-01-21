using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core.Domain.WeiXin;

namespace Ada.Data.Mapping.WeiXin
{
   public class WeiXinRequestReocrdMap : EntityTypeConfiguration<WeiXinRequestReocrd>
    {
        public WeiXinRequestReocrdMap()
        {
            //配置主键
            HasKey(s => s.Id);
            //配置字段

            Property(s => s.FromUserName).HasMaxLength(32);
            Property(s => s.ToUserName).HasMaxLength(32);
            Property(s => s.RequestType).HasMaxLength(32);
            Property(s => s.ReponseType).HasMaxLength(32);
            Property(s => s.WeiXinAccountId).HasMaxLength(32);
            


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
            ToTable("WeiXinRequestReocrd");
            //配置关系【一对多的配置，外键是UserId】 Withmany方法允许多个。HasForeignKey方法表示哪个属性是User表的外键，WillCascadeOnDelete方法用来配置是否级联删除
            HasRequired(s => s.WeiXinAccount).WithMany(s => s.WeiXinRequestReocrds).HasForeignKey(s => s.WeiXinAccountId).WillCascadeOnDelete(false);
        }
    }
}
