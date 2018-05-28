namespace Ada.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateMedias : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MediaPriceChange",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        PurchasePrice = c.Decimal(precision: 18, scale: 2),
                        MarketPrice = c.Decimal(precision: 18, scale: 2),
                        SellPrice = c.Decimal(precision: 18, scale: 2),
                        ChangeDate = c.DateTime(),
                        MediaPriceId = c.String(nullable: false, maxLength: 128),
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
                .ForeignKey("dbo.MediaPrice", t => t.MediaPriceId, cascadeDelete: true)
                .Index(t => t.MediaPriceId);
            
            AddColumn("dbo.Media", "PriceProtectionDate", c => c.Short());
            AddColumn("dbo.Media", "PriceProtectionIsPrePay", c => c.Boolean());
            AddColumn("dbo.Media", "PriceProtectionIsBrand", c => c.Boolean());
            AddColumn("dbo.Media", "PriceProtectionRemark", c => c.String(maxLength: 512));
            AddColumn("dbo.Receivables", "RelationshipNum", c => c.String(maxLength: 32));
            AddColumn("dbo.PurchaseReturnOrder", "LinkManName", c => c.String(maxLength: 128));
            AddColumn("dbo.PurchaseReturnOrder", "LinkManId", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.PurchaseReturenOrderDetail", "PurchaseOrderDetailId", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("dbo.PurchaseReturenOrderDetail", "PurchaseOrderDetailId");
            CreateIndex("dbo.PurchaseReturnOrder", "LinkManId");
            AddForeignKey("dbo.PurchaseReturenOrderDetail", "PurchaseOrderDetailId", "dbo.PurchaseOrderDetail", "Id");
            AddForeignKey("dbo.PurchaseReturnOrder", "LinkManId", "dbo.LinkMan", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PurchaseReturnOrder", "LinkManId", "dbo.LinkMan");
            DropForeignKey("dbo.PurchaseReturenOrderDetail", "PurchaseOrderDetailId", "dbo.PurchaseOrderDetail");
            DropForeignKey("dbo.MediaPriceChange", "MediaPriceId", "dbo.MediaPrice");
            DropIndex("dbo.PurchaseReturnOrder", new[] { "LinkManId" });
            DropIndex("dbo.PurchaseReturenOrderDetail", new[] { "PurchaseOrderDetailId" });
            DropIndex("dbo.MediaPriceChange", new[] { "MediaPriceId" });
            AlterColumn("dbo.PurchaseReturenOrderDetail", "PurchaseOrderDetailId", c => c.String(nullable: false, maxLength: 32));
            DropColumn("dbo.PurchaseReturnOrder", "LinkManId");
            DropColumn("dbo.PurchaseReturnOrder", "LinkManName");
            DropColumn("dbo.Receivables", "RelationshipNum");
            DropColumn("dbo.Media", "PriceProtectionRemark");
            DropColumn("dbo.Media", "PriceProtectionIsBrand");
            DropColumn("dbo.Media", "PriceProtectionIsPrePay");
            DropColumn("dbo.Media", "PriceProtectionDate");
            DropTable("dbo.MediaPriceChange");
        }
    }
}
