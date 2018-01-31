using System.Data.Entity.ModelConfiguration;
using Ada.Core.Domain.API;

namespace Ada.Data.Mapping.API
{
   public class APIRequestRecordMap : EntityTypeConfiguration<APIRequestRecord>
    {
        public APIRequestRecordMap()
        {
            //配置主键
            HasKey(s => s.Id);

            ////给ID配置自动增长
            //this.Property(s => s.ID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            //配置字段
            Property(s => s.RequestParameters).HasMaxLength(512);
            Property(s => s.Retcode).HasMaxLength(128);
            Property(s => s.Retmsg).HasMaxLength(512);
            Property(s => s.APIInterfacesId).HasMaxLength(32);

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
            ToTable("APIRequestRecord");
            //配置关系【一对多的配置，外键是FKId】 Withmany方法允许多个。HasForeignKey方法表示哪个属性是外键表的外键，WillCascadeOnDelete方法用来配置是否级联删除
            HasRequired(s => s.APIInterfaces).WithMany(s => s.APIRequestRecords).HasForeignKey(s => s.APIInterfacesId).WillCascadeOnDelete(false);

        }
    }
}
