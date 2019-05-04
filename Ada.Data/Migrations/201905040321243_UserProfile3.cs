namespace Ada.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserProfile3 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.BusinessInvoice", "LinkManId", "dbo.LinkMan");
            DropIndex("dbo.BusinessInvoice", new[] { "LinkManId" });
            AlterColumn("dbo.BusinessInvoice", "CompanyId", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("dbo.BusinessInvoice", "CompanyId");
            AddForeignKey("dbo.BusinessInvoice", "CompanyId", "dbo.Commpany", "Id");
            DropColumn("dbo.BusinessInvoice", "LinkManName");
            DropColumn("dbo.BusinessInvoice", "LinkManId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.BusinessInvoice", "LinkManId", c => c.String(nullable: false, maxLength: 128));
            AddColumn("dbo.BusinessInvoice", "LinkManName", c => c.String(maxLength: 32));
            DropForeignKey("dbo.BusinessInvoice", "CompanyId", "dbo.Commpany");
            DropIndex("dbo.BusinessInvoice", new[] { "CompanyId" });
            AlterColumn("dbo.BusinessInvoice", "CompanyId", c => c.String());
            CreateIndex("dbo.BusinessInvoice", "LinkManId");
            AddForeignKey("dbo.BusinessInvoice", "LinkManId", "dbo.LinkMan", "Id");
        }
    }
}
