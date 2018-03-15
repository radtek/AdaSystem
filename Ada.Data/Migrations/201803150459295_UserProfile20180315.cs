namespace Ada.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserProfile20180315 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.OrderDetailComment",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Score = c.Short(),
                        Content = c.String(maxLength: 512),
                        Transactor = c.String(maxLength: 32),
                        TransactorId = c.String(maxLength: 32),
                        CommentDate = c.DateTime(),
                        BusinessOrderDetailId = c.String(nullable: false, maxLength: 128),
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
                .ForeignKey("dbo.BusinessOrderDetail", t => t.BusinessOrderDetailId)
                .Index(t => t.BusinessOrderDetailId);
            
            CreateTable(
                "dbo.MediaDevelop",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        MediaName = c.String(nullable: false, maxLength: 128),
                        MediaID = c.String(maxLength: 64),
                        Platform = c.String(maxLength: 32),
                        Content = c.String(maxLength: 512),
                        SubBy = c.String(maxLength: 32),
                        SubById = c.String(maxLength: 32),
                        SubDate = c.DateTime(),
                        Transactor = c.String(maxLength: 32),
                        TransactorId = c.String(maxLength: 32),
                        GetDate = c.DateTime(),
                        Status = c.Short(),
                        AuditStatus = c.Short(),
                        AuditBy = c.String(maxLength: 32),
                        AuditById = c.String(maxLength: 32),
                        AuditDate = c.DateTime(),
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
                .ForeignKey("dbo.MediaType", t => t.MediaTypeId)
                .Index(t => t.MediaTypeId);
            
            CreateTable(
                "dbo.MediaDevelopProgress",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        ProgressContent = c.String(maxLength: 1024),
                        ProgressDate = c.DateTime(),
                        MediaDevelopId = c.String(nullable: false, maxLength: 128),
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
                .ForeignKey("dbo.MediaDevelop", t => t.MediaDevelopId, cascadeDelete: true)
                .Index(t => t.MediaDevelopId);
            
            CreateTable(
                "dbo.OfferRatioDetail",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        OfferMax = c.Decimal(precision: 18, scale: 2),
                        OfferMin = c.Decimal(precision: 18, scale: 2),
                        RatioType = c.Boolean(),
                        RatioValue = c.Decimal(precision: 18, scale: 2),
                        OfferRatioId = c.String(nullable: false, maxLength: 128),
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
                .ForeignKey("dbo.OfferRatio", t => t.OfferRatioId, cascadeDelete: true)
                .Index(t => t.OfferRatioId);
            
            CreateTable(
                "dbo.OfferRatio",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Status = c.Short(),
                        OfferType = c.String(maxLength: 32),
                        Title = c.String(nullable: false, maxLength: 128),
                        Transactor = c.String(maxLength: 32),
                        TransactorId = c.String(maxLength: 32),
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
            
            AddColumn("dbo.MediaType", "IsComment", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OfferRatioDetail", "OfferRatioId", "dbo.OfferRatio");
            DropForeignKey("dbo.MediaDevelop", "MediaTypeId", "dbo.MediaType");
            DropForeignKey("dbo.MediaDevelopProgress", "MediaDevelopId", "dbo.MediaDevelop");
            DropForeignKey("dbo.OrderDetailComment", "BusinessOrderDetailId", "dbo.BusinessOrderDetail");
            DropIndex("dbo.OfferRatioDetail", new[] { "OfferRatioId" });
            DropIndex("dbo.MediaDevelopProgress", new[] { "MediaDevelopId" });
            DropIndex("dbo.MediaDevelop", new[] { "MediaTypeId" });
            DropIndex("dbo.OrderDetailComment", new[] { "BusinessOrderDetailId" });
            DropColumn("dbo.MediaType", "IsComment");
            DropTable("dbo.OfferRatio");
            DropTable("dbo.OfferRatioDetail");
            DropTable("dbo.MediaDevelopProgress");
            DropTable("dbo.MediaDevelop");
            DropTable("dbo.OrderDetailComment");
        }
    }
}
