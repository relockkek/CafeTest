using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CafeAutomation.Models
{
    public class Employees
    {
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Patronymic { get; set; }
        public string Position { get; set; }
        public string Phone { get; set; }
        public DateTime? HireDate { get; set; }
        public decimal Salary { get; set; }
    }
}
