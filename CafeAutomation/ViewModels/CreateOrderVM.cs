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

        private List<Dishes> allDishes;

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
                _ = FilterDishesAsync();
                Signal();
            }
        }

        public ObservableCollection<int> TableNumbers { get; set; } =
            new ObservableCollection<int>(Enumerable.Range(1, 20));
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
            _ = LoadDishesAsync();

            AddToOrder = new CommandMvvm((_) =>
            {
                if (SelectedAvailableDish == null) return;

                var existing = SelectedDishes.FirstOrDefault(x => x.Dish.ID == SelectedAvailableDish.ID);
                if (existing != null)
                    existing.Quantity++;
                else
                    SelectedDishes.Add(new DishForOrder { Dish = SelectedAvailableDish, Quantity = 1 });

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

                bool inserted = await Task.Run(() => OrdersDB.GetDb().Insert(order));

                if (inserted)
                {
                    foreach (var dish in SelectedDishes)
                    {
                        await Task.Run(() => OrderItemsDB.GetDb().Insert(new OrderItems
                        {
                            OrderID = order.ID,
                            DishID = dish.Dish.ID,
                            Amount = dish.Quantity,
                            PriceAtOrderTime = dish.Dish.Price
                        }));
                    }

                    // Обновляем статус стола
                    var tables = await TablesDB.GetDb().SelectAllAsync();
                    var tableToUpdate = tables.FirstOrDefault(t => t.TableNumber == SelectedTable);
                    if (tableToUpdate != null)
                    {
                        tableToUpdate.IsActive = true;
                        await TablesDB.GetDb().UpdateAsync(tableToUpdate);
                    }

                    // Окно оплаты
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

                    // Очистка формы
                    ClearForm();
                    Signal(nameof(OrderNotes));
                    Signal(nameof(SelectedDishes));
                    Signal(nameof(OrderTotal));
                }
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

        private async Task LoadDishesAsync()
        {
            var all = await DishesDB.GetDb().SelectAllAsync();
            allDishes = all.Where(d => d.IsAvailable).ToList();
            Categories = new ObservableCollection<string>(allDishes.Select(d => d.Category).Distinct());
            SelectedCategory = Categories.FirstOrDefault();
            Signal(nameof(Categories));
        }

        private async Task FilterDishesAsync()
        {
            if (string.IsNullOrEmpty(SelectedCategory)) return;
            var filtered = await Task.Run(() =>
                allDishes.Where(d => d.Category == SelectedCategory).ToList());

            AvailableDishes = new ObservableCollection<Dishes>(filtered);
            Signal(nameof(AvailableDishes));
        }

        public void ClearForm()
        {
            SelectedDishes.Clear();
            OrderNotes = string.Empty;
            SelectedTable = TableNumbers.FirstOrDefault();
            SelectedAvailableDish = null;
        }
    }
}
