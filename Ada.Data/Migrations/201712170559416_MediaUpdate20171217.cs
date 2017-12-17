namespace Ada.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MediaUpdate20171217 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Media", "Platform", c => c.String(maxLength: 32));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Media", "Platform");
        }
    }
}
