using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SchedulingBlocks.Models.AppDb;
using System.Configuration;

namespace SchedulingBlocks.Models
{
    public class LocationDay
    {
        public Facility Location { get; set; }
        public DateTime Day { get; set; }
        public List<ReservedSlot> ReservedSlots { get; set; }
        public List<DateTime> SlotStartTimes
        {
            get
            {
                var startTimes = new List<DateTime>();
                for (DateTime slot = OpenTime; slot < CloseTime; slot = slot.AddMinutes(Int32.Parse(ConfigurationManager.AppSettings["SellingUnitMinutes"])))
                {
                    if (slot <= DateTime.Now.AddMinutes(30))
                    {
                        continue;
                    }
                    startTimes.Add(slot);
                }
                return startTimes;
            }
        }
        public List<DateTime> AvailableSlotStartTimes
        {
            get
            {
                var availableStartTimes = SlotStartTimes;
                var timesToRemove = new List<DateTime>();
                foreach (var slot in availableStartTimes)
                {
                    foreach (var reserved in ReservedSlots)
                    {
                        if (slot >= reserved.StartTime && slot < reserved.EndTime)
                        {
                            timesToRemove.Add(slot);
                            break;
                        }
                    }
                }
                foreach (var slot in timesToRemove)
                {
                    availableStartTimes.Remove(slot);
                }
                return availableStartTimes;
            }
        }
        public DateTime OpenTime
        {
            get
            {
                return Day.Date.AddHours(8);
            }
        }
        public DateTime CloseTime
        {
            get
            {
                return Day.Date.AddHours(22);
            }
        }
    }
}