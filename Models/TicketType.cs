﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TheBugTracker.Models
{
    public class TicketType
    {
        public int Id { get; set; }
        
        [Display(Name = "Type Name")]
        public string Name { get; set; }
    }
}
