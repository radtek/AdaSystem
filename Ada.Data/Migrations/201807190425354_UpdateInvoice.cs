namespace Ada.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateInvoice : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BusinessInvoiceReceivables",
                c => new
                    {
                        BusinessInvoiceId = c.String(nullable: false, maxLength: 128),
                        ReceivablesId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.BusinessInvoiceId, t.ReceivablesId })
                .ForeignKey("dbo.BusinessInvoice", t => t.BusinessInvoiceId, cascadeDelete: true)
                .ForeignKey("dbo.Receivables", t => t.ReceivablesId, cascadeDelete: true)
                .Index(t => t.BusinessInvoiceId)
                .Index(t => t.ReceivablesId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.BusinessInvoiceReceivables", "ReceivablesId", "dbo.Receivables");
            DropForeignKey("dbo.BusinessInvoiceReceivables", "BusinessInvoiceId", "dbo.BusinessInvoice");
            DropIndex("dbo.BusinessInvoiceReceivables", new[] { "ReceivablesId" });
            DropIndex("dbo.BusinessInvoiceReceivables", new[] { "BusinessInvoiceId" });
            DropTable("dbo.BusinessInvoiceReceivables");
        }
    }
}
