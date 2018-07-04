namespace Ada.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SaralyUpdate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AttendanceDetail",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Date = c.DateTime(),
                        OffWork = c.Double(nullable: false),
                        NoClockTimes = c.Int(nullable: false),
                        LateTimes = c.Int(nullable: false),
                        Absenteeism = c.Double(nullable: false),
                        Overtime = c.Int(nullable: false),
                        ManagerId = c.String(nullable: false, maxLength: 128),
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
                .ForeignKey("dbo.Manager", t => t.ManagerId)
                .Index(t => t.ManagerId);
            
            CreateTable(
                "dbo.SalaryDetail",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Date = c.DateTime(),
                        Total = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Commission = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SaleCommission = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Bonus = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DeductMoney = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AttendanceTotal = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ManagerId = c.String(nullable: false, maxLength: 128),
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
                .ForeignKey("dbo.Manager", t => t.ManagerId)
                .Index(t => t.ManagerId);
            
            CreateTable(
                "dbo.Quarters",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Title = c.String(nullable: false, maxLength: 128),
                        BaseSalary = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Allowance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Commission = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Attendance = c.Decimal(nullable: false, precision: 18, scale: 2),
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
            
            AddColumn("dbo.BusinessOrderDetail", "MediaUpdate", c => c.DateTime());
            AddColumn("dbo.BusinessOrderDetail", "UpdateContent", c => c.String(maxLength: 1024));
            AddColumn("dbo.BusinessOrder", "IsPic", c => c.Boolean());
            AddColumn("dbo.PurchaseOrderDetail", "Image", c => c.String(maxLength: 512));
            AddColumn("dbo.Manager", "PromotionDate", c => c.DateTime());
            AddColumn("dbo.Manager", "BankName", c => c.String(maxLength: 32));
            AddColumn("dbo.Manager", "BankNum", c => c.String(maxLength: 32));
            AddColumn("dbo.Manager", "BankAccount", c => c.String(maxLength: 32));
            AddColumn("dbo.Manager", "QuartersId", c => c.String(maxLength: 32));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AttendanceDetail", "ManagerId", "dbo.Manager");
            DropForeignKey("dbo.SalaryDetail", "ManagerId", "dbo.Manager");
            DropIndex("dbo.SalaryDetail", new[] { "ManagerId" });
            DropIndex("dbo.AttendanceDetail", new[] { "ManagerId" });
            DropColumn("dbo.Manager", "QuartersId");
            DropColumn("dbo.Manager", "BankAccount");
            DropColumn("dbo.Manager", "BankNum");
            DropColumn("dbo.Manager", "BankName");
            DropColumn("dbo.Manager", "PromotionDate");
            DropColumn("dbo.PurchaseOrderDetail", "Image");
            DropColumn("dbo.BusinessOrder", "IsPic");
            DropColumn("dbo.BusinessOrderDetail", "UpdateContent");
            DropColumn("dbo.BusinessOrderDetail", "MediaUpdate");
            DropTable("dbo.Quarters");
            DropTable("dbo.SalaryDetail");
            DropTable("dbo.AttendanceDetail");
        }
    }
}
