namespace Ada.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OrderDetailUpdate20171228 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BusinessOrderDetail", "VerificationMoney", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.BusinessOrderDetail", "ConfirmVerificationMoney", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.BusinessOrderDetail", "VerificationStatus", c => c.Short());
        }
        
        public override void Down()
        {
            DropColumn("dbo.BusinessOrderDetail", "VerificationStatus");
            DropColumn("dbo.BusinessOrderDetail", "ConfirmVerificationMoney");
            DropColumn("dbo.BusinessOrderDetail", "VerificationMoney");
        }
    }
}
