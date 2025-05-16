using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CafeAutomation.Models
{
    public class OrderItems
    {
        public int ID { get; set; }
        public int OrderID { get; set; }
        public int DishID { get; set; }
        public int Amount { get; set; }
        public decimal PriceAtOrderTime { get; set; }
    }
}
