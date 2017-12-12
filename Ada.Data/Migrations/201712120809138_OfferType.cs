namespace Ada.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OfferType : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BusinessOffer", "OfferType", c => c.Short());
        }
        
        public override void Down()
        {
            DropColumn("dbo.BusinessOffer", "OfferType");
        }
    }
}
