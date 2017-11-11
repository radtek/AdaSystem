namespace Ada.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MediaTrantor : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Media", "Transactor", c => c.String(maxLength: 32));
            AddColumn("dbo.Media", "TransactorId", c => c.String(maxLength: 32));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Media", "TransactorId");
            DropColumn("dbo.Media", "Transactor");
        }
    }
}
