using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CafeAutomation.Models
{
    public class Orders
    {
        public int ID { get; set; }
        public int EmployeeID { get; set; }
        public int TableNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public int StatusID { get; set; }
        public string Display => $"Заказ #{ID} • Стол {TableNumber} • {OrderDate:g}";

    }
}
