﻿using System.Data.Entity.ModelConfiguration;
using Ada.Core.Domain.Customer;

namespace Ada.Data.Mapping.Customer
{
   public class FollowUpMap : EntityTypeConfiguration<FollowUp>
    {
        public FollowUpMap()
        {
            //配置主键
            HasKey(s => s.Id);
            //配置字段
            Property(s => s.FollowUpWay).HasMaxLength(32);
            Property(s => s.Transactor).HasMaxLength(32);
            Property(s => s.TransactorId).HasMaxLength(32);
            Property(s => s.LinkManId).HasMaxLength(32);
            

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
            ToTable("FollowUp");
            //配置关系【一对多的配置，外键是UserId】 Withmany方法允许多个。HasForeignKey方法表示哪个属性是User表的外键，WillCascadeOnDelete方法用来配置是否级联删除
            HasRequired(s => s.LinkMan).WithMany(s => s.FollowUps).HasForeignKey(s => s.LinkManId).WillCascadeOnDelete(false);
        }
    }
}
