namespace Ada.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Update20180606 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.PurchaseReturenOrderDetail", "PurchaseReturnOrderId", "dbo.PurchaseReturnOrder");
            AddColumn("dbo.Media", "RetentionTime", c => c.String(maxLength: 128));
            AlterColumn("dbo.PurchaseReturnOrder", "PurchaseOrderId", c => c.String(maxLength: 32));
            AddForeignKey("dbo.PurchaseReturenOrderDetail", "PurchaseReturnOrderId", "dbo.PurchaseReturnOrder", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PurchaseReturenOrderDetail", "PurchaseReturnOrderId", "dbo.PurchaseReturnOrder");
            AlterColumn("dbo.PurchaseReturnOrder", "PurchaseOrderId", c => c.String(nullable: false, maxLength: 32));
            DropColumn("dbo.Media", "RetentionTime");
            AddForeignKey("dbo.PurchaseReturenOrderDetail", "PurchaseReturnOrderId", "dbo.PurchaseReturnOrder", "Id");
        }
    }
}
