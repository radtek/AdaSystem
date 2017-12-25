namespace Ada.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BusinessOrderUpdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BusinessOrderDetail", "Status", c => c.Short());
        }
        
        public override void Down()
        {
            DropColumn("dbo.BusinessOrderDetail", "Status");
        }
    }
}
