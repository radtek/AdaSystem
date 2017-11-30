namespace Ada.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Finance : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BusinessWriteOff",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        WriteOffDate = c.DateTime(),
                        Money = c.Decimal(precision: 18, scale: 2),
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
            
            CreateTable(
                "dbo.BusinessPayee",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Transactor = c.String(maxLength: 32),
                        TransactorId = c.String(maxLength: 32),
                        Money = c.Decimal(precision: 18, scale: 2),
                        ClaimDate = c.DateTime(),
                        VerificationMoney = c.Decimal(precision: 18, scale: 2),
                        ConfirmVerificationMoney = c.Decimal(precision: 18, scale: 2),
                        VerificationStatus = c.Short(),
                        ReceivablesId = c.String(nullable: false, maxLength: 128),
                        LinkManId = c.String(nullable: false, maxLength: 128),
                        LinkManName = c.String(maxLength: 32),
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
                .ForeignKey("dbo.Receivables", t => t.ReceivablesId)
                .Index(t => t.ReceivablesId)
                .Index(t => t.LinkManId);
            
            CreateTable(
                "dbo.BusinessPayment",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        AccountBank = c.String(maxLength: 32),
                        AccountName = c.String(maxLength: 32),
                        AccountNum = c.String(maxLength: 32),
                        AuditBy = c.String(maxLength: 32),
                        AuditById = c.String(maxLength: 32),
                        AuditDate = c.DateTime(),
                        AuditStatus = c.Short(),
                        CancelBy = c.String(maxLength: 32),
                        CancelById = c.String(maxLength: 32),
                        CancelDate = c.DateTime(),
                        ApplicationDate = c.DateTime(),
                        PayMoney = c.Decimal(precision: 18, scale: 2),
                        ApplicationNum = c.String(maxLength: 32),
                        PaymentType = c.String(maxLength: 32),
                        Status = c.Short(),
                        Transactor = c.String(maxLength: 32),
                        TransactorId = c.String(maxLength: 32),
                        Image = c.String(maxLength: 512),
                        BusinessPayeeId = c.String(nullable: false, maxLength: 128),
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
                .ForeignKey("dbo.BusinessPayee", t => t.BusinessPayeeId)
                .Index(t => t.BusinessPayeeId);
            
            CreateTable(
                "dbo.BillPayment",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        PaymentType = c.String(maxLength: 32),
                        BillNum = c.String(maxLength: 32),
                        BillDate = c.DateTime(),
                        LinkManName = c.String(maxLength: 32),
                        LinkManId = c.String(nullable: false, maxLength: 128),
                        AccountBank = c.String(maxLength: 32),
                        AccountName = c.String(maxLength: 32),
                        AccountNum = c.String(maxLength: 32),
                        Transactor = c.String(maxLength: 32),
                        TransactorId = c.String(maxLength: 32),
                        RequestType = c.Short(),
                        RequestNum = c.String(maxLength: 32),
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
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.LinkMan", t => t.LinkManId)
                .Index(t => t.LinkManId);
            
            CreateTable(
                "dbo.BillPaymentDetail",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        IncomeExpendId = c.String(nullable: false, maxLength: 128),
                        IncomeExpendName = c.String(maxLength: 32),
                        SettleAccountName = c.String(maxLength: 32),
                        SettleType = c.String(maxLength: 32),
                        SettleAccountId = c.String(nullable: false, maxLength: 128),
                        SettleNum = c.String(maxLength: 32),
                        BillPaymentId = c.String(nullable: false, maxLength: 128),
                        Money = c.Decimal(precision: 18, scale: 2),
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
                .ForeignKey("dbo.BillPayment", t => t.BillPaymentId)
                .ForeignKey("dbo.IncomeExpend", t => t.IncomeExpendId)
                .ForeignKey("dbo.SettleAccount", t => t.SettleAccountId)
                .Index(t => t.IncomeExpendId)
                .Index(t => t.SettleAccountId)
                .Index(t => t.BillPaymentId);
            
            CreateTable(
                "dbo.IncomeExpend",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        SubjectName = c.String(maxLength: 32),
                        SubjectNum = c.String(maxLength: 32),
                        SubjectType = c.Short(),
                        IsMain = c.Boolean(),
                        ParentId = c.String(maxLength: 32),
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
                "dbo.Receivables",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        ReceivablesType = c.String(maxLength: 32),
                        AccountBank = c.String(maxLength: 32),
                        AccountName = c.String(maxLength: 32),
                        AccountNum = c.String(maxLength: 32),
                        Transactor = c.String(maxLength: 32),
                        TransactorId = c.String(maxLength: 32),
                        Money = c.Decimal(precision: 18, scale: 2),
                        BalanceMoney = c.Decimal(precision: 18, scale: 2),
                        TaxMoney = c.Decimal(precision: 18, scale: 2),
                        IncomeExpendId = c.String(nullable: false, maxLength: 128),
                        IncomeExpendName = c.String(maxLength: 32),
                        SettleAccountName = c.String(maxLength: 32),
                        SettleType = c.String(maxLength: 32),
                        SettleAccountId = c.String(nullable: false, maxLength: 128),
                        BillNum = c.String(maxLength: 32),
                        BillDate = c.DateTime(),
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
                .ForeignKey("dbo.IncomeExpend", t => t.IncomeExpendId)
                .ForeignKey("dbo.SettleAccount", t => t.SettleAccountId)
                .Index(t => t.IncomeExpendId)
                .Index(t => t.SettleAccountId);
            
            CreateTable(
                "dbo.SettleAccount",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        SettleName = c.String(maxLength: 32),
                        AccountBank = c.String(maxLength: 32),
                        AccountName = c.String(maxLength: 32),
                        AccountNum = c.String(maxLength: 32),
                        Money = c.Decimal(precision: 18, scale: 2),
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
                "dbo.PurchasePaymentDetail",
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
            
            CreateTable(
                "dbo.PurchasePayment",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        LinkManName = c.String(maxLength: 32),
                        LinkManId = c.String(nullable: false, maxLength: 128),
                        AccountBank = c.String(maxLength: 32),
                        AccountName = c.String(maxLength: 32),
                        AccountNum = c.String(maxLength: 32),
                        ApplicationNum = c.String(maxLength: 32),
                        PaymentType = c.String(maxLength: 32),
                        Status = c.Short(),
                        Transactor = c.String(maxLength: 32),
                        TransactorId = c.String(maxLength: 32),
                        AuditBy = c.String(maxLength: 32),
                        AuditById = c.String(maxLength: 32),
                        AuditDate = c.DateTime(),
                        AuditStatus = c.Short(),
                        CancelBy = c.String(maxLength: 32),
                        CancelById = c.String(maxLength: 32),
                        CancelDate = c.DateTime(),
                        ApplicationDate = c.DateTime(),
                        PayMoney = c.Decimal(precision: 18, scale: 2),
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
                "dbo.BusinessOrderWriteOff",
                c => new
                    {
                        BusinessWriteOffId = c.String(nullable: false, maxLength: 128),
                        BusinessOrderId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.BusinessWriteOffId, t.BusinessOrderId })
                .ForeignKey("dbo.BusinessWriteOff", t => t.BusinessWriteOffId, cascadeDelete: true)
                .ForeignKey("dbo.BusinessOrder", t => t.BusinessOrderId, cascadeDelete: true)
                .Index(t => t.BusinessWriteOffId)
                .Index(t => t.BusinessOrderId);
            
            CreateTable(
                "dbo.BusinessPayeeWriteOff",
                c => new
                    {
                        BusinessWriteOffId = c.String(nullable: false, maxLength: 128),
                        BusinessPayeeId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.BusinessWriteOffId, t.BusinessPayeeId })
                .ForeignKey("dbo.BusinessWriteOff", t => t.BusinessWriteOffId, cascadeDelete: true)
                .ForeignKey("dbo.BusinessPayee", t => t.BusinessPayeeId, cascadeDelete: true)
                .Index(t => t.BusinessWriteOffId)
                .Index(t => t.BusinessPayeeId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.BusinessPayeeWriteOff", "BusinessPayeeId", "dbo.BusinessPayee");
            DropForeignKey("dbo.BusinessPayeeWriteOff", "BusinessWriteOffId", "dbo.BusinessWriteOff");
            DropForeignKey("dbo.BusinessPayee", "ReceivablesId", "dbo.Receivables");
            DropForeignKey("dbo.BusinessPayee", "LinkManId", "dbo.LinkMan");
            DropForeignKey("dbo.PurchasePaymentDetail", "PurchasePaymentId", "dbo.PurchasePayment");
            DropForeignKey("dbo.PurchasePayment", "LinkManId", "dbo.LinkMan");
            DropForeignKey("dbo.PurchasePaymentDetail", "PurchaseOrderDetailId", "dbo.PurchaseOrderDetail");
            DropForeignKey("dbo.BillPayment", "LinkManId", "dbo.LinkMan");
            DropForeignKey("dbo.BillPaymentDetail", "SettleAccountId", "dbo.SettleAccount");
            DropForeignKey("dbo.BillPaymentDetail", "IncomeExpendId", "dbo.IncomeExpend");
            DropForeignKey("dbo.Receivables", "SettleAccountId", "dbo.SettleAccount");
            DropForeignKey("dbo.Receivables", "IncomeExpendId", "dbo.IncomeExpend");
            DropForeignKey("dbo.BillPaymentDetail", "BillPaymentId", "dbo.BillPayment");
            DropForeignKey("dbo.BusinessPayment", "BusinessPayeeId", "dbo.BusinessPayee");
            DropForeignKey("dbo.BusinessOrderWriteOff", "BusinessOrderId", "dbo.BusinessOrder");
            DropForeignKey("dbo.BusinessOrderWriteOff", "BusinessWriteOffId", "dbo.BusinessWriteOff");
            DropIndex("dbo.BusinessPayeeWriteOff", new[] { "BusinessPayeeId" });
            DropIndex("dbo.BusinessPayeeWriteOff", new[] { "BusinessWriteOffId" });
            DropIndex("dbo.BusinessOrderWriteOff", new[] { "BusinessOrderId" });
            DropIndex("dbo.BusinessOrderWriteOff", new[] { "BusinessWriteOffId" });
            DropIndex("dbo.PurchasePayment", new[] { "LinkManId" });
            DropIndex("dbo.PurchasePaymentDetail", new[] { "PurchasePaymentId" });
            DropIndex("dbo.PurchasePaymentDetail", new[] { "PurchaseOrderDetailId" });
            DropIndex("dbo.Receivables", new[] { "SettleAccountId" });
            DropIndex("dbo.Receivables", new[] { "IncomeExpendId" });
            DropIndex("dbo.BillPaymentDetail", new[] { "BillPaymentId" });
            DropIndex("dbo.BillPaymentDetail", new[] { "SettleAccountId" });
            DropIndex("dbo.BillPaymentDetail", new[] { "IncomeExpendId" });
            DropIndex("dbo.BillPayment", new[] { "LinkManId" });
            DropIndex("dbo.BusinessPayment", new[] { "BusinessPayeeId" });
            DropIndex("dbo.BusinessPayee", new[] { "LinkManId" });
            DropIndex("dbo.BusinessPayee", new[] { "ReceivablesId" });
            DropTable("dbo.BusinessPayeeWriteOff");
            DropTable("dbo.BusinessOrderWriteOff");
            DropTable("dbo.PurchasePayment");
            DropTable("dbo.PurchasePaymentDetail");
            DropTable("dbo.SettleAccount");
            DropTable("dbo.Receivables");
            DropTable("dbo.IncomeExpend");
            DropTable("dbo.BillPaymentDetail");
            DropTable("dbo.BillPayment");
            DropTable("dbo.BusinessPayment");
            DropTable("dbo.BusinessPayee");
            DropTable("dbo.BusinessWriteOff");
        }
    }
}
