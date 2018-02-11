namespace Ada.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserMediaFilet : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Media", "Abstract", c => c.String(maxLength: 1024));
            AddColumn("dbo.Media", "IsLoop", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Media", "IsLoop");
            DropColumn("dbo.Media", "Abstract");
        }
    }
}
