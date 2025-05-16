using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using CafeAutomation.Models;
using Microsoft.Win32;

namespace CafeAutomation.Views
{
    public partial class AddDishDialog : Window
    {
        public Dishes ResultDish { get; private set; }

        private Dishes editingDish; // блюдо, которое редактируем
        private byte[] selectedImageBytes; // новое или существующее изображение

        public AddDishDialog()
        {
            InitializeComponent();
            CategoryBox.ItemsSource = new[] { "Горячие блюда", "Напитки", "Закуски", "Десерты", "Салаты", "Завтраки" };
        }

        public AddDishDialog(Dishes dishToEdit) : this()
        {
            editingDish = dishToEdit;

            NameBox.Text = dishToEdit.Name;
            DescriptionBox.Text = dishToEdit.Description;
            PriceBox.Text = dishToEdit.Price.ToString();
            CategoryBox.SelectedItem = dishToEdit.Category;
            selectedImageBytes = dishToEdit.ImageData;

            if (dishToEdit.ImageData != null)
            {
                using var ms = new MemoryStream(dishToEdit.ImageData);
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

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            // если не выбрано новое изображение — оставить старое
            if (selectedImageBytes == null && editingDish != null)
                selectedImageBytes = editingDish.ImageData;

            ResultDish = new Dishes
            {
                ID = editingDish?.ID ?? 0,
                Name = NameBox.Text.Trim(),
                Description = DescriptionBox.Text.Trim(),
                Price = decimal.TryParse(PriceBox.Text, out var price) ? price : 0,
                Category = CategoryBox.SelectedItem?.ToString(),
                IsAvailable = true,
                ImageData = selectedImageBytes
            };

            DialogResult = true;
            Close();
        }
        private void Add_Click(object sender, RoutedEventArgs e)
        {
            // Валидация полей (можно добавить по желанию)

            ResultDish = new Dishes
            {
                Name = NameBox.Text,
                Description = DescriptionBox.Text,
                Category = CategoryBox.SelectedItem?.ToString(),
                IsAvailable = true,
                Price = decimal.TryParse(PriceBox.Text, out var price) ? price : 0,
                ImageData = selectedImageBytes
            };

            DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
