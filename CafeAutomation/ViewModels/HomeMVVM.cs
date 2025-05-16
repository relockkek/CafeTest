using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using CafeAutomation.DB;
using CafeAutomation.Models;
using CafeAutomation.ViewModels;
using CafeAutomation.Views;

namespace CafeAutomation.ViewModels
{
    internal class HomeMVVM : BaseVM
    {
        private string todayRevenue;
        private string popularDish;
        private string tablesStatus;
        private int ordersCount;
        private bool isLoading;

        public string TodayRevenue
        {
            get => todayRevenue;
            set { todayRevenue = value; Signal(); }
        }

        public string PopularDish
        {
            get => popularDish;
            set { popularDish = value; Signal(); }
        }

        public string TablesStatus
        {
            get => tablesStatus;
            set { tablesStatus = value; Signal(); }
        }

        public int OrdersCount
        {
            get => ordersCount;
            set { ordersCount = value; Signal(); }
        }

        public bool IsLoading
        {
            get => isLoading;
            set { isLoading = value; Signal(); }
        }

        public CommandMvvm LoadReport { get; }
        public ICommand NavigateToCategoryCommand { get; }

        public HomeMVVM()
        {
            LoadReport = new CommandMvvm(async (_) => await LoadDataAsync(), (_) => !IsLoading);
            NavigateToCategoryCommand = new CommandMvvm(_ => ExecuteNavigateToCategory(), _ => true);

            Application.Current.Dispatcher.InvokeAsync(async () => await LoadDataAsync());
        }

        private async Task LoadDataAsync()
        {
            if (IsLoading) return;
            IsLoading = true;

            try
            {
                var todayOrders = (await OrdersDB.GetDb().SelectAllAsync())
                                        .Where(o => o.OrderDate.Date == DateTime.Today)
                                        .ToList();

                OrdersCount = todayOrders.Count;
                decimal totalRevenue = todayOrders.Sum(o => o.TotalAmount);
                TodayRevenue = $"Выручка за день: {totalRevenue:C}";

                var dish = await DishesDB.GetDb().GetMostPopularDishAsync();
                PopularDish = $"Популярное блюдо: {dish?.Name ?? "Нет данных"}";

                var reservations = await ReservationsDB.GetDb().SelectAllAsync();
                var occupiedTableIds = reservations
                    .Where(r => r.Status != null && r.Status.Equals("Занят", StringComparison.OrdinalIgnoreCase))
                    .Select(r => r.TableID)
                    .Distinct()
                    .ToList();

                int totalTables = 20; // Статическое значение для общего количества столов
                int occupiedTables = occupiedTableIds.Count;

                TablesStatus = $"{occupiedTables} из {totalTables} столов занято";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки данных: " + ex.Message);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void ExecuteNavigateToCategory()
        {
            var param = "Горячие блюда";
            OnNavigateToCategory(param);
        }

        private void OnNavigateToCategory(object param)
        {
            if (param is string categoryName)
            {
                var categoryPage = new CategoryPage(categoryName);
                var mainWindow = Application.Current.MainWindow as MainWindow;

                if (mainWindow != null)
                {
                    mainWindow.MainFrame.Content = categoryPage;
                }
            }
        }
    }
}
