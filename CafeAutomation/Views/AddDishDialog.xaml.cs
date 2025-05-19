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
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Изображения (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg";

            if (dialog.ShowDialog() == true)
            {
                var resized = ResizeAndCompressImage(dialog.FileName, 200, 200, 0.7);
                selectedImageBytes = resized; 

                var image = new BitmapImage();
                image.BeginInit();
                image.StreamSource = new MemoryStream(resized);
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.EndInit();
                PreviewImage.Source = image;
            }
        }

        private byte[] ResizeAndCompressImage(string filePath, int maxWidth, int maxHeight, double quality)
        {
            BitmapImage original = new BitmapImage();
            original.BeginInit();
            original.UriSource = new Uri(filePath);
            original.DecodePixelWidth = maxWidth;
            original.DecodePixelHeight = maxHeight;
            original.CacheOption = BitmapCacheOption.OnLoad;
            original.EndInit();
            original.Freeze();

            var encoder = new JpegBitmapEncoder();
            encoder.QualityLevel = (int)(quality * 100);
            encoder.Frames.Add(BitmapFrame.Create(original));

            using var stream = new MemoryStream();
            encoder.Save(stream);
            return stream.ToArray();
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
