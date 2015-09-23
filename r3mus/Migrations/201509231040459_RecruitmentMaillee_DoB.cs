namespace r3mus.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RecruitmentMaillee_DoB : DbMigration
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
