namespace Ada.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OrderAdd : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BusinessOrder", "OrderDate", c => c.DateTime());
            AddColumn("dbo.PurchaseOrder", "OrderDate", c => c.DateTime());
            AddColumn("dbo.PurchaseReturnOrder", "ReturnDate", c => c.DateTime());
            AddColumn("dbo.BusinessReturnOrder", "ReturnDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.BusinessReturnOrder", "ReturnDate");
            DropColumn("dbo.PurchaseReturnOrder", "ReturnDate");
            DropColumn("dbo.PurchaseOrder", "OrderDate");
            DropColumn("dbo.BusinessOrder", "OrderDate");
        }
    }
}
