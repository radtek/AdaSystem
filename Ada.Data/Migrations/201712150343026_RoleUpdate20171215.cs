namespace Ada.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RoleUpdate20171215 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Role", "RoleGrade", c => c.Short());
            AddColumn("dbo.Role", "DataRange", c => c.Short());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Role", "DataRange");
            DropColumn("dbo.Role", "RoleGrade");
        }
    }
}
