namespace Ada.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserProfile2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Media", "TravelArea", c => c.String());
            AddColumn("dbo.Media", "TravelRemark", c => c.String());
            AddColumn("dbo.BusinessInvoice", "CompanyId", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.BusinessInvoice", "CompanyId");
            DropColumn("dbo.Media", "TravelRemark");
            DropColumn("dbo.Media", "TravelArea");
        }
    }
}
