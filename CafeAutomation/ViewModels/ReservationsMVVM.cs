using System.Collections.ObjectModel;
using System.Windows;
using CafeAutomation.DB;
using CafeAutomation.ViewModels;
using System.Threading.Tasks;
using CafeAutomation.Models;

namespace CafeAutomation.ViewModels
{
    internal class ReservationsMVVM : BaseVM
    {
        private Reservations selectedReservation;
        private ObservableCollection<Reservations> reservations = new();

        public ObservableCollection<Reservations> Reservations
        {
            get => reservations;
            set
            {
                reservations = value;
                Signal();
            }
        }

        public Reservations SelectedReservation
        {
            get => selectedReservation;
            set
            {
                selectedReservation = value;
                Signal();
            }
        }

        public CommandMvvm AddReservation { get; }
        public CommandMvvm UpdateReservation { get; }
        public CommandMvvm RemoveReservation { get; }

        public ReservationsMVVM()
        {
            LoadDataAsync();

            AddReservation = new CommandMvvm((_) =>
            {
                var res = new Reservations
                {
                    TableID = 1,
                    CustomerName = "Гость",
                    GuestsCount = 2,
                    ReservationDate = DateTime.Now,
                    Status = "Активна"
                };

                if (ReservationsDB.GetDb().Insert(res))
                {
                    LoadDataAsync();
                    SelectedReservation = res;
                }
            }, (_) => true);

            UpdateReservation = new CommandMvvm(async (_) =>
            {
                if (SelectedReservation != null && await ReservationsDB.GetDb().UpdateAsync(SelectedReservation))
                {
                    MessageBox.Show("Обновлено");
                    await LoadDataAsync();
                }
            }, (_) => SelectedReservation != null);

            RemoveReservation = new CommandMvvm(async (_) =>
            {
                if (SelectedReservation != null && await ReservationsDB.GetDb().DeleteAsync(SelectedReservation))
                {
                    MessageBox.Show("Удалено");
                    await LoadDataAsync();
                }
            }, (_) => SelectedReservation != null);
        }

        public async Task LoadDataAsync()
        {
            var data = await ReservationsDB.GetDb().SelectAllAsync();
            Reservations = new ObservableCollection<Reservations>(data);
        }
    }
}