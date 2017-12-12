namespace Ada.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20171212OfferLog : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BusinessOfferDetail",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        TaxMoney = c.Decimal(precision: 18, scale: 2),
                        DiscountMoney = c.Decimal(precision: 18, scale: 2),
                        Tax = c.Decimal(precision: 18, scale: 2),
                        DiscountRate = c.Decimal(precision: 18, scale: 2),
                        Money = c.Decimal(precision: 18, scale: 2),
                        CostMoney = c.Decimal(precision: 18, scale: 2),
                        SellMoney = c.Decimal(precision: 18, scale: 2),
                        MediaPriceId = c.String(nullable: false, maxLength: 128),
                        BusinessOfferId = c.String(nullable: false, maxLength: 128),
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
                .ForeignKey("dbo.BusinessOffer", t => t.BusinessOfferId)
                .ForeignKey("dbo.MediaPrice", t => t.MediaPriceId)
                .Index(t => t.MediaPriceId)
                .Index(t => t.BusinessOfferId);
            
            CreateTable(
                "dbo.BusinessOffer",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        OfferNum = c.String(maxLength: 32),
                        TotalMoney = c.Decimal(precision: 18, scale: 2),
                        Tax = c.Decimal(precision: 18, scale: 2),
                        DiscountRate = c.Decimal(precision: 18, scale: 2),
                        DiscountMoney = c.Decimal(precision: 18, scale: 2),
                        TotalSellMoney = c.Decimal(precision: 18, scale: 2),
                        TotalTaxMoney = c.Decimal(precision: 18, scale: 2),
                        Transactor = c.String(maxLength: 32),
                        TransactorId = c.String(maxLength: 32),
                        Status = c.Short(),
                        ValidDays = c.Short(),
                        OfferDate = c.DateTime(),
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
                .ForeignKey("dbo.LinkMan", t => t.LinkManId)
                .Index(t => t.LinkManId);
            
            CreateTable(
                "dbo.FollowUp",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Content = c.String(),
                        FollowUpWay = c.String(maxLength: 32),
                        LinkManId = c.String(nullable: false, maxLength: 128),
                        NextTime = c.DateTime(),
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
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.LinkMan", t => t.LinkManId)
                .Index(t => t.LinkManId);
            
            CreateTable(
                "dbo.WorkLog",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Date = c.DateTime(),
                        Title = c.String(maxLength: 128),
                        Content = c.String(),
                        Manager = c.String(),
                        Director = c.String(),
                        Boss = c.String(),
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
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.BusinessOfferDetail", "MediaPriceId", "dbo.MediaPrice");
            DropForeignKey("dbo.BusinessOfferDetail", "BusinessOfferId", "dbo.BusinessOffer");
            DropForeignKey("dbo.BusinessOffer", "LinkManId", "dbo.LinkMan");
            DropForeignKey("dbo.FollowUp", "LinkManId", "dbo.LinkMan");
            DropIndex("dbo.FollowUp", new[] { "LinkManId" });
            DropIndex("dbo.BusinessOffer", new[] { "LinkManId" });
            DropIndex("dbo.BusinessOfferDetail", new[] { "BusinessOfferId" });
            DropIndex("dbo.BusinessOfferDetail", new[] { "MediaPriceId" });
            DropTable("dbo.WorkLog");
            DropTable("dbo.FollowUp");
            DropTable("dbo.BusinessOffer");
            DropTable("dbo.BusinessOfferDetail");
        }
    }
}
