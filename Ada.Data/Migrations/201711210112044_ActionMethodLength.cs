namespace Ada.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ActionMethodLength : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Action", "ControllerName", c => c.String(maxLength: 32));
            AlterColumn("dbo.Action", "MethodName", c => c.String(maxLength: 32));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Action", "MethodName", c => c.String(maxLength: 16));
            AlterColumn("dbo.Action", "ControllerName", c => c.String(maxLength: 16));
        }
    }
}
