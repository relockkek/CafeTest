using CafeAutomation.DB;
using CafeAutomation.Models;
using CafeAutomation.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CafeAutomation.Converters;
namespace CafeAutomation.Views
{
    public partial class DishesPage : Page
    {
        private readonly string category;

        public DishesPage(string category)
        {
            InitializeComponent();
            this.category = category;
            DataContext = new DishesMVVM(category);
            CategoryTitle.Text = category;
        }
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new MenuPage());
        }

        private void AddDish_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AddDishDialog(); // ← для создания нового блюда

            if (dialog.ShowDialog() == true)
            {
                var newDish = dialog.ResultDish;

                DishesDB.GetDb().Insert(newDish); // метод синхронный

                if (DataContext is DishesMVVM vm)
                {
                    _ = vm.RefreshData(category); // category — приватное поле класса
                    vm.SelectedDish = newDish;
                }
            }
        }



        private async void EditDish_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Dishes dish)
            {
                var editDialog = new EditDishDialog(dish);
                if (editDialog.ShowDialog() == true)
                {
                    await DishesDB.GetDb().UpdateAsync(editDialog.Dish);
                    await ((DishesMVVM)DataContext).RefreshData(category);
                    MessageBox.Show("Блюдо обновлено");
                }
            }
        }

        private async void DeleteDish_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Dishes dish)
            {
                var result = MessageBox.Show($"Удалить блюдо \"{dish.Name}\"?", "Подтверждение", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    await DishesDB.GetDb().DeleteAsync(dish); // ← передаём объект, как требует метод
                    await ((DishesMVVM)DataContext).RefreshData(category);
                    MessageBox.Show("Блюдо удалено");
                }
            }
        }
    }
}
