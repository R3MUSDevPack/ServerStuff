namespace r3mus.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeApplicationApplicantToId : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Applications", "Applicant_Id", "dbo.Applicants");
            DropIndex("dbo.Applications", new[] { "Applicant_Id" });
            AddColumn("dbo.Applications", "ApplicantId", c => c.Int(nullable: false));
            DropColumn("dbo.Applications", "Applicant_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Applications", "Applicant_Id", c => c.Int());
            DropColumn("dbo.Applications", "ApplicantId");
            CreateIndex("dbo.Applications", "Applicant_Id");
            AddForeignKey("dbo.Applications", "Applicant_Id", "dbo.Applicants", "Id");
        }
    }
}
