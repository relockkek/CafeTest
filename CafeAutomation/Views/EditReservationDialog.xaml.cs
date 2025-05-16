using System.Windows;
using CafeAutomation.Models;

namespace CafeAutomation.Views
{
    public partial class EditReservationDialog : Window
    {
        public Reservations Reservation { get; private set; }

        public EditReservationDialog(Reservations reservation)
        {
            InitializeComponent();
            Reservation = reservation;
            DataContext = Reservation;
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}