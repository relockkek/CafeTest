using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CafeAutomation.Models
{
    public class Tables
    {
        public int ID { get; set; }
        public int TableNumber { get; set; }
        public int Capacity { get; set; }
        public string Zone { get; set; }
        public bool IsActive { get; set; } 
    }
}
