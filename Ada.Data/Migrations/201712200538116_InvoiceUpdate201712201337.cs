namespace Ada.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InvoiceUpdate201712201337 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BusinessInvoice", "InvoiceNum", c => c.String(maxLength: 32));
        }
        
        public override void Down()
        {
            DropColumn("dbo.BusinessInvoice", "InvoiceNum");
        }
    }
}
