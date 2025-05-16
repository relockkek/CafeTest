using System.Collections.ObjectModel;
using System.Windows;

namespace CafeAutomation.Views
{
    public partial class PaymentDialog : Window
    {
        public ObservableCollection<string> PaymentMethods { get; set; } =
            new ObservableCollection<string> { "Наличные", "Банковская карта"};

        public string SelectedMethod { get; set; }
        public string OrderDetails { get; set; }
        public bool PaymentConfirmed { get; private set; }

        public PaymentDialog(string orderInfo)
        {
            InitializeComponent();
            OrderDetails = orderInfo;
            DataContext = this;
        }

        private void Pay_Click(object sender, RoutedEventArgs e)
        {
            PaymentConfirmed = true;
            DialogResult = true;
        }
    }
}
