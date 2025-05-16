using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CafeAutomation.Models;
using CafeAutomation.DB;

namespace CafeAutomation.ViewModels
{
    public class DishesMVVM : BaseVM
    {
        private string? selectedCategory;
        private ObservableCollection<Dishes> dishes = new();
        private Dishes selectedDish;
        private ObservableCollection<CategoryItem> categories;
        private bool isLoading;

        public ObservableCollection<Dishes> Dishes
        {
            get => dishes;
            set { dishes = value; Signal(); }
        }

        public Dishes SelectedDish
        {
            get => selectedDish;
            set { selectedDish = value; Signal(); }
        }

        public ObservableCollection<CategoryItem> Categories
        {
            get => categories;
            set { categories = value; Signal(); }
        }

        public bool IsLoading
        {
            get => isLoading;
            set { isLoading = value; Signal(); }
        }

        public DishesMVVM()
        {
            Categories = new ObservableCollection<CategoryItem>
            {
                new CategoryItem { Name = "Горячие блюда", ImagePath = "pack://application:,,,/Images/hot.jpg" },
                new CategoryItem { Name = "Напитки", ImagePath = "pack://application:,,,/Images/drinks.jpg" },
                new CategoryItem { Name = "Закуски", ImagePath = "pack://application:,,,/Images/snacks.jpg" },
                new CategoryItem { Name = "Десерты", ImagePath = "pack://application:,,,/Images/dessert.jpg" },
                new CategoryItem { Name = "Салаты", ImagePath = "pack://application:,,,/Images/salad.jpg" },
                new CategoryItem { Name = "Завтраки", ImagePath = "pack://application:,,,/Images/breakfast.jpg" }
            };
        }

        public DishesMVVM(string category) : this()
        {
            selectedCategory = category;
            _ = LoadDishesAsync();
        }

        public async Task RefreshData(string category)
        {
            selectedCategory = category;
            await LoadDishesAsync();
        }

        private async Task LoadDishesAsync()
        {
            IsLoading = true;

            Dishes.Clear();
            var filtered = await DishesDB.GetDb().SelectByCategoryAsync(selectedCategory);
            foreach (var dish in filtered)
                Dishes.Add(dish);

            IsLoading = false;
        }
    }

    public class CategoryItem
    {
        public string Name { get; set; }
        public string ImagePath { get; set; }
    }
}
