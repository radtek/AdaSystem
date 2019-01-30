namespace Ada.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class VoteUpdate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.VoteItem",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Title = c.String(nullable: false, maxLength: 128),
                        Url = c.String(maxLength: 512),
                        Abstract = c.String(maxLength: 128),
                        Image = c.String(maxLength: 512),
                        IsTop = c.Boolean(nullable: false),
                        Content = c.String(),
                        Click = c.Int(),
                        Status = c.Boolean(nullable: false),
                        Total = c.Int(nullable: false),
                        VoteThemeId = c.String(nullable: false, maxLength: 128),
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
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.VoteTheme", t => t.VoteThemeId, cascadeDelete: true)
                .Index(t => t.VoteThemeId);
            
            CreateTable(
                "dbo.VoteItemRecord",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Date = c.DateTime(nullable: false),
                        Name = c.String(nullable: false, maxLength: 128),
                        UID = c.String(maxLength: 32),
                        Phone = c.String(maxLength: 32),
                        Image = c.String(maxLength: 512),
                        OpenId = c.String(maxLength: 64),
                        Cookies = c.String(),
                        Score = c.Int(nullable: false),
                        VoteItemId = c.String(nullable: false, maxLength: 128),
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
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.VoteItem", t => t.VoteItemId, cascadeDelete: true)
                .Index(t => t.VoteItemId);
            
            CreateTable(
                "dbo.VoteTheme",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Title = c.String(nullable: false, maxLength: 128),
                        CallIndex = c.String(maxLength: 32),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        KeyWord = c.String(maxLength: 32),
                        CoverStart = c.String(maxLength: 512),
                        CoverEnd = c.String(maxLength: 512),
                        Abstract = c.String(maxLength: 128),
                        Rule = c.String(maxLength: 1024),
                        Content = c.String(),
                        EndTitle = c.String(maxLength: 128),
                        EndContent = c.String(),
                        Click = c.Int(),
                        Status = c.Boolean(nullable: false),
                        Config = c.String(),
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
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.VoteItem", "VoteThemeId", "dbo.VoteTheme");
            DropForeignKey("dbo.VoteItemRecord", "VoteItemId", "dbo.VoteItem");
            DropIndex("dbo.VoteItemRecord", new[] { "VoteItemId" });
            DropIndex("dbo.VoteItem", new[] { "VoteThemeId" });
            DropTable("dbo.VoteTheme");
            DropTable("dbo.VoteItemRecord");
            DropTable("dbo.VoteItem");
        }
    }
}
