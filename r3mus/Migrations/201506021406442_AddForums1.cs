namespace r3mus.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddForums1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Forums", "Deleted", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Forums", "Deleted");
        }
    }
}
