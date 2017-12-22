namespace Ada.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ManagerUpdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Manager", "Birthday", c => c.DateTime());
            AddColumn("dbo.Manager", "IsLunar", c => c.Boolean());
            AddColumn("dbo.Manager", "IdCard", c => c.String(maxLength: 32));
            AddColumn("dbo.Manager", "EntryDate", c => c.DateTime());
            AddColumn("dbo.Manager", "QuitDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Manager", "QuitDate");
            DropColumn("dbo.Manager", "EntryDate");
            DropColumn("dbo.Manager", "IdCard");
            DropColumn("dbo.Manager", "IsLunar");
            DropColumn("dbo.Manager", "Birthday");
        }
    }
}
