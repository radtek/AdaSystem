﻿using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core.Domain.Vote;

namespace Ada.Data.Mapping.Vote
{
   public class VoteThemeMap: EntityTypeConfiguration<VoteTheme>
    {
        public VoteThemeMap()
        {
            //配置主键
            HasKey(s => s.Id);
            //配置字段
            Property(s => s.Title).IsRequired().HasMaxLength(128);
            Property(s => s.CallIndex).HasMaxLength(32);
            Property(s => s.KeyWord).HasMaxLength(32);
            Property(s => s.CoverStart).HasMaxLength(512);
            Property(s => s.CoverEnd).HasMaxLength(512);
            Property(s => s.Abstract).HasMaxLength(128);
            Property(s => s.Rule).HasMaxLength(1024);
            Property(s => s.EndTitle).HasMaxLength(128);

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
            ToTable("VoteTheme");
        }
    }
}
