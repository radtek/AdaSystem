namespace Ada.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OrderDetailUpdate201712291718 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.BusinessOrderWriteOff", "BusinessWriteOffId", "dbo.BusinessWriteOff");
            DropForeignKey("dbo.BusinessOrderWriteOff", "BusinessOrderId", "dbo.BusinessOrder");
            DropIndex("dbo.BusinessOrderWriteOff", new[] { "BusinessWriteOffId" });
            DropIndex("dbo.BusinessOrderWriteOff", new[] { "BusinessOrderId" });
            CreateTable(
                "dbo.BusinessOrderDetailWriteOff",
                c => new
                    {
                        BusinessWriteOffId = c.String(nullable: false, maxLength: 128),
                        BusinessOrderDetailId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.BusinessWriteOffId, t.BusinessOrderDetailId })
                .ForeignKey("dbo.BusinessWriteOff", t => t.BusinessWriteOffId, cascadeDelete: true)
                .ForeignKey("dbo.BusinessOrderDetail", t => t.BusinessOrderDetailId, cascadeDelete: true)
                .Index(t => t.BusinessWriteOffId)
                .Index(t => t.BusinessOrderDetailId);
            
            DropTable("dbo.BusinessOrderWriteOff");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.BusinessOrderWriteOff",
                c => new
                    {
                        BusinessWriteOffId = c.String(nullable: false, maxLength: 128),
                        BusinessOrderId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.BusinessWriteOffId, t.BusinessOrderId });
            
            DropForeignKey("dbo.BusinessOrderDetailWriteOff", "BusinessOrderDetailId", "dbo.BusinessOrderDetail");
            DropForeignKey("dbo.BusinessOrderDetailWriteOff", "BusinessWriteOffId", "dbo.BusinessWriteOff");
            DropIndex("dbo.BusinessOrderDetailWriteOff", new[] { "BusinessOrderDetailId" });
            DropIndex("dbo.BusinessOrderDetailWriteOff", new[] { "BusinessWriteOffId" });
            DropTable("dbo.BusinessOrderDetailWriteOff");
            CreateIndex("dbo.BusinessOrderWriteOff", "BusinessOrderId");
            CreateIndex("dbo.BusinessOrderWriteOff", "BusinessWriteOffId");
            AddForeignKey("dbo.BusinessOrderWriteOff", "BusinessOrderId", "dbo.BusinessOrder", "Id", cascadeDelete: true);
            AddForeignKey("dbo.BusinessOrderWriteOff", "BusinessWriteOffId", "dbo.BusinessWriteOff", "Id", cascadeDelete: true);
        }
    }
}
