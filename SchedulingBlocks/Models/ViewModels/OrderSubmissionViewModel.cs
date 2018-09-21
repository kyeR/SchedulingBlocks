using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SchedulingBlocks.Models.ViewModels
{
    public class OrderSubmissionViewModel
    {
        public int ReservationId { get; set; }
        public string ReservationEmail { get; set; }
    }
}