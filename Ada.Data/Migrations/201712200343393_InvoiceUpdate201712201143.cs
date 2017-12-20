namespace Ada.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InvoiceUpdate201712201143 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BusinessInvoice", "InvoiceTime", c => c.DateTime());
            AddColumn("dbo.BusinessInvoice", "PayTime", c => c.DateTime());
            AddColumn("dbo.BusinessInvoice", "ReceivableNum", c => c.String(maxLength: 32));
        }
        
        public override void Down()
        {
            DropColumn("dbo.BusinessInvoice", "ReceivableNum");
            DropColumn("dbo.BusinessInvoice", "PayTime");
            DropColumn("dbo.BusinessInvoice", "InvoiceTime");
        }
    }
}
