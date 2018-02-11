namespace Ada.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserMediaFilet20180211 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Media", "CollectionGroup", c => c.String(maxLength: 32));
            AddColumn("dbo.Media", "CollectionDate", c => c.DateTime());
            AlterColumn("dbo.MediaArticle", "OriginUrl", c => c.String(maxLength: 2048));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.MediaArticle", "OriginUrl", c => c.String(maxLength: 512));
            DropColumn("dbo.Media", "CollectionDate");
            DropColumn("dbo.Media", "CollectionGroup");
        }
    }
}
