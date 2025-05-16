using System.Windows;
using System.Windows.Controls;
using CafeAutomation.ViewModels;

namespace CafeAutomation.Views
{
    public partial class MenuPage : Page
    {
        public MenuPage()
        {
            InitializeComponent();
            Loaded += MenuPage_Loaded;
        }

        private void MenuPage_Loaded(object sender, RoutedEventArgs e)
        {
            var vm = new DishesMVVM();
            DataContext = vm;
        }

        private void Category_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is string category)
            {
                NavigationService?.Navigate(new DishesPage(category));
            }
        }
    }
}
