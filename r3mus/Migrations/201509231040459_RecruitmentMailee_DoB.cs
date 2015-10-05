namespace r3mus.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RecruitmentMailee_DoB : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RecruitmentMailees", "DateOfBirth", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.RecruitmentMailees", "DateOfBirth");
        }
    }
}
