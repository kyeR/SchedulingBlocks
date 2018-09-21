using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;
using System.Web.Mvc;
using System.Configuration;

namespace SchedulingBlocks.Models.AppDb
{
    public class Reservation
    {
        [Key]
        public int Id { get; set; }

        public Customer CustomerInfo { get; set; }
    
        public List<ReservedSlot> ReservedSlots { get; set; }

        //public virtual List<ReservedSlot> ReservedSlots
        //{
        //    get { return _ReservedSlots = _ReservedSlots ?? new List<ReservedSlot>(); }
        //    set { _ReservedSlots = value; }
        //}

        [NotMapped]
        public double PricePer { get; set; }

        [NotMapped]
        public decimal OrderTotal
        {
            get
            {
                return ReservedSlots.Sum(slot => (decimal)slot.GetSlotPrice(PricePer));
            }
        }

        [NotMapped]
        public int TotalUnits
        {
            get 
            { 
                var total = ReservedSlots.Sum(slot => slot.TotalSlotMinutes);

                return total / Int32.Parse(ConfigurationManager.AppSettings["SellingUnitMinutes"]);
            }
        }

        [Required]
        public string Status { get; set; }

        public DateTime SubmittedTimestamp { get; set; }

        public DateTime PaymentCompletedTimestamp { get; set; }

        public string TransactionId { get; set; }

        public decimal AmountPaid { get; set; }
    }
}