using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core.Domain.Admin;
using Ada.Core.Domain.Customer;
using Ada.Core.Domain.Resource;

namespace Ada.Data.Mapping.Resource
{
   public class MediaDevelopProgressMap : EntityTypeConfiguration<MediaDevelopProgress>
    {
        public MediaDevelopProgressMap()
        {
            //配置主键
            HasKey(s => s.Id);
            //配置字段
            Property(s => s.ProgressContent).HasMaxLength(1024);
            Property(s => s.MediaDevelopId).HasMaxLength(32);

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
            ToTable("MediaDevelopProgress");
            HasRequired(s => s.MediaDevelop).WithMany(s => s.MediaDevelopProgresses).HasForeignKey(s => s.MediaDevelopId).WillCascadeOnDelete(true);
        }
    }
}
