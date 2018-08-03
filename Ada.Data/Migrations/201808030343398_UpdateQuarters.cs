namespace Ada.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateQuarters : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Quarters", "Post", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Quarters", "Training", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.Quarters", "Commission", c => c.Decimal(nullable: false, precision: 18, scale: 5));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Quarters", "Commission", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("dbo.Quarters", "Training");
            DropColumn("dbo.Quarters", "Post");
        }
    }
}
