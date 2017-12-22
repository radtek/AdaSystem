namespace Ada.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PurchasePaymentUpdate20171222929 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PurchasePayment", "InvoiceStauts", c => c.Boolean());
            AddColumn("dbo.PurchasePayment", "IsInvoice", c => c.Boolean());
            AddColumn("dbo.PurchasePayment", "InvoiceDate", c => c.DateTime());
            AddColumn("dbo.PurchasePayment", "InvoiceNum", c => c.String(maxLength: 32));
            AddColumn("dbo.PurchasePayment", "InvoiceTitle", c => c.String(maxLength: 32));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PurchasePayment", "InvoiceTitle");
            DropColumn("dbo.PurchasePayment", "InvoiceNum");
            DropColumn("dbo.PurchasePayment", "InvoiceDate");
            DropColumn("dbo.PurchasePayment", "IsInvoice");
            DropColumn("dbo.PurchasePayment", "InvoiceStauts");
        }
    }
}
