using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using CafeAutomation.ViewModels;
using CafeAutomation.Converters;
namespace CafeAutomation.Views
{
    public partial class CreateOrderDialog : Window
    {
        public CreateOrderDialog()
        {
            InitializeComponent();

            Resources["ImageFromBytes"] = new CafeAutomation.Converters.ByteArrayToImageConverter();

            DataContext = new CreateOrderVM();
        }
    }
}