namespace Ada.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Business : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.BusinessInvoice", "LinkManId", "dbo.LinkMan");
            DropIndex("dbo.BusinessInvoice", new[] { "LinkManId" });
            AddColumn("dbo.Media", "TravelArea", c => c.String(maxLength: 32));
            AddColumn("dbo.Media", "TravelRemark", c => c.String(maxLength: 512));
            AddColumn("dbo.BusinessInvoice", "CommpanyId", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("dbo.BusinessInvoice", "CommpanyId");
            AddForeignKey("dbo.BusinessInvoice", "CommpanyId", "dbo.Commpany", "Id");
            DropColumn("dbo.BusinessInvoice", "LinkManName");
            DropColumn("dbo.BusinessInvoice", "LinkManId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.BusinessInvoice", "LinkManId", c => c.String(nullable: false, maxLength: 128));
            AddColumn("dbo.BusinessInvoice", "LinkManName", c => c.String(maxLength: 32));
            DropForeignKey("dbo.BusinessInvoice", "CommpanyId", "dbo.Commpany");
            DropIndex("dbo.BusinessInvoice", new[] { "CommpanyId" });
            DropColumn("dbo.BusinessInvoice", "CommpanyId");
            DropColumn("dbo.Media", "TravelRemark");
            DropColumn("dbo.Media", "TravelArea");
            CreateIndex("dbo.BusinessInvoice", "LinkManId");
            AddForeignKey("dbo.BusinessInvoice", "LinkManId", "dbo.LinkMan", "Id");
        }
    }
}
