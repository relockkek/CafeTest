using System.Collections.ObjectModel;
using System.Windows;
using CafeAutomation.DB;
using CafeAutomation.ViewModels;
using System.Threading.Tasks;
using CafeAutomation.Models;
using CafeAutomation.Views;
using System.Linq;

namespace CafeAutomation.ViewModels
{
    public class OrdersMVVM : BaseVM
    {
        private Orders selectedOrder;
        private ObservableCollection<Orders> orders = new();
        private bool isLoading;

        public ObservableCollection<Orders> Orders
        {
            get => orders;
            set
            {
                orders = value;
                Signal();
            }
        }

        public Orders SelectedOrder
        {
            get => selectedOrder;
            set
            {
                selectedOrder = value;
                Signal();
                LoadOrderItems();
            }
        }

        public bool IsLoading
        {
            get => isLoading;
            set
            {
                isLoading = value;
                Signal();
            }
        }

        public OrderItemsMVVM OrderItemsVM { get; } = new OrderItemsMVVM();

        public CommandMvvm AddOrder { get; }
        public CommandMvvm UpdateOrder { get; }
        public CommandMvvm RemoveOrder { get; }

        public OrdersMVVM()
        {
            _ = LoadDataAsync(); // Асинхронная фоновая загрузка

            AddOrder = new CommandMvvm((_) =>
            {
                var dialog = new CreateOrderDialog();
                dialog.ShowDialog();
                _ = LoadDataAsync(); // Перезагрузка после добавления
            });

            UpdateOrder = new CommandMvvm(async (_) =>
            {
                if (SelectedOrder != null && await OrdersDB.GetDb().UpdateAsync(SelectedOrder))
                {
                    MessageBox.Show("Обновлено");
                    await LoadDataAsync();
                }
            }, (_) => SelectedOrder != null);

            RemoveOrder = new CommandMvvm(async (_) =>
            {
                if (SelectedOrder != null && await OrdersDB.GetDb().DeleteAsync(SelectedOrder))
                {
                    MessageBox.Show("Удалён заказ");
                    await LoadDataAsync();
                }
            }, (_) => SelectedOrder != null);
        }

        private async Task LoadDataAsync()
        {
            IsLoading = true;
            await Task.Delay(1); // даём UI отрисоваться
            var data = await OrdersDB.GetDb().SelectAllAsync();
            Orders = new ObservableCollection<Orders>(data);
            IsLoading = false;
        }

        private async void LoadOrderItems()
        {
            if (SelectedOrder != null)
            {
                var details = await OrderItemsDB.GetDb().GetOrderDetailsAsync(SelectedOrder.ID);
                OrderItemsVM.ItemsFormatted = new ObservableCollection<string>(
                    details.Select(i => $"{i.Name} × {i.Quantity} — {i.Total} ₽")
                );
            }
            else
            {
                OrderItemsVM.ItemsFormatted = new ObservableCollection<string>();
            }
        }
    }
}
