namespace Ada.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CustomerUpdate20171215 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.LinkMan", "Transactor", c => c.String(maxLength: 32));
            AddColumn("dbo.LinkMan", "TransactorId", c => c.String(maxLength: 32));
            AddColumn("dbo.Commpany", "Transactor", c => c.String(maxLength: 32));
            AddColumn("dbo.Commpany", "TransactorId", c => c.String(maxLength: 32));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Commpany", "TransactorId");
            DropColumn("dbo.Commpany", "Transactor");
            DropColumn("dbo.LinkMan", "TransactorId");
            DropColumn("dbo.LinkMan", "Transactor");
        }
    }
}
