namespace Ada.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MenuUpdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Menu", "IsBlank", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Menu", "IsBlank");
        }
    }
}
