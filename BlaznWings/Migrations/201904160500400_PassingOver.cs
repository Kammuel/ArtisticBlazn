namespace BlaznWings.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PassingOver : DbMigration
    {
        public override void Up()
        {
            
            
            CreateTable(
                "dbo.PictureItems",
                c => new
                    {
                        PictureItemID = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Category = c.String(nullable: false),
                        Description = c.String(),
                        Url = c.String(),
                    })
                .PrimaryKey(t => t.PictureItemID);
            
            CreateTable(
                "dbo.VideoItems",
                c => new
                    {
                        VideoItemID = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Description = c.String(),
                        Category = c.String(),
                        Url = c.String(),
                    })
                .PrimaryKey(t => t.VideoItemID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.VideoItems");
            DropTable("dbo.PictureItems");
        }
    }
}
