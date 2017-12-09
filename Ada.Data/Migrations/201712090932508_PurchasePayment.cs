namespace Ada.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PurchasePayment : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.PurchasePaymentDetail", "PurchaseOrderDetailId", "dbo.PurchaseOrderDetail");
            DropIndex("dbo.PurchasePaymentDetail", new[] { "PurchaseOrderDetailId" });
            CreateTable(
                "dbo.PurchasePaymentOrderDetail",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        PurchaseOrderDetailId = c.String(nullable: false, maxLength: 128),
                        PurchasePaymentId = c.String(nullable: false, maxLength: 128),
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
                .ForeignKey("dbo.PurchaseOrderDetail", t => t.PurchaseOrderDetailId)
                .ForeignKey("dbo.PurchasePayment", t => t.PurchasePaymentId)
                .Index(t => t.PurchaseOrderDetailId)
                .Index(t => t.PurchasePaymentId);
            
            AddColumn("dbo.PurchasePaymentDetail", "AccountBank", c => c.String(maxLength: 32));
            AddColumn("dbo.PurchasePaymentDetail", "AccountName", c => c.String(maxLength: 32));
            AddColumn("dbo.PurchasePaymentDetail", "AccountNum", c => c.String(maxLength: 32));
            AddColumn("dbo.PurchasePaymentDetail", "PayMoney", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.PurchasePaymentDetail", "AuditBy", c => c.String(maxLength: 32));
            AddColumn("dbo.PurchasePaymentDetail", "AuditById", c => c.String(maxLength: 32));
            AddColumn("dbo.PurchasePaymentDetail", "AuditDate", c => c.DateTime());
            AddColumn("dbo.PurchasePaymentDetail", "AuditStatus", c => c.Short());
            AddColumn("dbo.PurchasePaymentDetail", "CancelBy", c => c.String(maxLength: 32));
            AddColumn("dbo.PurchasePaymentDetail", "CancelById", c => c.String(maxLength: 32));
            AddColumn("dbo.PurchasePaymentDetail", "CancelDate", c => c.DateTime());
            AddColumn("dbo.PurchasePaymentDetail", "ApplicationDate", c => c.DateTime());
            AddColumn("dbo.PurchasePaymentDetail", "PaymentType", c => c.String(maxLength: 32));
            AddColumn("dbo.PurchasePaymentDetail", "Status", c => c.Short());
            AddColumn("dbo.PurchasePayment", "BillDate", c => c.DateTime());
            AddColumn("dbo.PurchasePayment", "BillNum", c => c.String(maxLength: 32));
            DropColumn("dbo.PurchasePaymentDetail", "PurchaseOrderDetailId");
            DropColumn("dbo.PurchasePayment", "AccountBank");
            DropColumn("dbo.PurchasePayment", "AccountName");
            DropColumn("dbo.PurchasePayment", "AccountNum");
            DropColumn("dbo.PurchasePayment", "ApplicationNum");
            DropColumn("dbo.PurchasePayment", "PaymentType");
            DropColumn("dbo.PurchasePayment", "AuditBy");
            DropColumn("dbo.PurchasePayment", "AuditById");
            DropColumn("dbo.PurchasePayment", "AuditDate");
            DropColumn("dbo.PurchasePayment", "AuditStatus");
            DropColumn("dbo.PurchasePayment", "CancelBy");
            DropColumn("dbo.PurchasePayment", "CancelById");
            DropColumn("dbo.PurchasePayment", "CancelDate");
            DropColumn("dbo.PurchasePayment", "ApplicationDate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.PurchasePayment", "ApplicationDate", c => c.DateTime());
            AddColumn("dbo.PurchasePayment", "CancelDate", c => c.DateTime());
            AddColumn("dbo.PurchasePayment", "CancelById", c => c.String(maxLength: 32));
            AddColumn("dbo.PurchasePayment", "CancelBy", c => c.String(maxLength: 32));
            AddColumn("dbo.PurchasePayment", "AuditStatus", c => c.Short());
            AddColumn("dbo.PurchasePayment", "AuditDate", c => c.DateTime());
            AddColumn("dbo.PurchasePayment", "AuditById", c => c.String(maxLength: 32));
            AddColumn("dbo.PurchasePayment", "AuditBy", c => c.String(maxLength: 32));
            AddColumn("dbo.PurchasePayment", "PaymentType", c => c.String(maxLength: 32));
            AddColumn("dbo.PurchasePayment", "ApplicationNum", c => c.String(maxLength: 32));
            AddColumn("dbo.PurchasePayment", "AccountNum", c => c.String(maxLength: 32));
            AddColumn("dbo.PurchasePayment", "AccountName", c => c.String(maxLength: 32));
            AddColumn("dbo.PurchasePayment", "AccountBank", c => c.String(maxLength: 32));
            AddColumn("dbo.PurchasePaymentDetail", "PurchaseOrderDetailId", c => c.String(nullable: false, maxLength: 128));
            DropForeignKey("dbo.PurchasePaymentOrderDetail", "PurchasePaymentId", "dbo.PurchasePayment");
            DropForeignKey("dbo.PurchasePaymentOrderDetail", "PurchaseOrderDetailId", "dbo.PurchaseOrderDetail");
            DropIndex("dbo.PurchasePaymentOrderDetail", new[] { "PurchasePaymentId" });
            DropIndex("dbo.PurchasePaymentOrderDetail", new[] { "PurchaseOrderDetailId" });
            DropColumn("dbo.PurchasePayment", "BillNum");
            DropColumn("dbo.PurchasePayment", "BillDate");
            DropColumn("dbo.PurchasePaymentDetail", "Status");
            DropColumn("dbo.PurchasePaymentDetail", "PaymentType");
            DropColumn("dbo.PurchasePaymentDetail", "ApplicationDate");
            DropColumn("dbo.PurchasePaymentDetail", "CancelDate");
            DropColumn("dbo.PurchasePaymentDetail", "CancelById");
            DropColumn("dbo.PurchasePaymentDetail", "CancelBy");
            DropColumn("dbo.PurchasePaymentDetail", "AuditStatus");
            DropColumn("dbo.PurchasePaymentDetail", "AuditDate");
            DropColumn("dbo.PurchasePaymentDetail", "AuditById");
            DropColumn("dbo.PurchasePaymentDetail", "AuditBy");
            DropColumn("dbo.PurchasePaymentDetail", "PayMoney");
            DropColumn("dbo.PurchasePaymentDetail", "AccountNum");
            DropColumn("dbo.PurchasePaymentDetail", "AccountName");
            DropColumn("dbo.PurchasePaymentDetail", "AccountBank");
            DropTable("dbo.PurchasePaymentOrderDetail");
            CreateIndex("dbo.PurchasePaymentDetail", "PurchaseOrderDetailId");
            AddForeignKey("dbo.PurchasePaymentDetail", "PurchaseOrderDetailId", "dbo.PurchaseOrderDetail", "Id");
        }
    }
}
