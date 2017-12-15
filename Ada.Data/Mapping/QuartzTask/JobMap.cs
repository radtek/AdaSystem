using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core.Domain.QuartzTask;

namespace Ada.Data.Mapping.QuartzTask
{
  public  class JobMap: EntityTypeConfiguration<Job>
    {
        public JobMap()
        {
            //配置主键
            HasKey(s => s.Id);


            Property(s => s.GroupName).HasMaxLength(128);
            Property(s => s.JobName).HasMaxLength(32);
            Property(s => s.JobType).HasMaxLength(32);
            Property(s => s.TriggerName).HasMaxLength(32);
            Property(s => s.Cron).HasMaxLength(32);
            Property(s => s.TriggerState).HasMaxLength(32);
            Property(s => s.AppId).HasMaxLength(64);
            Property(s => s.ApiUrl).HasMaxLength(512);
            Property(s => s.Params).HasMaxLength(512);
            Property(s => s.Token).HasMaxLength(64);


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
            ToTable("Job");
        }
    }
}
