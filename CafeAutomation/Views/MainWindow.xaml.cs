using System.Windows;
using CafeAutomation.Views;

namespace CafeAutomation
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Home_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new Home());
        }

        private void Menu_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new MenuPage());
        }

        private void Orders_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new OrdersPage());
        }

        private void Reservations_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ReservationsPage());
        }

        private void Employees_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new EmployeesPage());
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
