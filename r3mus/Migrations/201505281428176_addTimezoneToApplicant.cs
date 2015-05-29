namespace r3mus.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addTimezoneToApplicant : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Applicants", "TimeZone", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Applicants", "TimeZone");
        }
    }
}
