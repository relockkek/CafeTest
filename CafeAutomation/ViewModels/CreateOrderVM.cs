using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using CafeAutomation.Models;
using CafeAutomation.DB;
using CafeAutomation.Views;

namespace CafeAutomation.ViewModels
{
    public class CreateOrderVM : BaseVM
    {
        public ObservableCollection<Dishes> AvailableDishes { get; set; } = new();
        public ObservableCollection<DishForOrder> SelectedDishes { get; set; } = new();

        public DishForOrder SelectedDishForOrder { get; set; }
        public Dishes SelectedAvailableDish { get; set; }

        public ObservableCollection<string> Categories { get; set; } = new();
        private string selectedCategory;
        public string SelectedCategory
        {
            get => selectedCategory;
            set
            {
                selectedCategory = value;
                _ = LoadAvailableDishesAsync();
                Signal();
            }
        }

        public ObservableCollection<int> TableNumbers { get; set; } = new();
        public int SelectedTable { get; set; } = 1;

        public string OrderNotes { get; set; } = "";

        public decimal OrderTotal => SelectedDishes.Sum(x => x.Total);

        public CommandMvvm AddToOrder { get; }
        public CommandMvvm RemoveFromOrder { get; }
        public CommandMvvm ConfirmOrder { get; }

        public CommandMvvm IncreaseQuantity { get; }
        public CommandMvvm DecreaseQuantity { get; }

        public CreateOrderVM()
        {
            _ = LoadCategoriesAsync();
            _ = LoadFreeTablesAsync();

            AddToOrder = new CommandMvvm(async (_) =>
            {
                if (SelectedAvailableDish == null) return;

                var fullDish = await DishesDB.GetDb().GetByIdAsync(SelectedAvailableDish.ID);
                if (fullDish == null) return;

                var existing = SelectedDishes.FirstOrDefault(x => x.Dish.ID == fullDish.ID);
                if (existing != null)
                    existing.Quantity++;
                else
                    SelectedDishes.Add(new DishForOrder { Dish = fullDish, Quantity = 1 });

                Signal(nameof(OrderTotal));
            });

            RemoveFromOrder = new CommandMvvm((_) =>
            {
                if (SelectedDishForOrder != null)
                {
                    SelectedDishes.Remove(SelectedDishForOrder);
                    Signal(nameof(OrderTotal));
                }
            });

            ConfirmOrder = new CommandMvvm(async (_) =>
            {
                if (SelectedDishes.Count == 0 || SelectedTable == 0)
                {
                    MessageBox.Show("Добавьте хотя бы одно блюдо и выберите стол.");
                    return;
                }

                var order = new Orders
                {
                    EmployeeID = 1,
                    TableNumber = SelectedTable,
                    OrderDate = DateTime.Now,
                    StatusID = 1,
                    TotalAmount = OrderTotal
                };

                bool inserted = await OrdersDB.GetDb().InsertAsync(order);
                if (!inserted)
                {
                    MessageBox.Show("Не удалось создать заказ.");
                    return;
                }

                var insertTasks = SelectedDishes.Select(dish =>
                    OrderItemsDB.GetDb().InsertAsync(new OrderItems
                    {
                        OrderID = order.ID,
                        DishID = dish.Dish.ID,
                        Amount = dish.Quantity,
                        PriceAtOrderTime = dish.Dish.Price
                    })
                );
                await Task.WhenAll(insertTasks);

                string orderInfo = string.Join("\n", SelectedDishes.Select(d => $"{d.Dish.Name} x {d.Quantity} → {d.Total:C}")) +
                                   $"\n\nИтого: {OrderTotal:C}";

                var dialog = new PaymentDialog(orderInfo);
                if (dialog.ShowDialog() == true && dialog.PaymentConfirmed)
                {
                    MessageBox.Show($"Оплата прошла: {dialog.SelectedMethod}");
                }
                else
                {
                    MessageBox.Show("Оплата отменена.");
                }

                MessageBox.Show($"Заказ оформлен!\nСтол: {SelectedTable}\nДетали: {OrderNotes}");

                await ClearForm();
                Signal(nameof(OrderNotes));
                Signal(nameof(SelectedDishes));
                Signal(nameof(OrderTotal));
            });

            IncreaseQuantity = new CommandMvvm((obj) =>
            {
                if (obj is DishForOrder item)
                {
                    item.Quantity++;
                    Signal(nameof(OrderTotal));
                }
            });

            DecreaseQuantity = new CommandMvvm((obj) =>
            {
                if (obj is DishForOrder item)
                {
                    if (item.Quantity > 1)
                        item.Quantity--;
                    else
                        SelectedDishes.Remove(item);
                    Signal(nameof(OrderTotal));
                }
            });
        }

        private async Task LoadCategoriesAsync()
        {
            var categories = await DishesDB.GetDb().GetDistinctCategoriesAsync();
            Categories = new ObservableCollection<string>(categories);
            SelectedCategory = Categories.FirstOrDefault();
            Signal(nameof(Categories));
        }

        private async Task LoadAvailableDishesAsync()
        {
            if (string.IsNullOrEmpty(SelectedCategory)) return;

            var filtered = await DishesDB.GetDb().SelectByCategoryLightAsync(SelectedCategory);
            AvailableDishes = new ObservableCollection<Dishes>(filtered);
            Signal(nameof(AvailableDishes));
        }

        private async Task LoadFreeTablesAsync()
        {
            var allTables = Enumerable.Range(1, 20).ToList();

            var reservations = await ReservationsDB.GetDb().SelectAllAsync();
            var reservedTables = reservations
                .Where(r => !string.IsNullOrEmpty(r.Status) && r.Status.ToLower() == "занят") // ← изменено с "активна"
                .Select(r => r.TableID)
                .Distinct()
                .ToList();

            var freeTables = allTables.Except(reservedTables).ToList();

            TableNumbers = new ObservableCollection<int>(freeTables);
            SelectedTable = TableNumbers.FirstOrDefault();

            Signal(nameof(TableNumbers));
            Signal(nameof(SelectedTable));
        }


        public async Task ClearForm()
        {
            SelectedDishes.Clear();
            OrderNotes = string.Empty;
            await LoadFreeTablesAsync();
            SelectedAvailableDish = null;
        }
    }
}
