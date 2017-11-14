namespace Ada.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Orders : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BusinessOrder",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        BusinessType = c.String(maxLength: 32),
                        OrderNum = c.String(nullable: false, maxLength: 32),
                        TotalMoney = c.Decimal(nullable: false, precision: 18, scale: 2),
                        VerificationMoney = c.Decimal(precision: 18, scale: 2),
                        ConfirmVerificationMoney = c.Decimal(precision: 18, scale: 2),
                        VerificationStatus = c.Short(),
                        Transactor = c.String(maxLength: 32),
                        TransactorId = c.String(maxLength: 32),
                        Status = c.Short(),
                        Tax = c.Short(),
                        DiscountRate = c.Short(),
                        AuditBy = c.String(maxLength: 32),
                        AuditById = c.String(maxLength: 32),
                        AuditDate = c.DateTime(),
                        AuditStatus = c.Short(),
                        CancelBy = c.String(maxLength: 32),
                        CancelById = c.String(maxLength: 32),
                        CancelDate = c.DateTime(),
                        LinkManName = c.String(maxLength: 32),
                        SettlementType = c.String(maxLength: 32),
                        LinkManId = c.String(maxLength: 128),
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
                "dbo.BusinessOrderDetail",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        TaxMoney = c.Decimal(precision: 18, scale: 2),
                        DiscountMoney = c.Decimal(precision: 18, scale: 2),
                        Tax = c.Short(),
                        DiscountRate = c.Short(),
                        AdPositionName = c.String(maxLength: 32),
                        Money = c.Decimal(precision: 18, scale: 2),
                        CostMoney = c.Decimal(precision: 18, scale: 2),
                        SellMoney = c.Decimal(precision: 18, scale: 2),
                        PrePublishDate = c.DateTime(),
                        MediaTitle = c.String(maxLength: 512),
                        MediaTypeName = c.String(maxLength: 32),
                        MediaName = c.String(maxLength: 128),
                        MediaPriceId = c.String(nullable: false, maxLength: 128),
                        BusinessOrderId = c.String(nullable: false, maxLength: 128),
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
                .ForeignKey("dbo.BusinessOrder", t => t.BusinessOrderId, cascadeDelete: true)
                .ForeignKey("dbo.MediaPrice", t => t.MediaPriceId, cascadeDelete: true)
                .Index(t => t.MediaPriceId)
                .Index(t => t.BusinessOrderId);
            
            CreateTable(
                "dbo.PurchaseOrder",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        BusinessOrderId = c.String(maxLength: 32),
                        PurchaseType = c.String(maxLength: 32),
                        OrderNum = c.String(nullable: false, maxLength: 32),
                        TotalMoney = c.Decimal(nullable: false, precision: 18, scale: 2),
                        VerificationMoney = c.Decimal(precision: 18, scale: 2),
                        ConfirmVerificationMoney = c.Decimal(precision: 18, scale: 2),
                        VerificationStatus = c.Short(),
                        Transactor = c.String(maxLength: 32),
                        TransactorId = c.String(maxLength: 32),
                        Status = c.Short(),
                        DiscountRate = c.Short(),
                        AuditBy = c.String(maxLength: 32),
                        AuditById = c.String(maxLength: 32),
                        AuditDate = c.DateTime(),
                        AuditStatus = c.Short(),
                        CancelBy = c.String(maxLength: 32),
                        CancelById = c.String(maxLength: 32),
                        CancelDate = c.DateTime(),
                        LinkManName = c.String(maxLength: 32),
                        SettlementType = c.String(maxLength: 32),
                        LinkManId = c.String(maxLength: 128),
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
                "dbo.PurchaseOrderDetail",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        BusinessOrderDetailId = c.String(maxLength: 32),
                        DiscountMoney = c.Decimal(precision: 18, scale: 2),
                        DiscountRate = c.Short(),
                        AdPositionName = c.String(maxLength: 32),
                        Money = c.Decimal(precision: 18, scale: 2),
                        CostMoney = c.Decimal(precision: 18, scale: 2),
                        PurchaseMoney = c.Decimal(precision: 18, scale: 2),
                        PublishDate = c.DateTime(),
                        MediaTitle = c.String(maxLength: 512),
                        PublishLink = c.String(maxLength: 512),
                        MediaTypeName = c.String(maxLength: 32),
                        MediaName = c.String(maxLength: 128),
                        MediaPriceId = c.String(maxLength: 128),
                        PurchaseOrderId = c.String(nullable: false, maxLength: 128),
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
                .ForeignKey("dbo.MediaPrice", t => t.MediaPriceId)
                .ForeignKey("dbo.PurchaseOrder", t => t.PurchaseOrderId, cascadeDelete: true)
                .Index(t => t.MediaPriceId)
                .Index(t => t.PurchaseOrderId);
            
            CreateTable(
                "dbo.PurchaseReturenOrderDetail",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        PurchaseOrderDetailId = c.String(nullable: false, maxLength: 32),
                        Money = c.Decimal(precision: 18, scale: 2),
                        ReturnReason = c.String(maxLength: 512),
                        ReturnType = c.String(maxLength: 32),
                        ReturnDate = c.DateTime(),
                        PurchaseReturnOrderId = c.String(nullable: false, maxLength: 128),
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
                .ForeignKey("dbo.PurchaseReturnOrder", t => t.PurchaseReturnOrderId, cascadeDelete: true)
                .Index(t => t.PurchaseReturnOrderId);
            
            CreateTable(
                "dbo.PurchaseReturnOrder",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Status = c.Short(),
                        PurchaseOrderId = c.String(nullable: false, maxLength: 32),
                        ReturnOrderNum = c.String(maxLength: 32),
                        TotalMoney = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Transactor = c.String(maxLength: 32),
                        TransactorId = c.String(maxLength: 32),
                        AuditBy = c.String(maxLength: 32),
                        AuditById = c.String(maxLength: 32),
                        AuditDate = c.DateTime(),
                        AuditStatus = c.Short(),
                        CancelBy = c.String(maxLength: 32),
                        CancelById = c.String(maxLength: 32),
                        CancelDate = c.DateTime(),
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
                "dbo.BusinessReturnOrderDetail",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        BusinessOrderDetailId = c.String(nullable: false, maxLength: 32),
                        Money = c.Decimal(precision: 18, scale: 2),
                        ReturnReason = c.String(maxLength: 512),
                        ReturnType = c.String(maxLength: 32),
                        ReturnDate = c.DateTime(),
                        BusinessReturnOrderId = c.String(nullable: false, maxLength: 128),
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
                .ForeignKey("dbo.BusinessReturnOrder", t => t.BusinessReturnOrderId, cascadeDelete: true)
                .Index(t => t.BusinessReturnOrderId);
            
            CreateTable(
                "dbo.BusinessReturnOrder",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Status = c.Short(),
                        BusinessOrderId = c.String(nullable: false, maxLength: 32),
                        ReturnOrderNum = c.String(maxLength: 32),
                        TotalMoney = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Transactor = c.String(maxLength: 32),
                        TransactorId = c.String(maxLength: 32),
                        AuditBy = c.String(maxLength: 32),
                        AuditById = c.String(maxLength: 32),
                        AuditDate = c.DateTime(),
                        AuditStatus = c.Short(),
                        CancelBy = c.String(maxLength: 32),
                        CancelById = c.String(maxLength: 32),
                        CancelDate = c.DateTime(),
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
            DropForeignKey("dbo.BusinessReturnOrderDetail", "BusinessReturnOrderId", "dbo.BusinessReturnOrder");
            DropForeignKey("dbo.PurchaseReturenOrderDetail", "PurchaseReturnOrderId", "dbo.PurchaseReturnOrder");
            DropForeignKey("dbo.PurchaseOrderDetail", "PurchaseOrderId", "dbo.PurchaseOrder");
            DropForeignKey("dbo.PurchaseOrderDetail", "MediaPriceId", "dbo.MediaPrice");
            DropForeignKey("dbo.PurchaseOrder", "LinkManId", "dbo.LinkMan");
            DropForeignKey("dbo.BusinessOrder", "LinkManId", "dbo.LinkMan");
            DropForeignKey("dbo.BusinessOrderDetail", "MediaPriceId", "dbo.MediaPrice");
            DropForeignKey("dbo.BusinessOrderDetail", "BusinessOrderId", "dbo.BusinessOrder");
            DropIndex("dbo.BusinessReturnOrderDetail", new[] { "BusinessReturnOrderId" });
            DropIndex("dbo.PurchaseReturenOrderDetail", new[] { "PurchaseReturnOrderId" });
            DropIndex("dbo.PurchaseOrderDetail", new[] { "PurchaseOrderId" });
            DropIndex("dbo.PurchaseOrderDetail", new[] { "MediaPriceId" });
            DropIndex("dbo.PurchaseOrder", new[] { "LinkManId" });
            DropIndex("dbo.BusinessOrderDetail", new[] { "BusinessOrderId" });
            DropIndex("dbo.BusinessOrderDetail", new[] { "MediaPriceId" });
            DropIndex("dbo.BusinessOrder", new[] { "LinkManId" });
            DropTable("dbo.BusinessReturnOrder");
            DropTable("dbo.BusinessReturnOrderDetail");
            DropTable("dbo.PurchaseReturnOrder");
            DropTable("dbo.PurchaseReturenOrderDetail");
            DropTable("dbo.PurchaseOrderDetail");
            DropTable("dbo.PurchaseOrder");
            DropTable("dbo.BusinessOrderDetail");
            DropTable("dbo.BusinessOrder");
        }
    }
}
