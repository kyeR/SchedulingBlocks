using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SchedulingBlocks.Models.AppDb
{
    public class SqlDbContext : DbContext
    {
        public SqlDbContext()
            : base(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString)
        {
        }

        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<ReservedSlot> ReservedSlots { get; set; }
        public DbSet<Facility> Facilities { get; set; }
        public DbSet<Customer> Customers { get; set; }

        //protected override void OnModelCreating(DbModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<Reservation>().HasMany(r => r.ReservedSlots);
        //}
    }
}