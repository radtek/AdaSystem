namespace Ada.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdataJob : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.JobDetail",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Num1 = c.Int(nullable: false),
                        Num2 = c.Int(nullable: false),
                        Num3 = c.Int(nullable: false),
                        RequestDate = c.DateTime(),
                        ReponseDate = c.DateTime(),
                        ReponseContent = c.String(),
                        Retcode = c.String(maxLength: 32),
                        Retmsg = c.String(),
                        IsSuccess = c.Boolean(),
                        JobId = c.String(nullable: false, maxLength: 128),
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
                .ForeignKey("dbo.Job", t => t.JobId, cascadeDelete: true)
                .Index(t => t.JobId);
            
            AddColumn("dbo.Job", "Type", c => c.Short());
            AddColumn("dbo.Job", "Repetitions", c => c.Short());
            AddColumn("dbo.Job", "IsLog", c => c.Boolean());
            AddColumn("dbo.Job", "TimeOut", c => c.Int());
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.JobDetail", "JobId", "dbo.Job");
            DropIndex("dbo.JobDetail", new[] { "JobId" });
            DropColumn("dbo.Job", "TimeOut");
            DropColumn("dbo.Job", "IsLog");
            DropColumn("dbo.Job", "Repetitions");
            DropColumn("dbo.Job", "Type");
            DropTable("dbo.JobDetail");
        }
    }
}
