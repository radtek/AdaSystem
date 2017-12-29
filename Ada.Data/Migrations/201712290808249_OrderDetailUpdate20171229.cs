namespace Ada.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OrderDetailUpdate20171229 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BusinessOrderDetail", "AuditStatus", c => c.Short());
        }
        
        public override void Down()
        {
            DropColumn("dbo.BusinessOrderDetail", "AuditStatus");
        }
    }
}
