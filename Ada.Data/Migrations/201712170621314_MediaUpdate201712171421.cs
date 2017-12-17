namespace Ada.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MediaUpdate201712171421 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Media", "Sex", c => c.String(maxLength: 16));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Media", "Sex");
        }
    }
}
