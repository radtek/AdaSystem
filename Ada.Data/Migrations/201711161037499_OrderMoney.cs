namespace Ada.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OrderMoney : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BusinessOrder", "TotalDiscountMoney", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.BusinessOrder", "TotalSellMoney", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.BusinessOrder", "TotalTaxMoney", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.PurchaseOrder", "Tax", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.PurchaseOrder", "TotalDiscountMoney", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.PurchaseOrder", "TotalPurchaseMoney", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.PurchaseOrder", "TotalTaxMoney", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.PurchaseOrderDetail", "TaxMoney", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.PurchaseOrderDetail", "Tax", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.BusinessOrder", "Tax", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.BusinessOrder", "DiscountRate", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.BusinessOrderDetail", "Tax", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.BusinessOrderDetail", "DiscountRate", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.PurchaseOrder", "DiscountRate", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.PurchaseOrderDetail", "DiscountRate", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.PurchaseOrderDetail", "DiscountRate", c => c.Short());
            AlterColumn("dbo.PurchaseOrder", "DiscountRate", c => c.Short());
            AlterColumn("dbo.BusinessOrderDetail", "DiscountRate", c => c.Short());
            AlterColumn("dbo.BusinessOrderDetail", "Tax", c => c.Short());
            AlterColumn("dbo.BusinessOrder", "DiscountRate", c => c.Short());
            AlterColumn("dbo.BusinessOrder", "Tax", c => c.Short());
            DropColumn("dbo.PurchaseOrderDetail", "Tax");
            DropColumn("dbo.PurchaseOrderDetail", "TaxMoney");
            DropColumn("dbo.PurchaseOrder", "TotalTaxMoney");
            DropColumn("dbo.PurchaseOrder", "TotalPurchaseMoney");
            DropColumn("dbo.PurchaseOrder", "TotalDiscountMoney");
            DropColumn("dbo.PurchaseOrder", "Tax");
            DropColumn("dbo.BusinessOrder", "TotalTaxMoney");
            DropColumn("dbo.BusinessOrder", "TotalSellMoney");
            DropColumn("dbo.BusinessOrder", "TotalDiscountMoney");
        }
    }
}
