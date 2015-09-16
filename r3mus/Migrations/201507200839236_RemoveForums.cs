namespace r3mus.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveForums : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Threads", "ForumID", "dbo.Forums");
            DropForeignKey("dbo.Posts", "ThreadId", "dbo.Threads");
            DropForeignKey("dbo.Posts", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Threads", "User_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Threads", new[] { "ForumID" });
            DropIndex("dbo.Threads", new[] { "User_Id" });
            DropIndex("dbo.Posts", new[] { "ThreadId" });
            DropIndex("dbo.Posts", new[] { "UserId" });
            DropTable("dbo.Forums");
            DropTable("dbo.Threads");
            DropTable("dbo.Posts");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Posts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        Body = c.String(),
                        PostedAt = c.DateTime(nullable: false, storeType: "smalldatetime"),
                        ThreadId = c.Int(nullable: false),
                        UserId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Threads",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ForumID = c.Int(nullable: false),
                        CreatorId = c.String(),
                        Created = c.DateTime(nullable: false),
                        Title = c.String(),
                        User_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Forums",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        MinimumRole = c.String(),
                        CreatorId = c.String(),
                        Created = c.DateTime(nullable: false),
                        Deleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateIndex("dbo.Posts", "UserId");
            CreateIndex("dbo.Posts", "ThreadId");
            CreateIndex("dbo.Threads", "User_Id");
            CreateIndex("dbo.Threads", "ForumID");
            AddForeignKey("dbo.Threads", "User_Id", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.Posts", "UserId", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.Posts", "ThreadId", "dbo.Threads", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Threads", "ForumID", "dbo.Forums", "Id", cascadeDelete: true);
        }
    }
}
