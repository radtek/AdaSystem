namespace Ada.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LinMan : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.LinkMan", "IsLock", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.LinkMan", "IsLock");
        }
    }
}
