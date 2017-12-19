namespace Ada.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BusinessInvoice : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BusinessInvoiceDetail",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        BusinessOrderId = c.String(nullable: false, maxLength: 128),
                        OrderMoney = c.Decimal(precision: 18, scale: 2),
                        InvoiceMoney = c.Decimal(precision: 18, scale: 2),
                        BusinessInvoiceId = c.String(nullable: false, maxLength: 128),
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
                .ForeignKey("dbo.BusinessInvoice", t => t.BusinessInvoiceId)
                .ForeignKey("dbo.BusinessOrder", t => t.BusinessOrderId)
                .Index(t => t.BusinessOrderId)
                .Index(t => t.BusinessInvoiceId);
            
            CreateTable(
                "dbo.BusinessInvoice",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        InvoiceTitle = c.String(maxLength: 32),
                        InvoiceType = c.String(maxLength: 32),
                        Company = c.String(maxLength: 32),
                        TaxNum = c.String(maxLength: 32),
                        Address = c.String(maxLength: 32),
                        Bank = c.String(maxLength: 32),
                        BankNum = c.String(maxLength: 32),
                        Phone = c.String(maxLength: 32),
                        Status = c.Short(),
                        MoneyStatus = c.Short(),
                        Transactor = c.String(maxLength: 32),
                        TransactorId = c.String(maxLength: 32),
                        TotalMoney = c.Decimal(precision: 18, scale: 2),
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
            DropForeignKey("dbo.BusinessInvoiceDetail", "BusinessOrderId", "dbo.BusinessOrder");
            DropForeignKey("dbo.BusinessInvoiceDetail", "BusinessInvoiceId", "dbo.BusinessInvoice");
            DropIndex("dbo.BusinessInvoiceDetail", new[] { "BusinessInvoiceId" });
            DropIndex("dbo.BusinessInvoiceDetail", new[] { "BusinessOrderId" });
            DropTable("dbo.BusinessInvoice");
            DropTable("dbo.BusinessInvoiceDetail");
        }
    }
}
