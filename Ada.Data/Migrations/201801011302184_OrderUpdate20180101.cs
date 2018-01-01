namespace Ada.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OrderUpdate20180101 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.BusinessOrder", "TotalMoney", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.PurchaseOrder", "TotalMoney", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.PurchaseOrder", "TotalMoney", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.BusinessOrder", "TotalMoney", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
    }
}
