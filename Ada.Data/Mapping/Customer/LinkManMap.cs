using System.Data.Entity.ModelConfiguration;
using Ada.Core.Domain.Customer;

namespace Ada.Data.Mapping.Customer
{
   public class LinkManMap : EntityTypeConfiguration<LinkMan>
    {
        public LinkManMap()
        {
            //配置主键
            HasKey(s => s.Id);
            //配置字段
            Property(s => s.Name).IsRequired().HasMaxLength(32);
            Property(s => s.WorkName).HasMaxLength(32);
            Property(s => s.LinkManType).HasMaxLength(32);
            Property(s => s.Phone).HasMaxLength(64);
            Property(s => s.QQ).HasMaxLength(32);
            Property(s => s.WeiXin).HasMaxLength(32);
            Property(s => s.Address).HasMaxLength(128);
            Property(s => s.Sex).HasMaxLength(16);
            Property(s => s.Status).HasMaxLength(32);
            Property(s => s.Image).HasMaxLength(128);
            Property(s => s.OpenId).HasMaxLength(64);
            Property(s => s.Password).HasMaxLength(32);
            Property(s => s.LoginName).HasMaxLength(32);
            Property(s => s.CommpanyId).HasMaxLength(32);
           

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
            ToTable("LinkMan");
            //配置关系【一对多的配置，外键是UserId】 Withmany方法允许多个。HasForeignKey方法表示哪个属性是User表的外键，WillCascadeOnDelete方法用来配置是否级联删除
            HasRequired(s => s.Commpany).WithMany(s => s.LinkMans).HasForeignKey(s => s.CommpanyId).WillCascadeOnDelete(true);
        }
    }
}
