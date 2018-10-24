namespace Ada.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class WorkFlowUpdate20181024 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.WorkFlowRecord", "Status", c => c.Short());
        }
        
        public override void Down()
        {
            DropColumn("dbo.WorkFlowRecord", "Status");
        }
    }
}
