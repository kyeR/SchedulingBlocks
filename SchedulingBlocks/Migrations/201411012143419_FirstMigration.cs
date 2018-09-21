namespace SchedulingBlocks.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FirstMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Reservations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Status = c.String(nullable: false),
                        SubmittedTimestamp = c.DateTime(nullable: false),
                        PaymentCompletedTimestamp = c.DateTime(nullable: false),
                        TransactionId = c.String(),
                        AmountPaid = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CustomerInfo_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Customers", t => t.CustomerInfo_Id)
                .Index(t => t.CustomerInfo_Id);
            
            CreateTable(
                "dbo.Customers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FirstName = c.String(nullable: false),
                        LastName = c.String(nullable: false),
                        Email = c.String(nullable: false),
                        Sport = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ReservedSlots",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Facility = c.String(nullable: false),
                        StartTime = c.DateTime(nullable: false),
                        EndTime = c.DateTime(nullable: false),
                        Reservation_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Reservations", t => t.Reservation_Id)
                .Index(t => t.Reservation_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ReservedSlots", "Reservation_Id", "dbo.Reservations");
            DropForeignKey("dbo.Reservations", "CustomerInfo_Id", "dbo.Customers");
            DropIndex("dbo.ReservedSlots", new[] { "Reservation_Id" });
            DropIndex("dbo.Reservations", new[] { "CustomerInfo_Id" });
            DropTable("dbo.ReservedSlots");
            DropTable("dbo.Customers");
            DropTable("dbo.Reservations");
        }
    }
}
