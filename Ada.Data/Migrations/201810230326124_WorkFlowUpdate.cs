namespace Ada.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class WorkFlowUpdate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.WorkFlowRecord",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Title = c.String(nullable: false, maxLength: 128),
                        Content = c.String(),
                        Level = c.Short(),
                        WfInstanceId = c.String(maxLength: 32),
                        WorkFlowDefinitionId = c.String(nullable: false, maxLength: 128),
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
                .ForeignKey("dbo.WorkFlowDefinition", t => t.WorkFlowDefinitionId, cascadeDelete: true)
                .Index(t => t.WorkFlowDefinitionId);
            
            CreateTable(
                "dbo.WorkFlowRecordDetail",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(maxLength: 128),
                        ProcessBy = c.String(maxLength: 32),
                        ProcessById = c.String(maxLength: 32),
                        ProcessDate = c.DateTime(),
                        ProcessResult = c.String(maxLength: 256),
                        ProcessComment = c.String(maxLength: 512),
                        Status = c.Short(),
                        IsStart = c.Boolean(),
                        IsEnd = c.Boolean(),
                        ParentDetailId = c.String(maxLength: 32),
                        WorkFlowRecordId = c.String(nullable: false, maxLength: 128),
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
                .ForeignKey("dbo.WorkFlowRecord", t => t.WorkFlowRecordId, cascadeDelete: true)
                .Index(t => t.WorkFlowRecordId);
            
            CreateTable(
                "dbo.WorkFlowRecordAttachment",
                c => new
                    {
                        WorkFlowRecordId = c.String(nullable: false, maxLength: 128),
                        AttachmentId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.WorkFlowRecordId, t.AttachmentId })
                .ForeignKey("dbo.WorkFlowRecord", t => t.WorkFlowRecordId, cascadeDelete: true)
                .ForeignKey("dbo.Attachment", t => t.AttachmentId, cascadeDelete: true)
                .Index(t => t.WorkFlowRecordId)
                .Index(t => t.AttachmentId);
            
            AddColumn("dbo.WorkFlowDefinition", "Description", c => c.String());
            AddColumn("dbo.WorkFlowDefinition", "TempForm", c => c.String());
            AddColumn("dbo.WorkFlowDefinition", "ActityType", c => c.String(maxLength: 512));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.WorkFlowRecordDetail", "WorkFlowRecordId", "dbo.WorkFlowRecord");
            DropForeignKey("dbo.WorkFlowRecord", "WorkFlowDefinitionId", "dbo.WorkFlowDefinition");
            DropForeignKey("dbo.WorkFlowRecordAttachment", "AttachmentId", "dbo.Attachment");
            DropForeignKey("dbo.WorkFlowRecordAttachment", "WorkFlowRecordId", "dbo.WorkFlowRecord");
            DropIndex("dbo.WorkFlowRecordAttachment", new[] { "AttachmentId" });
            DropIndex("dbo.WorkFlowRecordAttachment", new[] { "WorkFlowRecordId" });
            DropIndex("dbo.WorkFlowRecordDetail", new[] { "WorkFlowRecordId" });
            DropIndex("dbo.WorkFlowRecord", new[] { "WorkFlowDefinitionId" });
            DropColumn("dbo.WorkFlowDefinition", "ActityType");
            DropColumn("dbo.WorkFlowDefinition", "TempForm");
            DropColumn("dbo.WorkFlowDefinition", "Description");
            DropTable("dbo.WorkFlowRecordAttachment");
            DropTable("dbo.WorkFlowRecordDetail");
            DropTable("dbo.WorkFlowRecord");
        }
    }
}
