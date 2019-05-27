using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core.Domain.Demand;
using Ada.Core.Domain.Resource;

namespace Ada.Data.Mapping.Resource
{
  public  class MediaReferencePriceMap : EntityTypeConfiguration<MediaReferencePrice>
    {
        public MediaReferencePriceMap()
        {
            //配置主键
            HasKey(s => s.Id);
            //配置字段
            Property(s => s.Platform).HasMaxLength(32);
            Property(s => s.PriceName).HasMaxLength(32);
            Property(s => s.MediaId).HasMaxLength(32);
           

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
            ToTable("MediaReferencePrice");
            HasRequired(s => s.Media).WithMany(s => s.MediaReferencePrices).HasForeignKey(s => s.MediaId).WillCascadeOnDelete(true);

        }
    }
}
