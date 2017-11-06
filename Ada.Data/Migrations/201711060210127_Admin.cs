namespace Ada.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Admin : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ManagerLoginLog", "AddedById", c => c.String(maxLength: 32));
            AddColumn("dbo.ManagerLoginLog", "ModifiedById", c => c.String(maxLength: 32));
            AddColumn("dbo.ManagerLoginLog", "DeletedById", c => c.String(maxLength: 32));
            AddColumn("dbo.Manager", "AddedById", c => c.String(maxLength: 32));
            AddColumn("dbo.Manager", "ModifiedById", c => c.String(maxLength: 32));
            AddColumn("dbo.Manager", "DeletedById", c => c.String(maxLength: 32));
            AddColumn("dbo.ManagerAction", "AddedById", c => c.String(maxLength: 32));
            AddColumn("dbo.ManagerAction", "ModifiedById", c => c.String(maxLength: 32));
            AddColumn("dbo.ManagerAction", "DeletedById", c => c.String(maxLength: 32));
            AddColumn("dbo.Action", "AddedById", c => c.String(maxLength: 32));
            AddColumn("dbo.Action", "ModifiedById", c => c.String(maxLength: 32));
            AddColumn("dbo.Action", "DeletedById", c => c.String(maxLength: 32));
            AddColumn("dbo.Role", "AddedById", c => c.String(maxLength: 32));
            AddColumn("dbo.Role", "ModifiedById", c => c.String(maxLength: 32));
            AddColumn("dbo.Role", "DeletedById", c => c.String(maxLength: 32));
            AddColumn("dbo.Organization", "AddedById", c => c.String(maxLength: 32));
            AddColumn("dbo.Organization", "ModifiedById", c => c.String(maxLength: 32));
            AddColumn("dbo.Organization", "DeletedById", c => c.String(maxLength: 32));
            AddColumn("dbo.Menu", "AddedById", c => c.String(maxLength: 32));
            AddColumn("dbo.Menu", "ModifiedById", c => c.String(maxLength: 32));
            AddColumn("dbo.Menu", "DeletedById", c => c.String(maxLength: 32));
            AlterColumn("dbo.Action", "TreePath", c => c.String(maxLength: 1024));
            AlterColumn("dbo.Organization", "TreePath", c => c.String(maxLength: 1024));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Organization", "TreePath", c => c.String(maxLength: 32));
            AlterColumn("dbo.Action", "TreePath", c => c.String(maxLength: 32));
            DropColumn("dbo.Menu", "DeletedById");
            DropColumn("dbo.Menu", "ModifiedById");
            DropColumn("dbo.Menu", "AddedById");
            DropColumn("dbo.Organization", "DeletedById");
            DropColumn("dbo.Organization", "ModifiedById");
            DropColumn("dbo.Organization", "AddedById");
            DropColumn("dbo.Role", "DeletedById");
            DropColumn("dbo.Role", "ModifiedById");
            DropColumn("dbo.Role", "AddedById");
            DropColumn("dbo.Action", "DeletedById");
            DropColumn("dbo.Action", "ModifiedById");
            DropColumn("dbo.Action", "AddedById");
            DropColumn("dbo.ManagerAction", "DeletedById");
            DropColumn("dbo.ManagerAction", "ModifiedById");
            DropColumn("dbo.ManagerAction", "AddedById");
            DropColumn("dbo.Manager", "DeletedById");
            DropColumn("dbo.Manager", "ModifiedById");
            DropColumn("dbo.Manager", "AddedById");
            DropColumn("dbo.ManagerLoginLog", "DeletedById");
            DropColumn("dbo.ManagerLoginLog", "ModifiedById");
            DropColumn("dbo.ManagerLoginLog", "AddedById");
        }
    }
}
