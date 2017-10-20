using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core.Domain.Admin;
using Ada.Core.Domain.Log;

namespace Ada.Data.Mapping.Log
{
   public class SystemLogMap : EntityTypeConfiguration<SystemLog>
    {
        public SystemLogMap()
        {
            //配置主键
            HasKey(s => s.Id);

            ////给ID配置自动增长
            this.Property(s => s.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            //配置字段
            Property(s => s.Level).HasMaxLength(32);
            Property(s => s.Thread).HasMaxLength(32);
            Property(s => s.Date);
            Property(s => s.Logger).HasMaxLength(128);
            Property(s => s.Message);
            Property(s => s.Exception);
            

            //配置表
            ToTable("SystemLog");

            
        }
    }
}
