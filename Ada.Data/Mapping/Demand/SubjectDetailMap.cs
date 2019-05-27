using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core.Domain.Demand;

namespace Ada.Data.Mapping.Demand
{
  public  class SubjectDetailMap : EntityTypeConfiguration<SubjectDetail>
    {
        public SubjectDetailMap()
        {
            //配置主键
            HasKey(s => s.Id);
            //配置字段
            Property(s => s.Title).HasMaxLength(32);
            Property(s => s.Type).HasMaxLength(32);
            Property(s => s.Transactor).HasMaxLength(32);
            Property(s => s.TransactorId).HasMaxLength(32);
            Property(s => s.ProducerBy).HasMaxLength(32);
            Property(s => s.ProducerById).HasMaxLength(32);
            Property(s => s.SubjectId).HasMaxLength(32);

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
            ToTable("SubjectDetail");
            HasRequired(s => s.Subject).WithMany(s => s.SubjectDetails).HasForeignKey(s => s.SubjectId).WillCascadeOnDelete(true);

        }
    }
}
