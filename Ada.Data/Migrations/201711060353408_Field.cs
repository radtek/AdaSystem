namespace Ada.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Field : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FieldType",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        TypeName = c.String(nullable: false, maxLength: 32),
                        CallIndex = c.String(maxLength: 32),
                        ParentId = c.String(maxLength: 128),
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
                "dbo.Field",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Text = c.String(nullable: false, maxLength: 128),
                        Value = c.String(maxLength: 128),
                        FieldTypeId = c.String(nullable: false, maxLength: 128),
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
                .ForeignKey("dbo.FieldType", t => t.FieldTypeId, cascadeDelete: true)
                .Index(t => t.FieldTypeId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Field", "FieldTypeId", "dbo.FieldType");
            DropIndex("dbo.Field", new[] { "FieldTypeId" });
            DropTable("dbo.Field");
            DropTable("dbo.FieldType");
        }
    }
}
