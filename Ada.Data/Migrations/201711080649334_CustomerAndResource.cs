namespace Ada.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CustomerAndResource : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MediaPrice",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        PurchasePrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MarketPrice = c.Decimal(precision: 18, scale: 2),
                        SellPrice = c.Decimal(precision: 18, scale: 2),
                        PriceDate = c.DateTime(),
                        InvalidDate = c.DateTime(),
                        MediaId = c.String(nullable: false, maxLength: 128),
                        AdPositionId = c.String(maxLength: 32),
                        AdPositionName = c.String(maxLength: 32),
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
                .ForeignKey("dbo.Media", t => t.MediaId, cascadeDelete: true)
                .Index(t => t.MediaId);
            
            CreateTable(
                "dbo.Media",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        MediaName = c.String(nullable: false, maxLength: 128),
                        MediaID = c.String(maxLength: 128),
                        MediaLink = c.String(maxLength: 512),
                        MediaLogo = c.String(maxLength: 512),
                        MediaQR = c.String(maxLength: 512),
                        IsAuthenticate = c.Boolean(),
                        IsOriginal = c.Boolean(),
                        IsComment = c.Boolean(),
                        FansNum = c.Int(),
                        LastReadNum = c.Int(),
                        AvgReadNum = c.Int(),
                        PublishFrequency = c.Int(),
                        Area = c.String(maxLength: 32),
                        LastPushDate = c.DateTime(),
                        AuthenticateType = c.String(maxLength: 32),
                        TransmitNum = c.Int(),
                        CommentNum = c.Int(),
                        LikesNum = c.Int(),
                        Content = c.String(unicode: false, storeType: "text"),
                        ClickNum = c.Int(),
                        IsHot = c.Boolean(),
                        IsTop = c.Boolean(),
                        IsSlide = c.Boolean(),
                        IsRecommend = c.Boolean(),
                        Status = c.Short(),
                        ApiUpDate = c.DateTime(),
                        MediaTypeId = c.String(nullable: false, maxLength: 128),
                        LinkManId = c.String(nullable: false, maxLength: 128),
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
                .ForeignKey("dbo.LinkMan", t => t.LinkManId, cascadeDelete: true)
                .ForeignKey("dbo.MediaType", t => t.MediaTypeId, cascadeDelete: true)
                .Index(t => t.MediaTypeId)
                .Index(t => t.LinkManId);
            
            CreateTable(
                "dbo.LinkMan",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 32),
                        WorkName = c.String(maxLength: 32),
                        LinkManType = c.String(maxLength: 32),
                        Phone = c.String(maxLength: 64),
                        QQ = c.String(maxLength: 32),
                        WeiXin = c.String(maxLength: 32),
                        Address = c.String(maxLength: 128),
                        Sex = c.String(maxLength: 16),
                        Status = c.String(maxLength: 32),
                        Image = c.String(maxLength: 128),
                        OpenId = c.String(maxLength: 64),
                        Password = c.String(maxLength: 32),
                        LoginName = c.String(maxLength: 32),
                        CommpanyId = c.String(nullable: false, maxLength: 128),
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
                .ForeignKey("dbo.Commpany", t => t.CommpanyId, cascadeDelete: true)
                .Index(t => t.CommpanyId);
            
            CreateTable(
                "dbo.Commpany",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 32),
                        City = c.String(maxLength: 32),
                        CommpanyType = c.String(maxLength: 32),
                        CommpanyGrade = c.String(maxLength: 32),
                        Address = c.String(maxLength: 128),
                        Phone = c.String(maxLength: 64),
                        IsBusiness = c.Boolean(nullable: false),
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
                "dbo.PayAccount",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        AccountType = c.String(maxLength: 32),
                        AccountName = c.String(nullable: false, maxLength: 64),
                        AccountNum = c.String(maxLength: 64),
                        Status = c.Short(),
                        LinkManId = c.String(nullable: false, maxLength: 128),
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
                .ForeignKey("dbo.LinkMan", t => t.LinkManId, cascadeDelete: true)
                .Index(t => t.LinkManId);
            
            CreateTable(
                "dbo.MediaTag",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        TagName = c.String(nullable: false, maxLength: 128),
                        IsHot = c.Boolean(),
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
                "dbo.MediaType",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        TypeName = c.String(nullable: false, maxLength: 32),
                        CallIndex = c.String(maxLength: 32),
                        ParentId = c.String(maxLength: 32),
                        Image = c.String(maxLength: 256),
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
                "dbo.AdPosition",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 32),
                        MediaTypeId = c.String(nullable: false, maxLength: 128),
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
                .ForeignKey("dbo.MediaType", t => t.MediaTypeId, cascadeDelete: true)
                .Index(t => t.MediaTypeId);
            
            CreateTable(
                "dbo.MediaAndTag",
                c => new
                    {
                        MediaId = c.String(nullable: false, maxLength: 128),
                        TagId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.MediaId, t.TagId })
                .ForeignKey("dbo.Media", t => t.MediaId, cascadeDelete: true)
                .ForeignKey("dbo.MediaTag", t => t.TagId, cascadeDelete: true)
                .Index(t => t.MediaId)
                .Index(t => t.TagId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MediaPrice", "MediaId", "dbo.Media");
            DropForeignKey("dbo.Media", "MediaTypeId", "dbo.MediaType");
            DropForeignKey("dbo.AdPosition", "MediaTypeId", "dbo.MediaType");
            DropForeignKey("dbo.MediaAndTag", "TagId", "dbo.MediaTag");
            DropForeignKey("dbo.MediaAndTag", "MediaId", "dbo.Media");
            DropForeignKey("dbo.Media", "LinkManId", "dbo.LinkMan");
            DropForeignKey("dbo.PayAccount", "LinkManId", "dbo.LinkMan");
            DropForeignKey("dbo.LinkMan", "CommpanyId", "dbo.Commpany");
            DropIndex("dbo.MediaAndTag", new[] { "TagId" });
            DropIndex("dbo.MediaAndTag", new[] { "MediaId" });
            DropIndex("dbo.AdPosition", new[] { "MediaTypeId" });
            DropIndex("dbo.PayAccount", new[] { "LinkManId" });
            DropIndex("dbo.LinkMan", new[] { "CommpanyId" });
            DropIndex("dbo.Media", new[] { "LinkManId" });
            DropIndex("dbo.Media", new[] { "MediaTypeId" });
            DropIndex("dbo.MediaPrice", new[] { "MediaId" });
            DropTable("dbo.MediaAndTag");
            DropTable("dbo.AdPosition");
            DropTable("dbo.MediaType");
            DropTable("dbo.MediaTag");
            DropTable("dbo.PayAccount");
            DropTable("dbo.Commpany");
            DropTable("dbo.LinkMan");
            DropTable("dbo.Media");
            DropTable("dbo.MediaPrice");
        }
    }
}
