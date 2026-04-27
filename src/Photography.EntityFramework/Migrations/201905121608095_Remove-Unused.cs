namespace Photography.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveUnused : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.Album", newName: "Albums");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.Albums", newName: "Album");
        }
    }
}
