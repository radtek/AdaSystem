namespace Ada.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateCommpany : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.WorkFlowDefinition", "WFType", c => c.Short());
            AddColumn("dbo.Commpany", "IsCooperation", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Commpany", "IsCooperation");
            DropColumn("dbo.WorkFlowDefinition", "WFType");
        }
    }
}
