using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SchedulingBlocks.Models.AppDb
{
    public class Facility
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string FacilityName { get; set; }
    }
}