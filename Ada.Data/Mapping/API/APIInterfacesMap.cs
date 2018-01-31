using System.Data.Entity.ModelConfiguration;
using Ada.Core.Domain.API;

namespace Ada.Data.Mapping.API
{
    public class APIInterfacesMap : EntityTypeConfiguration<APIInterfaces>
    {
        public APIInterfacesMap()
        {
            //配置主键
            HasKey(s => s.Id);
            //配置字段
            Property(s => s.APIName).IsRequired().HasMaxLength(32);
            Property(s => s.CallIndex).HasMaxLength(32);
            Property(s => s.APIUrl).HasMaxLength(128);
            Property(s => s.HttpMethod).HasMaxLength(32);
            Property(s => s.Token).HasMaxLength(128);
            Property(s => s.AppId).HasMaxLength(128);
            Property(s => s.AppSecret).HasMaxLength(128);
            Property(s => s.Parameters).HasMaxLength(1024);

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
            ToTable("APIInterfaces");
        }
    }
}
