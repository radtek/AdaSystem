namespace Ada.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dingjin : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PurchaseOrderDetail", "BargainMoney", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.PurchaseOrder", "TotalBargainMoney", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PurchaseOrder", "TotalBargainMoney");
            DropColumn("dbo.PurchaseOrderDetail", "BargainMoney");
        }
    }
}
