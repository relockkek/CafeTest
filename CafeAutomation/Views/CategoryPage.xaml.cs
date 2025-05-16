using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CafeAutomation.DB;
using CafeAutomation.Models;

namespace CafeAutomation.Views
{
    public partial class CategoryPage : Page, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private bool isLoading;
        private ObservableCollection<Dishes> dishes = new();

        public string SelectedCategory { get; }
        public ObservableCollection<Dishes> Dishes
        {
            get => dishes;
            set
            {
                dishes = value;
                OnPropertyChanged(nameof(Dishes));
            }
        }

        public bool IsLoading
        {
            get => isLoading;
            set
            {
                isLoading = value;
                OnPropertyChanged(nameof(IsLoading));
            }
        }

        public CategoryPage(string categoryName)
        {
            InitializeComponent();
            SelectedCategory = categoryName;
            DataContext = this;
            Loaded += CategoryPage_Loaded;
        }

        private async void CategoryPage_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadDishesAsync();
        }

        private async Task LoadDishesAsync()
        {
            IsLoading = true;

            var all = await DishesDB.GetDb().SelectAllAsync();
            Dishes = new ObservableCollection<Dishes>(
                all.Where(d => d.Category == SelectedCategory && d.IsAvailable)
            );

            IsLoading = false;
        }

        private void BackToCategories_Click(object sender, MouseButtonEventArgs e)
        {
            var main = Application.Current.MainWindow as MainWindow;
            main?.MainFrame.Navigate(new MenuPage());
        }

        private void EditDish_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is Dishes dish)
            {
                var dialog = new AddDishDialog(dish);
                if (dialog.ShowDialog() == true && dialog.ResultDish != null)
                {
                    _ = DishesDB.GetDb().UpdateAsync(dialog.ResultDish);
                    _ = LoadDishesAsync();
                }
            }
        }

        private async void DeleteDish_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is Dishes dish)
            {
                await DishesDB.GetDb().DeleteAsync(dish);
                _ = LoadDishesAsync();
            }
        }

        private async void AddDish_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AddDishDialog();
            if (dialog.ShowDialog() == true && dialog.ResultDish != null)
            {
                await DishesDB.GetDb().InsertAsync(dialog.ResultDish);
                await LoadDishesAsync();
            }
        }

        protected void OnPropertyChanged(string propName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
    }
}
