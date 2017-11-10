namespace Ada.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MediaChannelType : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Media", "ChannelType", c => c.String(maxLength: 32));
            AlterColumn("dbo.Media", "Content", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Media", "Content", c => c.String(unicode: false, storeType: "text"));
            DropColumn("dbo.Media", "ChannelType");
        }
    }
}
