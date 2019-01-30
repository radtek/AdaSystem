namespace Ada.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class VoteUpdate1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.VoteItem", "TotalCount", c => c.Int(nullable: false));
            DropColumn("dbo.VoteItem", "Total");
        }
        
        public override void Down()
        {
            AddColumn("dbo.VoteItem", "Total", c => c.Int(nullable: false));
            DropColumn("dbo.VoteItem", "TotalCount");
        }
    }
}
