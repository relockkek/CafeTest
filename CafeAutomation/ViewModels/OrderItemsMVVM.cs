using System.Collections.ObjectModel;
using System.Windows;
using CafeAutomation.DB;
using CafeAutomation.ViewModels;
using System.Threading.Tasks;
using CafeAutomation.Models;

namespace CafeAutomation.ViewModels
{
    public class OrderItemsMVVM : BaseVM
    {
        private OrderItems selectedItem;
        private ObservableCollection<OrderItems> items = new();
        private ObservableCollection<string> itemsFormatted = new();

        public ObservableCollection<OrderItems> Items
        {
            get => items;
            set
            {
                items = value;
                Signal();
            }
        }

        public ObservableCollection<string> ItemsFormatted
        {
            get => itemsFormatted;
            set
            {
                itemsFormatted = value;
                Signal();
            }
        }

        public OrderItems SelectedItem
        {
            get => selectedItem;
            set
            {
                selectedItem = value;
                Signal();
            }
        }

        public CommandMvvm AddItem { get; }
        public CommandMvvm UpdateItem { get; }
        public CommandMvvm RemoveItem { get; }

        public OrderItemsMVVM()
        {
            LoadDataAsync();

            AddItem = new CommandMvvm((_) =>
            {
                var newItem = new OrderItems
                {
                    OrderID = 1,
                    DishID = 1,
                    Amount = 1,
                    PriceAtOrderTime = 0
                };

                if (OrderItemsDB.GetDb().Insert(newItem))
                {
                    LoadDataAsync();
                    SelectedItem = newItem;
                }
            }, (_) => true);

            UpdateItem = new CommandMvvm(async (_) =>
            {
                if (SelectedItem != null && await OrderItemsDB.GetDb().UpdateAsync(SelectedItem))
                {
                    MessageBox.Show("Обновлено");
                    await LoadDataAsync();
                }
            }, (_) => SelectedItem != null);

            RemoveItem = new CommandMvvm(async (_) =>
            {
                if (SelectedItem != null && await OrderItemsDB.GetDb().DeleteAsync(SelectedItem))
                {
                    MessageBox.Show("Удалено");
                    await LoadDataAsync();
                }
            }, (_) => SelectedItem != null);
        }

        private async Task LoadDataAsync()
        {
            var data = await OrderItemsDB.GetDb().SelectAllAsync();
            Items = new ObservableCollection<OrderItems>(data);
        }
    }
}
