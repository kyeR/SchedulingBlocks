namespace SchedulingBlocks.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CustomerPhone : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Customers", "Phone", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Customers", "Phone");
        }
    }
}
