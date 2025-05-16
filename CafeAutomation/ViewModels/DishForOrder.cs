using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CafeAutomation.Models;

namespace CafeAutomation.ViewModels
{
    public class DishForOrder : BaseVM
    {
        public Dishes Dish { get; set; }
        private int quantity = 1;
        public int Quantity
        {
            get => quantity;
            set { quantity = value; Signal(); Signal(nameof(Total)); }
        }

        public decimal Total => Dish.Price * Quantity;
    }
}
