using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core.Domain.WeiXin;

namespace Ada.Data.Mapping.WeiXin
{
   public class WeiXinAccountMap : EntityTypeConfiguration<WeiXinAccount>
    {
        public WeiXinAccountMap()
        {
            //配置主键
            HasKey(s => s.Id);
            //配置字段

            Property(s => s.Name).HasMaxLength(32);
            Property(s => s.SourceId).HasMaxLength(32);
            Property(s => s.AppId).HasMaxLength(32);
            Property(s => s.AppSecret).HasMaxLength(32);
            Property(s => s.Token).HasMaxLength(32);
            Property(s => s.EncodingAESKey).HasMaxLength(64);
            Property(s => s.MchId).HasMaxLength(32);
            Property(s => s.MchKey).HasMaxLength(64);
            Property(s => s.NotifyUrl).HasMaxLength(512);
            Property(s => s.CretPath).HasMaxLength(512);
            Property(s => s.Image).HasMaxLength(512);


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
            ToTable("WeiXinAccount");
        }
    }
}
