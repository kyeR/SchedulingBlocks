using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SchedulingBlocks.Models.AppDb
{
    public class ReservedSlot
    {
        [NotMapped]
        private int _sellingUnitMinutes;

        [Key]
        public int Id { get; set; }

        [Required]
        public string Facility { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        public Reservation Reservation { get; set; }

        [NotMapped]
        public int TotalSlotMinutes
        {
            get
            {
                var span = EndTime - StartTime;
                return Convert.ToInt32(span.TotalMinutes);
            }
        }

        [NotMapped]
        public int SellingUnitMinutes
        {
            get
            {
                if (_sellingUnitMinutes != 0)
                {
                    return _sellingUnitMinutes;
                }

                _sellingUnitMinutes = Int32.Parse(ConfigurationManager.AppSettings["SellingUnitMinutes"]);
                return _sellingUnitMinutes;
            }
        }

        [NotMapped]
        public DateTime DayOfReservation
        {
            get { return StartTime.Date; }
        }

        public double GetSlotPrice(double unitPrice)
        {
            return (TotalSlotMinutes / SellingUnitMinutes) * unitPrice;
        }

        public string ToLineItemString()
        {
            return Facility + ": " + StartTime.ToString("MMMM dd h:mmtt") + " - " + EndTime.ToString("h:mmtt");
        }

        [NotMapped]
        public string CartItemId
        {
            get { return Facility + StartTime; }
        }
    }
}