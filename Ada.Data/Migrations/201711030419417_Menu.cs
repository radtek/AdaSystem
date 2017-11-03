namespace Ada.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Menu : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Menu", "ActionId", c => c.String(maxLength: 128));
            AlterColumn("dbo.Menu", "TreePath", c => c.String(maxLength: 1024));
            DropColumn("dbo.Menu", "ActionInfoId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Menu", "ActionInfoId", c => c.String(maxLength: 128));
            AlterColumn("dbo.Menu", "TreePath", c => c.String(maxLength: 32));
            DropColumn("dbo.Menu", "ActionId");
        }
    }
}
