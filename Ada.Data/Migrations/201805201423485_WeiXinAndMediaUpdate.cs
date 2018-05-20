namespace Ada.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class WeiXinAndMediaUpdate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.WeiXinOpenWebAppMap",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(maxLength: 32),
                        AppId = c.String(maxLength: 32),
                        AppSecret = c.String(maxLength: 32),
                        LoginCallBackUrl = c.String(maxLength: 512),
                        HomeUrl = c.String(maxLength: 512),
                        BindAccountUrl = c.String(maxLength: 512),
                        WeiXinAccountId = c.String(maxLength: 32),
                        AddedDate = c.DateTime(),
                        AddedBy = c.String(maxLength: 32),
                        AddedById = c.String(maxLength: 32),
                        ModifiedDate = c.DateTime(),
                        ModifiedBy = c.String(maxLength: 32),
                        ModifiedById = c.String(maxLength: 32),
                        IsDelete = c.Boolean(nullable: false),
                        DeletedDate = c.DateTime(),
                        DeletedBy = c.String(maxLength: 32),
                        DeletedById = c.String(maxLength: 32),
                        IpAddress = c.String(maxLength: 32),
                        Taxis = c.Int(),
                        Remark = c.String(maxLength: 1024),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.MediaAppointment",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Transactor = c.String(maxLength: 32),
                        TransactorId = c.String(maxLength: 32),
                        State = c.Short(),
                        AppointmentDate = c.DateTime(),
                        MediaId = c.String(nullable: false, maxLength: 128),
                        AddedDate = c.DateTime(),
                        AddedBy = c.String(maxLength: 32),
                        AddedById = c.String(maxLength: 32),
                        ModifiedDate = c.DateTime(),
                        ModifiedBy = c.String(maxLength: 32),
                        ModifiedById = c.String(maxLength: 32),
                        IsDelete = c.Boolean(nullable: false),
                        DeletedDate = c.DateTime(),
                        DeletedBy = c.String(maxLength: 32),
                        DeletedById = c.String(maxLength: 32),
                        IpAddress = c.String(maxLength: 32),
                        Taxis = c.Int(),
                        Remark = c.String(maxLength: 1024),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Media", t => t.MediaId, cascadeDelete: true)
                .Index(t => t.MediaId);
            
            AddColumn("dbo.WeiXinAccount", "WeiXinOpenWebAppId", c => c.String(maxLength: 32));
            AddColumn("dbo.LinkMan", "UnionId", c => c.String(maxLength: 64));
            AddColumn("dbo.Manager", "UnionId", c => c.String(maxLength: 64));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MediaAppointment", "MediaId", "dbo.Media");
            DropIndex("dbo.MediaAppointment", new[] { "MediaId" });
            DropColumn("dbo.Manager", "UnionId");
            DropColumn("dbo.LinkMan", "UnionId");
            DropColumn("dbo.WeiXinAccount", "WeiXinOpenWebAppId");
            DropTable("dbo.MediaAppointment");
            DropTable("dbo.WeiXinOpenWebAppMap");
        }
    }
}
