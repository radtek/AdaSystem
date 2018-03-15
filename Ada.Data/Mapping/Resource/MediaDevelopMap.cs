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
   public class MediaDevelopMap : EntityTypeConfiguration<MediaDevelop>
    {
        public MediaDevelopMap()
        {
            //配置主键
            HasKey(s => s.Id);
            //配置字段
            Property(s => s.MediaName).IsRequired().HasMaxLength(128);
            Property(s => s.MediaID).HasMaxLength(64);
            Property(s => s.Platform).HasMaxLength(32);
            Property(s => s.Content).HasMaxLength(512);
            Property(s => s.SubBy).HasMaxLength(32);
            Property(s => s.SubById).HasMaxLength(32);
            Property(s => s.Transactor).HasMaxLength(32);
            Property(s => s.TransactorId).HasMaxLength(32);
            Property(s => s.AuditBy).HasMaxLength(32);
            Property(s => s.AuditById).HasMaxLength(32);
            Property(s => s.MediaTypeId).HasMaxLength(32);

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
            ToTable("MediaDevelop");
            HasRequired(s => s.MediaType).WithMany(s => s.MediaDevelops).HasForeignKey(s => s.MediaTypeId).WillCascadeOnDelete(false);
        }
    }
}
