﻿using System;
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
   public class MediaTypeMap : EntityTypeConfiguration<MediaType>
    {
        public MediaTypeMap()
        {
            //配置主键
            HasKey(s => s.Id);
            //配置字段
            Property(s => s.TypeName).IsRequired().HasMaxLength(32);
            Property(s => s.CallIndex).HasMaxLength(32);
            Property(s => s.ParentId).HasMaxLength(32);
            Property(s => s.Image).HasMaxLength(256);
            

            Property(s => s.AddedDate);
            Property(s => s.AddedBy).HasMaxLength(32);
            Property(s => s.AddedById).HasMaxLength(32);
            Property(s => s.ModifiedDate);
            Property(s => s.ModifiedBy).HasMaxLength(32);
            Property(s => s.ModifiedById).HasMaxLength(32);
            Property(s => s.IsDelete).IsRequired();
            Property(s => s.DeletedDate);
            Property(s => s.DeletedBy).HasMaxLength(32);
            Property(s => s.DeletedById).HasMaxLength(32);
            Property(s => s.IpAddress).HasMaxLength(32);
            Property(s => s.Taxis);
            Property(s => s.Remark).HasMaxLength(1024);

            //配置表
            ToTable("MediaType");
        }
    }
}
