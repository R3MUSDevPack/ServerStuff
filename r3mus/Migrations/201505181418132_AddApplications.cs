namespace r3mus.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddApplications : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Applicants",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        EmailAddress = c.String(),
                        ApiKey = c.Int(nullable: false),
                        VerificationCode = c.String(),
                        Information = c.String(),
                        Age = c.String(),
                        ToonAge = c.String(),
                        Source = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Applications",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Status = c.String(),
                        Notes = c.String(),
                        DateTimeCreated = c.DateTime(nullable: false),
                        Applicant_Id = c.Int(),
                        Reviewer_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Applicants", t => t.Applicant_Id)
                .ForeignKey("dbo.AspNetUsers", t => t.Reviewer_Id)
                .Index(t => t.Applicant_Id)
                .Index(t => t.Reviewer_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Applications", "Reviewer_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Applications", "Applicant_Id", "dbo.Applicants");
            DropIndex("dbo.Applications", new[] { "Reviewer_Id" });
            DropIndex("dbo.Applications", new[] { "Applicant_Id" });
            DropTable("dbo.Applications");
            DropTable("dbo.Applicants");
        }
    }
}
