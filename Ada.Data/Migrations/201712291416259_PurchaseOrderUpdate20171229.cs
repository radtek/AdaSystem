namespace Ada.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PurchaseOrderUpdate20171229 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BusinessInvoiceDetail", "AuditStatus", c => c.Short());
            AddColumn("dbo.BusinessInvoice", "AuditBy", c => c.String(maxLength: 32));
            AddColumn("dbo.BusinessInvoice", "AuditById", c => c.String(maxLength: 32));
            AddColumn("dbo.BusinessInvoice", "AuditDate", c => c.DateTime());
            AddColumn("dbo.BusinessInvoice", "AuditStatus", c => c.Short());
            AddColumn("dbo.PurchasePaymentOrderDetail", "AuditStatus", c => c.Short());
            AddColumn("dbo.PurchasePayment", "AuditBy", c => c.String(maxLength: 32));
            AddColumn("dbo.PurchasePayment", "AuditById", c => c.String(maxLength: 32));
            AddColumn("dbo.PurchasePayment", "AuditDate", c => c.DateTime());
            AddColumn("dbo.PurchasePayment", "AuditStatus", c => c.Short());
        }
        
        public override void Down()
        {
            DropColumn("dbo.PurchasePayment", "AuditStatus");
            DropColumn("dbo.PurchasePayment", "AuditDate");
            DropColumn("dbo.PurchasePayment", "AuditById");
            DropColumn("dbo.PurchasePayment", "AuditBy");
            DropColumn("dbo.PurchasePaymentOrderDetail", "AuditStatus");
            DropColumn("dbo.BusinessInvoice", "AuditStatus");
            DropColumn("dbo.BusinessInvoice", "AuditDate");
            DropColumn("dbo.BusinessInvoice", "AuditById");
            DropColumn("dbo.BusinessInvoice", "AuditBy");
            DropColumn("dbo.BusinessInvoiceDetail", "AuditStatus");
        }
    }
}
