using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using CafeAutomation.Models;
using Microsoft.Win32;

namespace CafeAutomation.Views
{
    public partial class EditDishDialog : Window
    {
        public Dishes Dish { get; }
        private byte[] selectedImageBytes;

        public EditDishDialog(Dishes dish)
        {
            InitializeComponent();

            // Копируем блюдо и изображение
            Dish = new Dishes
            {
                ID = dish.ID,
                Name = dish.Name,
                Description = dish.Description,
                Price = dish.Price,
                Category = dish.Category,
                IsAvailable = dish.IsAvailable,
                ImageData = dish.ImageData
            };

            selectedImageBytes = dish.ImageData;

            DataContext = this;

            // Показать изображение, если есть
            if (selectedImageBytes != null)
            {
                using var ms = new MemoryStream(selectedImageBytes);
                var image = new BitmapImage();
                image.BeginInit();
                image.StreamSource = ms;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.EndInit();
                PreviewImage.Source = image;
            }
        }

        private void SelectImage_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Изображения (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg"
            };

            if (dialog.ShowDialog() == true)
            {
                selectedImageBytes = File.ReadAllBytes(dialog.FileName);
                PreviewImage.Source = new BitmapImage(new System.Uri(dialog.FileName));
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            // Если новое изображение не выбрано — оставить старое
            Dish.ImageData = selectedImageBytes;

            DialogResult = true;
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

    }
}
