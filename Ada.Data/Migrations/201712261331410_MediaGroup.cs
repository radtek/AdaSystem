namespace Ada.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MediaGroup : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MediaGroup",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        GroupName = c.String(maxLength: 32),
                        AddedDate = c.DateTime(),
                        AddedBy = c.String(maxLength: 32),
                        AddedById = c.String(maxLength: 32),
                        ModifiedDate = c.DateTime(),
                        ModifiedBy = c.String(maxLength: 32),
                        ModifiedById = c.String(maxLength: 32),
                        IsDelete = c.Boolean(nullable: false),
                        DeletedDate = c.DateTime(),
                        DeletedBy = c.String(maxLength: 32),
                        DeletedById = c.String(maxLength: 32),
                        IpAddress = c.String(maxLength: 32),
                        Taxis = c.Int(),
                        Remark = c.String(maxLength: 1024),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.MediaAndGroup",
                c => new
                    {
                        MediaId = c.String(nullable: false, maxLength: 128),
                        MediaGroupId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.MediaId, t.MediaGroupId })
                .ForeignKey("dbo.MediaGroup", t => t.MediaId, cascadeDelete: true)
                .ForeignKey("dbo.Media", t => t.MediaGroupId, cascadeDelete: true)
                .Index(t => t.MediaId)
                .Index(t => t.MediaGroupId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MediaAndGroup", "MediaGroupId", "dbo.Media");
            DropForeignKey("dbo.MediaAndGroup", "MediaId", "dbo.MediaGroup");
            DropIndex("dbo.MediaAndGroup", new[] { "MediaGroupId" });
            DropIndex("dbo.MediaAndGroup", new[] { "MediaId" });
            DropTable("dbo.MediaAndGroup");
            DropTable("dbo.MediaGroup");
        }
    }
}
