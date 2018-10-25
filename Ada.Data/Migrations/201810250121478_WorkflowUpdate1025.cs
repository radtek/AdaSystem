namespace Ada.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class WorkflowUpdate1025 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.WorkFlowRecord", "WfInstanceId", c => c.String(maxLength: 128));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.WorkFlowRecord", "WfInstanceId", c => c.String(maxLength: 32));
        }
    }
}
