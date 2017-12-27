namespace Ada.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MediaUpdate20171227 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Media", "Client", c => c.String(maxLength: 32));
            AddColumn("dbo.Media", "SEO", c => c.String(maxLength: 32));
            AddColumn("dbo.Media", "Efficiency", c => c.String(maxLength: 32));
            AddColumn("dbo.Media", "ResourceType", c => c.String(maxLength: 32));
            AddColumn("dbo.Media", "Channel", c => c.String(maxLength: 32));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Media", "Channel");
            DropColumn("dbo.Media", "ResourceType");
            DropColumn("dbo.Media", "Efficiency");
            DropColumn("dbo.Media", "SEO");
            DropColumn("dbo.Media", "Client");
        }
    }
}
