using System.Collections.ObjectModel;
using System.Windows;
using CafeAutomation.DB;
using CafeAutomation.ViewModels;
using System.Threading.Tasks;
using CafeAutomation.Models;

namespace CafeAutomation.ViewModels
{
    internal class StatusMVVM : BaseVM
    {
        private Status selectedStatus;
        private ObservableCollection<Status> statuses = new();

        public ObservableCollection<Status> Statuses
        {
            get => statuses;
            set
            {
                statuses = value;
                Signal();
            }
        }

        public Status SelectedStatus
        {
            get => selectedStatus;
            set
            {
                selectedStatus = value;
                Signal();
            }
        }

        public CommandMvvm AddStatus { get; }
        public CommandMvvm UpdateStatus { get; }
        public CommandMvvm RemoveStatus { get; }

        public StatusMVVM()
        {
            LoadDataAsync();

            AddStatus = new CommandMvvm((_) =>
            {
                var status = new Status
                {
                    Title = "Новый статус"
                };

                if (StatusDB.GetDb().Insert(status))
                {
                    LoadDataAsync();
                    SelectedStatus = status;
                }
            }, (_) => true);

            UpdateStatus = new CommandMvvm(async (_) =>
            {
                if (SelectedStatus != null && await StatusDB.GetDb().UpdateAsync(SelectedStatus))
                {
                    MessageBox.Show("Статус обновлён");
                    await LoadDataAsync();
                }
            }, (_) => SelectedStatus != null);

            RemoveStatus = new CommandMvvm(async (_) =>
            {
                if (SelectedStatus != null && await StatusDB.GetDb().DeleteAsync(SelectedStatus))
                {
                    MessageBox.Show("Статус удалён");
                    await LoadDataAsync();
                }
            }, (_) => SelectedStatus != null);
        }

        private async Task LoadDataAsync()
        {
            var data = await StatusDB.GetDb().SelectAllAsync();
            Statuses = new ObservableCollection<Status>(data);
        }
    }
}