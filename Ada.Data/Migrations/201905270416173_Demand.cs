namespace Ada.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Demand : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SubjectDetailProgress",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        UploadDate = c.DateTime(),
                        SubjectDetailId = c.String(nullable: false, maxLength: 128),
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
                .ForeignKey("dbo.SubjectDetail", t => t.SubjectDetailId, cascadeDelete: true)
                .Index(t => t.SubjectDetailId);
            
            CreateTable(
                "dbo.SubjectDetail",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Title = c.String(maxLength: 32),
                        Type = c.String(maxLength: 32),
                        Transactor = c.String(maxLength: 32),
                        TransactorId = c.String(maxLength: 32),
                        GetDate = c.DateTime(),
                        Status = c.Short(),
                        ProducerBy = c.String(maxLength: 32),
                        ProducerById = c.String(maxLength: 32),
                        ProducerDate = c.DateTime(),
                        CompletDate = c.DateTime(),
                        SubjectId = c.String(nullable: false, maxLength: 128),
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
                .ForeignKey("dbo.Subject", t => t.SubjectId, cascadeDelete: true)
                .Index(t => t.SubjectId);
            
            CreateTable(
                "dbo.Subject",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Title = c.String(maxLength: 128),
                        Content = c.String(maxLength: 1024),
                        Type = c.String(maxLength: 32),
                        Status = c.Short(),
                        Offer = c.Decimal(nullable: false, precision: 18, scale: 2),
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
                "dbo.MediaReferencePrice",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Platform = c.String(maxLength: 32),
                        PriceName = c.String(maxLength: 32),
                        Offer = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OfferDate = c.DateTime(),
                        MediaId = c.String(nullable: false, maxLength: 128),
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
                .ForeignKey("dbo.Media", t => t.MediaId, cascadeDelete: true)
                .Index(t => t.MediaId);
            
            CreateTable(
                "dbo.SubjectDetailProgressAttachment",
                c => new
                    {
                        SubjectDetailProgressId = c.String(nullable: false, maxLength: 128),
                        AttachmentId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.SubjectDetailProgressId, t.AttachmentId })
                .ForeignKey("dbo.SubjectDetailProgress", t => t.SubjectDetailProgressId, cascadeDelete: true)
                .ForeignKey("dbo.Attachment", t => t.AttachmentId, cascadeDelete: true)
                .Index(t => t.SubjectDetailProgressId)
                .Index(t => t.AttachmentId);
            
            CreateTable(
                "dbo.SubjectAttachment",
                c => new
                    {
                        SubjectId = c.String(nullable: false, maxLength: 128),
                        AttachmentId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.SubjectId, t.AttachmentId })
                .ForeignKey("dbo.Subject", t => t.SubjectId, cascadeDelete: true)
                .ForeignKey("dbo.Attachment", t => t.AttachmentId, cascadeDelete: true)
                .Index(t => t.SubjectId)
                .Index(t => t.AttachmentId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MediaReferencePrice", "MediaId", "dbo.Media");
            DropForeignKey("dbo.SubjectDetailProgress", "SubjectDetailId", "dbo.SubjectDetail");
            DropForeignKey("dbo.SubjectDetail", "SubjectId", "dbo.Subject");
            DropForeignKey("dbo.SubjectAttachment", "AttachmentId", "dbo.Attachment");
            DropForeignKey("dbo.SubjectAttachment", "SubjectId", "dbo.Subject");
            DropForeignKey("dbo.SubjectDetailProgressAttachment", "AttachmentId", "dbo.Attachment");
            DropForeignKey("dbo.SubjectDetailProgressAttachment", "SubjectDetailProgressId", "dbo.SubjectDetailProgress");
            DropIndex("dbo.SubjectAttachment", new[] { "AttachmentId" });
            DropIndex("dbo.SubjectAttachment", new[] { "SubjectId" });
            DropIndex("dbo.SubjectDetailProgressAttachment", new[] { "AttachmentId" });
            DropIndex("dbo.SubjectDetailProgressAttachment", new[] { "SubjectDetailProgressId" });
            DropIndex("dbo.MediaReferencePrice", new[] { "MediaId" });
            DropIndex("dbo.SubjectDetail", new[] { "SubjectId" });
            DropIndex("dbo.SubjectDetailProgress", new[] { "SubjectDetailId" });
            DropTable("dbo.SubjectAttachment");
            DropTable("dbo.SubjectDetailProgressAttachment");
            DropTable("dbo.MediaReferencePrice");
            DropTable("dbo.Subject");
            DropTable("dbo.SubjectDetail");
            DropTable("dbo.SubjectDetailProgress");
        }
    }
}
