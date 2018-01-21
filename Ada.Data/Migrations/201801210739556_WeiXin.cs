namespace Ada.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class WeiXin : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.WeiXinRequestReocrd",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        FromUserName = c.String(maxLength: 32),
                        ToUserName = c.String(maxLength: 32),
                        RequestType = c.String(maxLength: 32),
                        RequestContent = c.String(),
                        ReponseType = c.String(maxLength: 32),
                        ReponseContent = c.String(),
                        RequestTime = c.DateTime(),
                        WeiXinAccountId = c.String(nullable: false, maxLength: 128),
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
                .ForeignKey("dbo.WeiXinAccount", t => t.WeiXinAccountId)
                .Index(t => t.WeiXinAccountId);
            
            CreateTable(
                "dbo.WeiXinAccount",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(maxLength: 32),
                        SourceId = c.String(maxLength: 32),
                        AccountType = c.Short(),
                        AppId = c.String(maxLength: 32),
                        AppSecret = c.String(maxLength: 32),
                        Status = c.Boolean(),
                        Token = c.String(maxLength: 32),
                        EncodingAESKey = c.String(maxLength: 64),
                        MchId = c.String(maxLength: 32),
                        MchKey = c.String(maxLength: 64),
                        NotifyUrl = c.String(maxLength: 512),
                        CretPath = c.String(maxLength: 512),
                        Image = c.String(maxLength: 512),
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
                "dbo.WeiXinKeyword",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(maxLength: 128),
                        Keywords = c.String(),
                        IsLikeQuery = c.Boolean(),
                        IsDefault = c.Boolean(),
                        RequestType = c.Short(),
                        ResponseType = c.Short(),
                        WeiXinAccountId = c.String(nullable: false, maxLength: 128),
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
                .ForeignKey("dbo.WeiXinAccount", t => t.WeiXinAccountId)
                .Index(t => t.WeiXinAccountId);
            
            CreateTable(
                "dbo.WeiXinKeywordMatch",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Title = c.String(maxLength: 128),
                        LinkUrl = c.String(maxLength: 512),
                        ImgUrl = c.String(maxLength: 512),
                        MediaUrl = c.String(maxLength: 512),
                        MediaHDUrl = c.String(maxLength: 512),
                        Content = c.String(),
                        WeiXinKeywordId = c.String(nullable: false, maxLength: 128),
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
                .ForeignKey("dbo.WeiXinKeyword", t => t.WeiXinKeywordId)
                .Index(t => t.WeiXinKeywordId);
            
            CreateTable(
                "dbo.MediaComment",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Score = c.Short(),
                        Content = c.String(),
                        Transactor = c.String(maxLength: 32),
                        TransactorId = c.String(maxLength: 32),
                        CommentDate = c.DateTime(),
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
            
            AddColumn("dbo.Media", "Cooperation", c => c.Short());
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MediaComment", "MediaId", "dbo.Media");
            DropForeignKey("dbo.WeiXinRequestReocrd", "WeiXinAccountId", "dbo.WeiXinAccount");
            DropForeignKey("dbo.WeiXinKeywordMatch", "WeiXinKeywordId", "dbo.WeiXinKeyword");
            DropForeignKey("dbo.WeiXinKeyword", "WeiXinAccountId", "dbo.WeiXinAccount");
            DropIndex("dbo.MediaComment", new[] { "MediaId" });
            DropIndex("dbo.WeiXinKeywordMatch", new[] { "WeiXinKeywordId" });
            DropIndex("dbo.WeiXinKeyword", new[] { "WeiXinAccountId" });
            DropIndex("dbo.WeiXinRequestReocrd", new[] { "WeiXinAccountId" });
            DropColumn("dbo.Media", "Cooperation");
            DropTable("dbo.MediaComment");
            DropTable("dbo.WeiXinKeywordMatch");
            DropTable("dbo.WeiXinKeyword");
            DropTable("dbo.WeiXinAccount");
            DropTable("dbo.WeiXinRequestReocrd");
        }
    }
}
