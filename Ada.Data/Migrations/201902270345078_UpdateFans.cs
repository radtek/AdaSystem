namespace Ada.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateFans : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BusinessOrder", "IsRecommend", c => c.Boolean());
            AddColumn("dbo.BusinessOrderDetail", "IsRecommend", c => c.Boolean());
            AddColumn("dbo.Fans", "ParentId", c => c.String(maxLength: 32));
            AddColumn("dbo.Fans", "AvatarRange", c => c.String(maxLength: 32));
            AddColumn("dbo.Fans", "FansRange", c => c.String(maxLength: 32));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Fans", "FansRange");
            DropColumn("dbo.Fans", "AvatarRange");
            DropColumn("dbo.Fans", "ParentId");
            DropColumn("dbo.BusinessOrderDetail", "IsRecommend");
            DropColumn("dbo.BusinessOrder", "IsRecommend");
        }
    }
}
