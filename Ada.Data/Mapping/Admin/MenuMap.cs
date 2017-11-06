﻿using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core.Domain.Admin;

namespace Ada.Data.Mapping.Admin
{
   public class MenuMap : EntityTypeConfiguration<Menu>
    {
        public MenuMap()
        {
            //配置主键
            HasKey(s => s.Id);

            ////给ID配置自动增长
            //this.Property(s => s.ID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            //配置字段
            Property(s => s.MenuName).IsRequired().HasMaxLength(32);
            Property(s => s.TreePath).HasMaxLength(1024);
            Property(s => s.ParentId).HasMaxLength(128);
            Property(s => s.IsLeaf);
            Property(s => s.Level);
            Property(s => s.IsVisable);
            Property(s => s.Image).HasMaxLength(512);
            Property(s => s.IconCls).HasMaxLength(32);
            Property(s => s.ActionId).HasMaxLength(128);

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
            ToTable("Menu");

            
        }
    }
}
