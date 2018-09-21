namespace SchedulingBlocks.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedFacility : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Facilities",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FacilityName = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Facilities");
        }
    }
}
