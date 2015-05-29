namespace r3mus.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Major1 : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.ApiInfoes");
            CreateTable(
                "dbo.Titles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        TitleName = c.String(nullable: false, maxLength: 128),
                        ApplicationUser_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.TitleName })
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUser_Id)
                .Index(t => t.ApplicationUser_Id);
            
            AddColumn("dbo.AspNetUsers", "MemberSince", c => c.DateTime());
            AddColumn("dbo.AspNetUsers", "Avatar", c => c.String());
            AddColumn("dbo.RecruitmentMailees", "CorpId_AtLastCheck", c => c.Long(nullable: false));
            AlterColumn("dbo.ApiInfoes", "ApiKey", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.ApiInfoes", "ApiKey");
            DropColumn("dbo.ApiInfoes", "Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ApiInfoes", "Id", c => c.Int(nullable: false, identity: true));
            DropForeignKey("dbo.Titles", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Titles", new[] { "ApplicationUser_Id" });
            DropPrimaryKey("dbo.ApiInfoes");
            AlterColumn("dbo.ApiInfoes", "ApiKey", c => c.Int(nullable: false));
            DropColumn("dbo.RecruitmentMailees", "CorpId_AtLastCheck");
            DropColumn("dbo.AspNetUsers", "Avatar");
            DropColumn("dbo.AspNetUsers", "MemberSince");
            DropTable("dbo.Titles");
            AddPrimaryKey("dbo.ApiInfoes", "Id");
        }
    }
}
