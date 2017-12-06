namespace Ada.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SettleAccount : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SettleAccount", "Tax", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SettleAccount", "Tax");
        }
    }
}
