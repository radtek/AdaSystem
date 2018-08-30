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
    public class WorkFlowsTransitionMap : EntityTypeConfiguration<WorkFlowsTransition>
    {
        public WorkFlowsTransitionMap()
        {
            //配置主键
            HasKey(s => s.Id);
            //配置字段

            Property(s => s.SourceEndpoint).HasMaxLength(32);
            Property(s => s.DestinationEndpoint).HasMaxLength(32);
            Property(s => s.SourceActivityId).HasMaxLength(32);
            Property(s => s.DestinationActivityId).HasMaxLength(32);
            Property(s => s.WorkFlowDefinitionId).HasMaxLength(32);


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
            ToTable("WorkFlowsTransition");
            HasRequired(s => s.WorkFlowDefinition).WithMany(s => s.WorkFlowsTransitions).HasForeignKey(s => s.WorkFlowDefinitionId).WillCascadeOnDelete(true);
        }
    }
}
