namespace Ada.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PurchasePaymentUpdate20171222 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PurchasePayment", "Tax", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.PurchasePayment", "TaxMoney", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.PurchasePayment", "DiscountMoney", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PurchasePayment", "DiscountMoney");
            DropColumn("dbo.PurchasePayment", "TaxMoney");
            DropColumn("dbo.PurchasePayment", "Tax");
        }
    }
}
