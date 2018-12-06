namespace Ada.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserProfile20181606 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BusinessWriteOffDetail",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        PublishDate = c.DateTime(),
                        SellMoney = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CostMoney = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Profit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Percentage = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Commission = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MoneyBackDay = c.Int(nullable: false),
                        BusinessOrderDetailId = c.String(maxLength: 32),
                        BusinessOrderId = c.String(maxLength: 32),
                        MediaTypeId = c.String(maxLength: 32),
                        BusinessWriteOffId = c.String(nullable: false, maxLength: 128),
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
                .ForeignKey("dbo.BusinessWriteOff", t => t.BusinessWriteOffId, cascadeDelete: true)
                .Index(t => t.BusinessWriteOffId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.BusinessWriteOffDetail", "BusinessWriteOffId", "dbo.BusinessWriteOff");
            DropIndex("dbo.BusinessWriteOffDetail", new[] { "BusinessWriteOffId" });
            DropTable("dbo.BusinessWriteOffDetail");
        }
    }
}
