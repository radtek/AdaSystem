namespace Ada.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InvoiceUpdate2018115 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.BusinessInvoice", "Address", c => c.String(maxLength: 128));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.BusinessInvoice", "Address", c => c.String(maxLength: 32));
        }
    }
}
