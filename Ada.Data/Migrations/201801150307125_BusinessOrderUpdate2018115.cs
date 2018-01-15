namespace Ada.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BusinessOrderUpdate2018115 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BusinessOrderDetail", "RequestSellMoney", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.BusinessOrderDetail", "AuditRemark", c => c.String(maxLength: 1024));
            AddColumn("dbo.BusinessOrderDetail", "AuditDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.BusinessOrderDetail", "AuditDate");
            DropColumn("dbo.BusinessOrderDetail", "AuditRemark");
            DropColumn("dbo.BusinessOrderDetail", "RequestSellMoney");
        }
    }
}
