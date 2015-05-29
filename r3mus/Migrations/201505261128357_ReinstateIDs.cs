namespace r3mus.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ReinstateIDs : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.ApiInfoes");
            DropPrimaryKey("dbo.Titles");
            AddColumn("dbo.ApiInfoes", "Id", c => c.Int(nullable: false, identity: true));
            AddColumn("dbo.Titles", "Id", c => c.Int(nullable: false, identity: true));
            AlterColumn("dbo.ApiInfoes", "ApiKey", c => c.Int(nullable: false));
            AlterColumn("dbo.Titles", "UserId", c => c.String());
            AlterColumn("dbo.Titles", "TitleName", c => c.String());
            AddPrimaryKey("dbo.ApiInfoes", "Id");
            AddPrimaryKey("dbo.Titles", "Id");
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.Titles");
            DropPrimaryKey("dbo.ApiInfoes");
            AlterColumn("dbo.Titles", "TitleName", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.Titles", "UserId", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.ApiInfoes", "ApiKey", c => c.Int(nullable: false, identity: true));
            DropColumn("dbo.Titles", "Id");
            DropColumn("dbo.ApiInfoes", "Id");
            AddPrimaryKey("dbo.Titles", new[] { "UserId", "TitleName" });
            AddPrimaryKey("dbo.ApiInfoes", "ApiKey");
        }
    }
}
