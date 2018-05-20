namespace Ada.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class WeiXinAndMediaUpdate2 : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.WeiXinOpenWebAppMap", newName: "WeiXinOpenWebApp");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.WeiXinOpenWebApp", newName: "WeiXinOpenWebAppMap");
        }
    }
}
