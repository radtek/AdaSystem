namespace Ada.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Quartz : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Job",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        GroupName = c.String(maxLength: 128),
                        JobName = c.String(maxLength: 32),
                        JobType = c.String(maxLength: 32),
                        TriggerName = c.String(maxLength: 32),
                        Cron = c.String(maxLength: 32),
                        TriggerState = c.String(maxLength: 32),
                        StartTime = c.DateTime(),
                        EndTime = c.DateTime(),
                        PreTime = c.DateTime(),
                        NextTime = c.DateTime(),
                        AppId = c.String(maxLength: 64),
                        ApiUrl = c.String(maxLength: 512),
                        Params = c.String(maxLength: 512),
                        Token = c.String(maxLength: 64),
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
            DropTable("dbo.Job");
        }
    }
}
