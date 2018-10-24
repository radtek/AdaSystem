using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core.Domain.WeiXin;
using Ada.Core.Domain.WorkFlow;

namespace Ada.Data.Mapping.WorkFlow
{
    public class WorkFlowRecordDetailMap : EntityTypeConfiguration<WorkFlowRecordDetail>
    {
        public WorkFlowRecordDetailMap()
        {
            //配置主键
            HasKey(s => s.Id);
            //配置字段

            Property(s => s.Name).HasMaxLength(128);
            Property(s => s.ProcessBy).HasMaxLength(32);
            Property(s => s.ProcessById).HasMaxLength(32);
            Property(s => s.ProcessResult).HasMaxLength(256);
            Property(s => s.ProcessComment).HasMaxLength(512);
            Property(s => s.ParentDetailId).HasMaxLength(32);
            Property(s => s.WorkFlowRecordId).HasMaxLength(32);


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
            ToTable("WorkFlowRecordDetail");
            HasRequired(s => s.WorkFlowRecord).WithMany(s => s.WorkFlowRecordDetails).HasForeignKey(s => s.WorkFlowRecordId).WillCascadeOnDelete(true);
        }
    }
}
