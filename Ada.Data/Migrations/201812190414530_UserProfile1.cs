namespace Ada.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserProfile1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Manager", "IsInsurance", c => c.Boolean());
            AddColumn("dbo.SalaryDetail", "Endowment", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.SalaryDetail", "Health", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.SalaryDetail", "Injury", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.SalaryDetail", "Childbirth", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.SalaryDetail", "Unemployment", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.SalaryDetail", "HousingFund", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.SalaryDetail", "Tax", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SalaryDetail", "Tax");
            DropColumn("dbo.SalaryDetail", "HousingFund");
            DropColumn("dbo.SalaryDetail", "Unemployment");
            DropColumn("dbo.SalaryDetail", "Childbirth");
            DropColumn("dbo.SalaryDetail", "Injury");
            DropColumn("dbo.SalaryDetail", "Health");
            DropColumn("dbo.SalaryDetail", "Endowment");
            DropColumn("dbo.Manager", "IsInsurance");
        }
    }
}
