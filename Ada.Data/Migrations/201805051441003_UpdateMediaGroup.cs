namespace Ada.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateMediaGroup : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MediaGroup", "GroupType", c => c.Short());
        }
        
        public override void Down()
        {
            DropColumn("dbo.MediaGroup", "GroupType");
        }
    }
}
