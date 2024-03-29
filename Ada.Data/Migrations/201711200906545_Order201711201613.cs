namespace Ada.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Order201711201613 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.PurchaseOrder", "LinkManId", "dbo.LinkMan");
            DropForeignKey("dbo.MediaPrice", "MediaId", "dbo.Media");
            DropForeignKey("dbo.Media", "LinkManId", "dbo.LinkMan");
            DropForeignKey("dbo.Media", "MediaTypeId", "dbo.MediaType");
            DropForeignKey("dbo.LinkMan", "CommpanyId", "dbo.Commpany");
            DropForeignKey("dbo.PayAccount", "LinkManId", "dbo.LinkMan");
            DropForeignKey("dbo.BusinessOrderDetail", "BusinessOrderId", "dbo.BusinessOrder");
            DropForeignKey("dbo.BusinessOrderDetail", "MediaPriceId", "dbo.MediaPrice");
            DropForeignKey("dbo.PurchaseOrderDetail", "PurchaseOrderId", "dbo.PurchaseOrder");
            DropForeignKey("dbo.AdPosition", "MediaTypeId", "dbo.MediaType");
            DropForeignKey("dbo.PurchaseReturenOrderDetail", "PurchaseReturnOrderId", "dbo.PurchaseReturnOrder");
            DropForeignKey("dbo.ManagerLoginLog", "ManagerId", "dbo.Manager");
            DropForeignKey("dbo.ManagerAction", "ManagerId", "dbo.Manager");
            DropForeignKey("dbo.ManagerAction", "ActionInfoId", "dbo.Action");
            DropForeignKey("dbo.BusinessReturnOrderDetail", "BusinessReturnOrderId", "dbo.BusinessReturnOrder");
            DropForeignKey("dbo.Field", "FieldTypeId", "dbo.FieldType");
            DropIndex("dbo.BusinessOrder", new[] { "LinkManId" });
            DropIndex("dbo.PurchaseOrder", new[] { "LinkManId" });
            DropIndex("dbo.PurchaseOrderDetail", new[] { "MediaPriceId" });
            AddColumn("dbo.BusinessOrderDetail", "MediaByPurchase", c => c.String(maxLength: 32));
            AddColumn("dbo.PurchaseOrder", "BusinessBy", c => c.String(maxLength: 32));
            AddColumn("dbo.PurchaseOrder", "BusinessById", c => c.String(maxLength: 32));
            AddColumn("dbo.PurchaseOrderDetail", "VerificationMoney", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.PurchaseOrderDetail", "ConfirmVerificationMoney", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.PurchaseOrderDetail", "VerificationStatus", c => c.Short());
            AddColumn("dbo.PurchaseOrderDetail", "Status", c => c.Short());
            AddColumn("dbo.PurchaseOrderDetail", "Transactor", c => c.String(maxLength: 32));
            AddColumn("dbo.PurchaseOrderDetail", "TransactorId", c => c.String(maxLength: 32));
            AddColumn("dbo.PurchaseOrderDetail", "AuditBy", c => c.String(maxLength: 32));
            AddColumn("dbo.PurchaseOrderDetail", "AuditById", c => c.String(maxLength: 32));
            AddColumn("dbo.PurchaseOrderDetail", "AuditDate", c => c.DateTime());
            AddColumn("dbo.PurchaseOrderDetail", "AuditStatus", c => c.Short());
            AddColumn("dbo.PurchaseOrderDetail", "CancelBy", c => c.String(maxLength: 32));
            AddColumn("dbo.PurchaseOrderDetail", "CancelById", c => c.String(maxLength: 32));
            AddColumn("dbo.PurchaseOrderDetail", "CancelDate", c => c.DateTime());
            AddColumn("dbo.PurchaseOrderDetail", "SettlementType", c => c.String(maxLength: 32));
            AddColumn("dbo.PurchaseOrderDetail", "PurchaseType", c => c.String(maxLength: 32));
            AddColumn("dbo.PurchaseOrderDetail", "LinkManName", c => c.String(maxLength: 64));
            AddColumn("dbo.PurchaseOrderDetail", "LinkManId", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.BusinessOrder", "LinkManId", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.PurchaseOrderDetail", "MediaPriceId", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("dbo.BusinessOrder", "LinkManId");
            CreateIndex("dbo.PurchaseOrderDetail", "MediaPriceId");
            CreateIndex("dbo.PurchaseOrderDetail", "LinkManId");
            AddForeignKey("dbo.PurchaseOrderDetail", "LinkManId", "dbo.LinkMan", "Id");
            AddForeignKey("dbo.MediaPrice", "MediaId", "dbo.Media", "Id");
            AddForeignKey("dbo.Media", "LinkManId", "dbo.LinkMan", "Id");
            AddForeignKey("dbo.Media", "MediaTypeId", "dbo.MediaType", "Id");
            AddForeignKey("dbo.LinkMan", "CommpanyId", "dbo.Commpany", "Id");
            AddForeignKey("dbo.PayAccount", "LinkManId", "dbo.LinkMan", "Id");
            AddForeignKey("dbo.BusinessOrderDetail", "BusinessOrderId", "dbo.BusinessOrder", "Id");
            AddForeignKey("dbo.BusinessOrderDetail", "MediaPriceId", "dbo.MediaPrice", "Id");
            AddForeignKey("dbo.PurchaseOrderDetail", "PurchaseOrderId", "dbo.PurchaseOrder", "Id");
            AddForeignKey("dbo.AdPosition", "MediaTypeId", "dbo.MediaType", "Id");
            AddForeignKey("dbo.PurchaseReturenOrderDetail", "PurchaseReturnOrderId", "dbo.PurchaseReturnOrder", "Id");
            AddForeignKey("dbo.ManagerLoginLog", "ManagerId", "dbo.Manager", "Id");
            AddForeignKey("dbo.ManagerAction", "ManagerId", "dbo.Manager", "Id");
            AddForeignKey("dbo.ManagerAction", "ActionInfoId", "dbo.Action", "Id");
            AddForeignKey("dbo.BusinessReturnOrderDetail", "BusinessReturnOrderId", "dbo.BusinessReturnOrder", "Id");
            AddForeignKey("dbo.Field", "FieldTypeId", "dbo.FieldType", "Id");
            DropColumn("dbo.PurchaseOrder", "PurchaseType");
            DropColumn("dbo.PurchaseOrder", "Tax");
            DropColumn("dbo.PurchaseOrder", "DiscountRate");
            DropColumn("dbo.PurchaseOrder", "VerificationMoney");
            DropColumn("dbo.PurchaseOrder", "ConfirmVerificationMoney");
            DropColumn("dbo.PurchaseOrder", "VerificationStatus");
            DropColumn("dbo.PurchaseOrder", "Transactor");
            DropColumn("dbo.PurchaseOrder", "TransactorId");
            DropColumn("dbo.PurchaseOrder", "AuditBy");
            DropColumn("dbo.PurchaseOrder", "AuditById");
            DropColumn("dbo.PurchaseOrder", "AuditDate");
            DropColumn("dbo.PurchaseOrder", "AuditStatus");
            DropColumn("dbo.PurchaseOrder", "CancelBy");
            DropColumn("dbo.PurchaseOrder", "CancelById");
            DropColumn("dbo.PurchaseOrder", "CancelDate");
            DropColumn("dbo.PurchaseOrder", "LinkManName");
            DropColumn("dbo.PurchaseOrder", "SettlementType");
            DropColumn("dbo.PurchaseOrder", "LinkManId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.PurchaseOrder", "LinkManId", c => c.String(maxLength: 128));
            AddColumn("dbo.PurchaseOrder", "SettlementType", c => c.String(maxLength: 32));
            AddColumn("dbo.PurchaseOrder", "LinkManName", c => c.String(maxLength: 32));
            AddColumn("dbo.PurchaseOrder", "CancelDate", c => c.DateTime());
            AddColumn("dbo.PurchaseOrder", "CancelById", c => c.String(maxLength: 32));
            AddColumn("dbo.PurchaseOrder", "CancelBy", c => c.String(maxLength: 32));
            AddColumn("dbo.PurchaseOrder", "AuditStatus", c => c.Short());
            AddColumn("dbo.PurchaseOrder", "AuditDate", c => c.DateTime());
            AddColumn("dbo.PurchaseOrder", "AuditById", c => c.String(maxLength: 32));
            AddColumn("dbo.PurchaseOrder", "AuditBy", c => c.String(maxLength: 32));
            AddColumn("dbo.PurchaseOrder", "TransactorId", c => c.String(maxLength: 32));
            AddColumn("dbo.PurchaseOrder", "Transactor", c => c.String(maxLength: 32));
            AddColumn("dbo.PurchaseOrder", "VerificationStatus", c => c.Short());
            AddColumn("dbo.PurchaseOrder", "ConfirmVerificationMoney", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.PurchaseOrder", "VerificationMoney", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.PurchaseOrder", "DiscountRate", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.PurchaseOrder", "Tax", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.PurchaseOrder", "PurchaseType", c => c.String(maxLength: 32));
            DropForeignKey("dbo.Field", "FieldTypeId", "dbo.FieldType");
            DropForeignKey("dbo.BusinessReturnOrderDetail", "BusinessReturnOrderId", "dbo.BusinessReturnOrder");
            DropForeignKey("dbo.ManagerAction", "ActionInfoId", "dbo.Action");
            DropForeignKey("dbo.ManagerAction", "ManagerId", "dbo.Manager");
            DropForeignKey("dbo.ManagerLoginLog", "ManagerId", "dbo.Manager");
            DropForeignKey("dbo.PurchaseReturenOrderDetail", "PurchaseReturnOrderId", "dbo.PurchaseReturnOrder");
            DropForeignKey("dbo.AdPosition", "MediaTypeId", "dbo.MediaType");
            DropForeignKey("dbo.PurchaseOrderDetail", "PurchaseOrderId", "dbo.PurchaseOrder");
            DropForeignKey("dbo.BusinessOrderDetail", "MediaPriceId", "dbo.MediaPrice");
            DropForeignKey("dbo.BusinessOrderDetail", "BusinessOrderId", "dbo.BusinessOrder");
            DropForeignKey("dbo.PayAccount", "LinkManId", "dbo.LinkMan");
            DropForeignKey("dbo.LinkMan", "CommpanyId", "dbo.Commpany");
            DropForeignKey("dbo.Media", "MediaTypeId", "dbo.MediaType");
            DropForeignKey("dbo.Media", "LinkManId", "dbo.LinkMan");
            DropForeignKey("dbo.MediaPrice", "MediaId", "dbo.Media");
            DropForeignKey("dbo.PurchaseOrderDetail", "LinkManId", "dbo.LinkMan");
            DropIndex("dbo.PurchaseOrderDetail", new[] { "LinkManId" });
            DropIndex("dbo.PurchaseOrderDetail", new[] { "MediaPriceId" });
            DropIndex("dbo.BusinessOrder", new[] { "LinkManId" });
            AlterColumn("dbo.PurchaseOrderDetail", "MediaPriceId", c => c.String(maxLength: 128));
            AlterColumn("dbo.BusinessOrder", "LinkManId", c => c.String(maxLength: 128));
            DropColumn("dbo.PurchaseOrderDetail", "LinkManId");
            DropColumn("dbo.PurchaseOrderDetail", "LinkManName");
            DropColumn("dbo.PurchaseOrderDetail", "PurchaseType");
            DropColumn("dbo.PurchaseOrderDetail", "SettlementType");
            DropColumn("dbo.PurchaseOrderDetail", "CancelDate");
            DropColumn("dbo.PurchaseOrderDetail", "CancelById");
            DropColumn("dbo.PurchaseOrderDetail", "CancelBy");
            DropColumn("dbo.PurchaseOrderDetail", "AuditStatus");
            DropColumn("dbo.PurchaseOrderDetail", "AuditDate");
            DropColumn("dbo.PurchaseOrderDetail", "AuditById");
            DropColumn("dbo.PurchaseOrderDetail", "AuditBy");
            DropColumn("dbo.PurchaseOrderDetail", "TransactorId");
            DropColumn("dbo.PurchaseOrderDetail", "Transactor");
            DropColumn("dbo.PurchaseOrderDetail", "Status");
            DropColumn("dbo.PurchaseOrderDetail", "VerificationStatus");
            DropColumn("dbo.PurchaseOrderDetail", "ConfirmVerificationMoney");
            DropColumn("dbo.PurchaseOrderDetail", "VerificationMoney");
            DropColumn("dbo.PurchaseOrder", "BusinessById");
            DropColumn("dbo.PurchaseOrder", "BusinessBy");
            DropColumn("dbo.BusinessOrderDetail", "MediaByPurchase");
            CreateIndex("dbo.PurchaseOrderDetail", "MediaPriceId");
            CreateIndex("dbo.PurchaseOrder", "LinkManId");
            CreateIndex("dbo.BusinessOrder", "LinkManId");
            AddForeignKey("dbo.Field", "FieldTypeId", "dbo.FieldType", "Id", cascadeDelete: true);
            AddForeignKey("dbo.BusinessReturnOrderDetail", "BusinessReturnOrderId", "dbo.BusinessReturnOrder", "Id", cascadeDelete: true);
            AddForeignKey("dbo.ManagerAction", "ActionInfoId", "dbo.Action", "Id", cascadeDelete: true);
            AddForeignKey("dbo.ManagerAction", "ManagerId", "dbo.Manager", "Id", cascadeDelete: true);
            AddForeignKey("dbo.ManagerLoginLog", "ManagerId", "dbo.Manager", "Id", cascadeDelete: true);
            AddForeignKey("dbo.PurchaseReturenOrderDetail", "PurchaseReturnOrderId", "dbo.PurchaseReturnOrder", "Id", cascadeDelete: true);
            AddForeignKey("dbo.AdPosition", "MediaTypeId", "dbo.MediaType", "Id", cascadeDelete: true);
            AddForeignKey("dbo.PurchaseOrderDetail", "PurchaseOrderId", "dbo.PurchaseOrder", "Id", cascadeDelete: true);
            AddForeignKey("dbo.BusinessOrderDetail", "MediaPriceId", "dbo.MediaPrice", "Id", cascadeDelete: true);
            AddForeignKey("dbo.BusinessOrderDetail", "BusinessOrderId", "dbo.BusinessOrder", "Id", cascadeDelete: true);
            AddForeignKey("dbo.PayAccount", "LinkManId", "dbo.LinkMan", "Id", cascadeDelete: true);
            AddForeignKey("dbo.LinkMan", "CommpanyId", "dbo.Commpany", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Media", "MediaTypeId", "dbo.MediaType", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Media", "LinkManId", "dbo.LinkMan", "Id", cascadeDelete: true);
            AddForeignKey("dbo.MediaPrice", "MediaId", "dbo.Media", "Id", cascadeDelete: true);
            AddForeignKey("dbo.PurchaseOrder", "LinkManId", "dbo.LinkMan", "Id");
        }
    }
}
