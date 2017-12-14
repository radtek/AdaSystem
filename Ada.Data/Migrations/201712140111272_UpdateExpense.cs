namespace Ada.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateExpense : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ExpenseDetail",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        IncomeExpendId = c.String(nullable: false, maxLength: 128),
                        IncomeExpendName = c.String(maxLength: 32),
                        SettleAccountName = c.String(maxLength: 32),
                        SettleType = c.String(maxLength: 32),
                        SettleAccountId = c.String(nullable: false, maxLength: 128),
                        SettleNum = c.String(maxLength: 32),
                        ExpenseId = c.String(nullable: false, maxLength: 128),
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
                .ForeignKey("dbo.Expense", t => t.ExpenseId, cascadeDelete: true)
                .ForeignKey("dbo.IncomeExpend", t => t.IncomeExpendId)
                .ForeignKey("dbo.SettleAccount", t => t.SettleAccountId)
                .Index(t => t.IncomeExpendId)
                .Index(t => t.SettleAccountId)
                .Index(t => t.ExpenseId);
            
            CreateTable(
                "dbo.Expense",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        IsIncom = c.Boolean(),
                        BillNum = c.String(maxLength: 32),
                        BillDate = c.DateTime(),
                        LinkManName = c.String(maxLength: 32),
                        LinkManId = c.String(maxLength: 32),
                        AccountBank = c.String(maxLength: 32),
                        AccountName = c.String(maxLength: 32),
                        AccountNum = c.String(maxLength: 32),
                        Transactor = c.String(maxLength: 32),
                        TransactorId = c.String(maxLength: 32),
                        Employe = c.String(maxLength: 32),
                        EmployerId = c.String(maxLength: 32),
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
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ExpenseDetail", "SettleAccountId", "dbo.SettleAccount");
            DropForeignKey("dbo.ExpenseDetail", "IncomeExpendId", "dbo.IncomeExpend");
            DropForeignKey("dbo.ExpenseDetail", "ExpenseId", "dbo.Expense");
            DropIndex("dbo.ExpenseDetail", new[] { "ExpenseId" });
            DropIndex("dbo.ExpenseDetail", new[] { "SettleAccountId" });
            DropIndex("dbo.ExpenseDetail", new[] { "IncomeExpendId" });
            DropTable("dbo.Expense");
            DropTable("dbo.ExpenseDetail");
        }
    }
}
