using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CafeAutomation.Models
{
    public class Reservations
    {
        public int ID { get; set; }
        public int TableID { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public int GuestsCount { get; set; }
        public DateTime ReservationDate { get; set; }   
        public string Status { get; set; } 
    }
}
