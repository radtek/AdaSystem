﻿using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core.Domain.Business;

namespace Ada.Data.Mapping.Business
{
  public  class OfferRatioDetailMap : EntityTypeConfiguration<OfferRatioDetail>
    {
        public OfferRatioDetailMap()
        {
            //配置主键
            HasKey(s => s.Id);

            ////给ID配置自动增长
            //this.Property(s => s.ID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            //配置字段
            Property(s => s.OfferRatioId).HasMaxLength(32);


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
            ToTable("OfferRatioDetail");
            //配置关系【一对多的配置，外键是FKId】 Withmany方法允许多个。HasForeignKey方法表示哪个属性是外键表的外键，WillCascadeOnDelete方法用来配置是否级联删除
            HasRequired(s => s.OfferRatio).WithMany(s => s.OfferRatioDetails).HasForeignKey(s => s.OfferRatioId).WillCascadeOnDelete(true);
        }
    }
}
