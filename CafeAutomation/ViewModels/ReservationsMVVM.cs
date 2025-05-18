using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using CafeAutomation.Models;
using CafeAutomation.DB;
using CafeAutomation.Views;

namespace CafeAutomation.ViewModels
{
    public class ReservationsMVVM : BaseVM
    {
        public ObservableCollection<Reservations> Reservations { get; set; } = new();
        public Reservations SelectedReservation { get; set; }

        public CommandMvvm AddReservation { get; }
        public CommandMvvm UpdateReservation { get; }
        public CommandMvvm RemoveReservation { get; }

        public ReservationsMVVM()
        {
            _ = LoadDataAsync();

            AddReservation = new CommandMvvm(async (_) =>
            {
                var res = new Reservations
                {
                    TableID = 1,
                    CustomerName = "",
                    CustomerPhone = "",
                    GuestsCount = 1,
                    ReservationDate = DateTime.Now,
                    Status = "Свободен"
                };

                var dialog = new EditReservationDialog(res);
                if (dialog.ShowDialog() == true)
                {
                    if (ReservationsDB.GetDb().Insert(res))
                    {
                        await LoadDataAsync();
                    }
                }
            });

            UpdateReservation = new CommandMvvm(async (_) =>
            {
                if (SelectedReservation == null) return;

                var copy = new Reservations
                {
                    ID = SelectedReservation.ID,
                    TableID = SelectedReservation.TableID,
                    CustomerName = SelectedReservation.CustomerName,
                    CustomerPhone = SelectedReservation.CustomerPhone,
                    GuestsCount = SelectedReservation.GuestsCount,
                    ReservationDate = SelectedReservation.ReservationDate,
                    Status = SelectedReservation.Status
                };

                var dialog = new EditReservationDialog(copy);
                if (dialog.ShowDialog() == true)
                {
                    await ReservationsDB.GetDb().UpdateAsync(copy);
                    await LoadDataAsync();
                }
            });

            RemoveReservation = new CommandMvvm(async (_) =>
            {
                if (SelectedReservation == null) return;

                var result = MessageBox.Show("Удалить выбранное бронирование?", "Подтверждение", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    await ReservationsDB.GetDb().DeleteAsync(SelectedReservation);
                    await LoadDataAsync();
                }
            });
        }

        public async Task LoadDataAsync()
        {
            var data = await ReservationsDB.GetDb().SelectAllAsync();
            Reservations = new ObservableCollection<Reservations>(data);
            Signal(nameof(Reservations));
        }
    }
}
