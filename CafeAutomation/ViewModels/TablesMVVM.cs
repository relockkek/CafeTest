using System.Collections.ObjectModel;
using System.Windows;
using CafeAutomation.DB;
using CafeAutomation.ViewModels;
using System.Threading.Tasks;
using CafeAutomation.Models;

namespace CafeAutomation.ViewModels
{
    internal class TablesMVVM : BaseVM
    {
        private Tables selectedTable;
        private ObservableCollection<Tables> tables = new();

        public ObservableCollection<Tables> Tables
        {
            get => tables;
            set
            {
                tables = value;
                Signal();
            }
        }

        public Tables SelectedTable
        {
            get => selectedTable;
            set
            {
                selectedTable = value;
                Signal();
            }
        }

        public CommandMvvm AddTable { get; }
        public CommandMvvm UpdateTable { get; }
        public CommandMvvm RemoveTable { get; }

        public TablesMVVM()
        {
            LoadDataAsync();

            AddTable = new CommandMvvm((_) =>
            {
                var table = new Tables
                {
                    TableNumber = 1,
                    Capacity = 4,
                    Zone = "Зал",
                    IsActive = true
                };

                if (TablesDB.GetDb().Insert(table))
                {
                    LoadDataAsync();
                    SelectedTable = table;
                }
            }, (_) => true);

            UpdateTable = new CommandMvvm(async (_) =>
            {
                if (SelectedTable != null && await TablesDB.GetDb().UpdateAsync(SelectedTable))
                {
                    MessageBox.Show("Обновлён стол");
                    await LoadDataAsync();
                }
            }, (_) => SelectedTable != null);

            RemoveTable = new CommandMvvm(async (_) =>
            {
                if (SelectedTable != null && await TablesDB.GetDb().DeleteAsync(SelectedTable))
                {
                    MessageBox.Show("Стол удалён");
                    await LoadDataAsync();
                }
            }, (_) => SelectedTable != null);
        }

        private async Task LoadDataAsync()
        {
            var data = await TablesDB.GetDb().SelectAllAsync();
            Tables = new ObservableCollection<Tables>(data);
        }
    }
}