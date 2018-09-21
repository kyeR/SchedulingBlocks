using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SchedulingBlocks.Models.AppDb;

namespace SchedulingBlocks.Models
{
    public class CartModel
    {
        public List<ReservedSlot> Slots { get; set; }
    }
}