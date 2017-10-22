namespace Ada.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ManagerLoginLog : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ManagerLoginLog",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        LoginTime = c.DateTime(),
                        WebInfo = c.String(maxLength: 512),
                        ManagerId = c.String(nullable: false, maxLength: 128),
                        AddedDate = c.DateTime(),
                        AddedBy = c.String(maxLength: 32),
                        ModifiedDate = c.DateTime(),
                        ModifiedBy = c.String(maxLength: 32),
                        IsDelete = c.Boolean(nullable: false),
                        DeletedDate = c.DateTime(),
                        DeletedBy = c.String(maxLength: 32),
                        IpAddress = c.String(maxLength: 32),
                        Taxis = c.Int(),
                        Remark = c.String(maxLength: 1024),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Manager", t => t.ManagerId, cascadeDelete: true)
                .Index(t => t.ManagerId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ManagerLoginLog", "ManagerId", "dbo.Manager");
            DropIndex("dbo.ManagerLoginLog", new[] { "ManagerId" });
            DropTable("dbo.ManagerLoginLog");
        }
    }
}
