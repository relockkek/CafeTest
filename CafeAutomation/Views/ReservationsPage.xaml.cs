using System.Windows;
using System.Windows.Controls;
using CafeAutomation.Models;
using CafeAutomation.DB;
using CafeAutomation.ViewModels;

namespace CafeAutomation.Views
{
    public partial class ReservationsPage : Page
    {
        public ReservationsPage()
        {
            InitializeComponent();
        }

        private async void AddReservation_Click(object sender, RoutedEventArgs e)
        {
            var res = new Reservations();
            var dialog = new EditReservationDialog(res);
            if (dialog.ShowDialog() == true)
            {
                if (ReservationsDB.GetDb().Insert(dialog.Reservation))
                {
                    await ((ReservationsMVVM)DataContext).LoadDataAsync();
                }
            }
        }

        private async void EditReservation_Click(object sender, RoutedEventArgs e)
        {
            if (((ReservationsMVVM)DataContext).SelectedReservation is Reservations selected)
            {
                var dialog = new EditReservationDialog(new Reservations
                {
                    ID = selected.ID,
                    TableID = selected.TableID,
                    CustomerName = selected.CustomerName,
                    CustomerPhone = selected.CustomerPhone,
                    GuestsCount = selected.GuestsCount,
                    ReservationDate = selected.ReservationDate,
                    Status = selected.Status
                });

                if (dialog.ShowDialog() == true)
                {
                    await ReservationsDB.GetDb().UpdateAsync(dialog.Reservation);
                    await ((ReservationsMVVM)DataContext).LoadDataAsync();
                }
            }
        }
    }
}