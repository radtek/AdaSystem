namespace Ada.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class API : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MediaArticle",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        ArticleId = c.String(maxLength: 128),
                        Title = c.String(maxLength: 512),
                        Content = c.String(),
                        PublishDate = c.DateTime(),
                        OriginUrl = c.String(maxLength: 512),
                        ArticleUrl = c.String(maxLength: 512),
                        IsOriginal = c.Boolean(),
                        IsTop = c.Boolean(),
                        ArticleIdx = c.String(maxLength: 32),
                        Biz = c.String(maxLength: 64),
                        ViewCount = c.Int(),
                        CommentCount = c.Int(),
                        LikeCount = c.Int(),
                        ShareCount = c.Int(),
                        MediaId = c.String(nullable: false, maxLength: 128),
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
                .ForeignKey("dbo.Media", t => t.MediaId)
                .Index(t => t.MediaId);
            
            CreateTable(
                "dbo.APIInterfaces",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        APIType = c.Short(),
                        APIName = c.String(nullable: false, maxLength: 32),
                        CallIndex = c.String(maxLength: 32),
                        APIUrl = c.String(maxLength: 128),
                        HttpMethod = c.String(maxLength: 32),
                        Token = c.String(maxLength: 128),
                        AppId = c.String(maxLength: 128),
                        AppSecret = c.String(maxLength: 128),
                        Parameters = c.String(maxLength: 1024),
                        TimeOut = c.Int(),
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
                "dbo.APIRequestRecord",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        RequestParameters = c.String(maxLength: 512),
                        RequestDate = c.DateTime(),
                        ReponseDate = c.DateTime(),
                        ReponseContent = c.String(),
                        Retcode = c.String(maxLength: 128),
                        Retmsg = c.String(maxLength: 512),
                        IsSuccess = c.Boolean(),
                        APIInterfacesId = c.String(nullable: false, maxLength: 128),
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
                .ForeignKey("dbo.APIInterfaces", t => t.APIInterfacesId)
                .Index(t => t.APIInterfacesId);
            
            AddColumn("dbo.Media", "PostNum", c => c.Int());
            AddColumn("dbo.Media", "MonthPostNum", c => c.Int());
            AddColumn("dbo.Media", "FriendNum", c => c.Int());
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.APIRequestRecord", "APIInterfacesId", "dbo.APIInterfaces");
            DropForeignKey("dbo.MediaArticle", "MediaId", "dbo.Media");
            DropIndex("dbo.APIRequestRecord", new[] { "APIInterfacesId" });
            DropIndex("dbo.MediaArticle", new[] { "MediaId" });
            DropColumn("dbo.Media", "FriendNum");
            DropColumn("dbo.Media", "MonthPostNum");
            DropColumn("dbo.Media", "PostNum");
            DropTable("dbo.APIRequestRecord");
            DropTable("dbo.APIInterfaces");
            DropTable("dbo.MediaArticle");
        }
    }
}
