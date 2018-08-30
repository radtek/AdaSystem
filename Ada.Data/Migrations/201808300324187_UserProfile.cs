namespace Ada.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserProfile : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.WorkFlowActivity",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 32),
                        X = c.Int(),
                        Y = c.Int(),
                        Parameter = c.String(),
                        IsStart = c.Boolean(nullable: false),
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
                "dbo.WorkFlowDefinition",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 32),
                        Enabled = c.Boolean(nullable: false),
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
                "dbo.WorkFlowsTransition",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        SourceEndpoint = c.String(maxLength: 32),
                        DestinationEndpoint = c.String(maxLength: 32),
                        SourceActivityId = c.String(maxLength: 32),
                        DestinationActivityId = c.String(maxLength: 32),
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
                "dbo.Article",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Title = c.String(nullable: false, maxLength: 128),
                        ColumnId = c.String(nullable: false, maxLength: 128),
                        Url = c.String(maxLength: 512),
                        CoverPic = c.String(maxLength: 512),
                        Summary = c.String(maxLength: 256),
                        Content = c.String(),
                        Author = c.String(maxLength: 32),
                        Source = c.String(maxLength: 32),
                        Click = c.Int(),
                        Status = c.Short(),
                        IsComment = c.Boolean(),
                        IsHot = c.Boolean(),
                        IsTop = c.Boolean(),
                        IsRecommend = c.Boolean(),
                        IsSlide = c.Boolean(),
                        IsPush = c.Boolean(),
                        Type = c.String(maxLength: 32),
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
                .ForeignKey("dbo.Column", t => t.ColumnId)
                .Index(t => t.ColumnId);
            
            CreateTable(
                "dbo.Attachment",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Title = c.String(maxLength: 128),
                        Describe = c.String(maxLength: 512),
                        Path = c.String(maxLength: 512),
                        FileSize = c.Int(),
                        FileExt = c.String(maxLength: 8),
                        Times = c.Int(),
                        ThumbPath = c.String(maxLength: 512),
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
                "dbo.Column",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Title = c.String(nullable: false, maxLength: 32),
                        ParentId = c.String(maxLength: 32),
                        IsLeaf = c.Boolean(),
                        Level = c.Int(),
                        TreePath = c.String(maxLength: 1024),
                        Content = c.String(),
                        CoverPic = c.String(maxLength: 512),
                        Url = c.String(maxLength: 512),
                        IsTop = c.Short(),
                        CallIndex = c.String(maxLength: 32),
                        Type = c.String(maxLength: 32),
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
                "dbo.Feedback",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Title = c.String(maxLength: 128),
                        Content = c.String(),
                        Contact = c.String(maxLength: 128),
                        Name = c.String(maxLength: 32),
                        SubTime = c.DateTime(),
                        Type = c.String(maxLength: 32),
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
                "dbo.Fans",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        NickName = c.String(nullable: false, maxLength: 64),
                        Type = c.String(maxLength: 32),
                        Avatar = c.String(maxLength: 512),
                        Cover = c.String(maxLength: 512),
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
                "dbo.ArticleAttachment",
                c => new
                    {
                        ArticleId = c.String(nullable: false, maxLength: 128),
                        AttachmentId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.ArticleId, t.AttachmentId })
                .ForeignKey("dbo.Article", t => t.ArticleId, cascadeDelete: true)
                .ForeignKey("dbo.Attachment", t => t.AttachmentId, cascadeDelete: true)
                .Index(t => t.ArticleId)
                .Index(t => t.AttachmentId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Article", "ColumnId", "dbo.Column");
            DropForeignKey("dbo.ArticleAttachment", "AttachmentId", "dbo.Attachment");
            DropForeignKey("dbo.ArticleAttachment", "ArticleId", "dbo.Article");
            DropForeignKey("dbo.WorkFlowActivity", "WorkFlowDefinitionId", "dbo.WorkFlowDefinition");
            DropForeignKey("dbo.WorkFlowsTransition", "WorkFlowDefinitionId", "dbo.WorkFlowDefinition");
            DropIndex("dbo.ArticleAttachment", new[] { "AttachmentId" });
            DropIndex("dbo.ArticleAttachment", new[] { "ArticleId" });
            DropIndex("dbo.Article", new[] { "ColumnId" });
            DropIndex("dbo.WorkFlowsTransition", new[] { "WorkFlowDefinitionId" });
            DropIndex("dbo.WorkFlowActivity", new[] { "WorkFlowDefinitionId" });
            DropTable("dbo.ArticleAttachment");
            DropTable("dbo.Fans");
            DropTable("dbo.Feedback");
            DropTable("dbo.Column");
            DropTable("dbo.Attachment");
            DropTable("dbo.Article");
            DropTable("dbo.WorkFlowsTransition");
            DropTable("dbo.WorkFlowDefinition");
            DropTable("dbo.WorkFlowActivity");
        }
    }
}
