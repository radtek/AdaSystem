namespace Ada.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InvoiceUpdate20171220 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BusinessInvoice", "LinkManName", c => c.String(maxLength: 32));
            AddColumn("dbo.BusinessInvoice", "LinkManId", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("dbo.BusinessInvoice", "LinkManId");
            AddForeignKey("dbo.BusinessInvoice", "LinkManId", "dbo.LinkMan", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.BusinessInvoice", "LinkManId", "dbo.LinkMan");
            DropIndex("dbo.BusinessInvoice", new[] { "LinkManId" });
            DropColumn("dbo.BusinessInvoice", "LinkManId");
            DropColumn("dbo.BusinessInvoice", "LinkManName");
        }
    }
}
