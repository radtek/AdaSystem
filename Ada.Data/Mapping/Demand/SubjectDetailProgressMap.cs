using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core.Domain.Demand;

namespace Ada.Data.Mapping.Demand
{
  public  class SubjectDetailProgressMap : EntityTypeConfiguration<SubjectDetailProgress>
    {
        public SubjectDetailProgressMap()
        {
            //配置主键
            HasKey(s => s.Id);
            //配置字段
            Property(s => s.SubjectDetailId).HasMaxLength(32);


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
            ToTable("SubjectDetailProgress");
            HasRequired(s => s.SubjectDetail).WithMany(s => s.SubjectDetailProgresses).HasForeignKey(s => s.SubjectDetailId).WillCascadeOnDelete(true);
            HasMany(s => s.Attachments).
                WithMany(s => s.SubjectDetailProgresses)
                .Map(s => s.ToTable("SubjectDetailProgressAttachment").
                    MapLeftKey("SubjectDetailProgressId").
                    MapRightKey("AttachmentId"));
        }
    }
}
