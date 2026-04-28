using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PerfumeWarehouseWPF
{
    public partial class ProductEditWindow : Window
    {
        private readonly Product _product;
        private readonly bool _isNew;
        private ObservableCollection<Brand> _brands;
        private ObservableCollection<Note> _allNotes;

        public ProductEditWindow(Product product = null)
        {
            InitializeComponent();
            _product = product;
            _isNew = product == null;
            TitleText.Text = _isNew ? "Новый товар" : "Редактирование товара";
            LoadData();
        }

        private void LoadData()
        {
            using (var db = new AppDbContext())
            {
                _brands = new ObservableCollection<Brand>(db.Brands.ToList());
                BrandComboBox.ItemsSource = _brands;

                _allNotes = new ObservableCollection<Note>(db.Notes.OrderBy(n => n.NoteName).ToList());
                NotesListBox.ItemsSource = _allNotes;

                if (!_isNew)
                {
                    ProductNameTextBox.Text = _product.ProductName;
                    DescriptionTextBox.Text = _product.Description;
                    BrandComboBox.SelectedValue = _product.BrandID;

                    var mainImage = _product.Images.FirstOrDefault(img => img.IsMain) ?? _product.Images.FirstOrDefault();
                    ImageFileNameTextBox.Text = mainImage?.ImageFileName ?? "";

                    var variant60 = _product.Variants.FirstOrDefault(v => v.VolumeML == 60);
                    var variant120 = _product.Variants.FirstOrDefault(v => v.VolumeML == 120);

                    Has60CheckBox.IsChecked = variant60 != null;
                    Has120CheckBox.IsChecked = variant120 != null;

                    if (variant60 != null)
                    {
                        Price60TextBox.Text = variant60.Price.ToString();
                        Stock60TextBox.Text = variant60.StockQuantity.ToString();
                    }
                    if (variant120 != null)
                    {
                        Price120TextBox.Text = variant120.Price.ToString();
                        Stock120TextBox.Text = variant120.StockQuantity.ToString();
                    }

                    var anyVariant = variant60 ?? variant120;
                    AlcoholPercentTextBox.Text = anyVariant?.AlcoholPercent.ToString() ?? "80";

                    var productNoteIds = db.ProductNotes.Where(pn => pn.ProductID == _product.ProductID).Select(pn => pn.NoteID).ToList();
                    foreach (var note in _allNotes.Where(n => productNoteIds.Contains(n.NoteID)))
                        NotesListBox.SelectedItems.Add(note);
                }
                else
                {
                    BrandComboBox.SelectedIndex = _brands.Any() ? 0 : -1;
                    AlcoholPercentTextBox.Text = "80";
                    Price60TextBox.Text = "5000";
                    Stock60TextBox.Text = "10";
                    Price120TextBox.Text = "8000";
                    Stock120TextBox.Text = "5";
                }
            }
            UpdateVolumePanels();
        }

        private void VolumeCheckBox_Changed(object sender, RoutedEventArgs e) => UpdateVolumePanels();

        private void UpdateVolumePanels()
        {
            Panel60.IsEnabled = Has60CheckBox.IsChecked == true;
            Panel120.IsEnabled = Has120CheckBox.IsChecked == true;
        }

        private void BrowseImageButton_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog();
            dlg.Filter = "Изображения (*.png;*.jpg;*.jpeg;*.bmp)|*.png;*.jpg;*.jpeg;*.bmp|Все файлы (*.*)|*.*";
            if (dlg.ShowDialog() == true)
            {
                string selectedFile = dlg.FileName;
                string fileName = Path.GetFileName(selectedFile);
                string destFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");
                if (!Directory.Exists(destFolder))
                    Directory.CreateDirectory(destFolder);
                string destPath = Path.Combine(destFolder, fileName);
                try
                {
                    File.Copy(selectedFile, destPath, true);
                    ImageFileNameTextBox.Text = fileName;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Не удалось скопировать файл: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            ErrorMessage.Visibility = Visibility.Collapsed;

            string productName = ProductNameTextBox.Text.Trim();
            if (string.IsNullOrEmpty(productName))
            {
                ShowError("Название обязательно.");
                return;
            }
            if (BrandComboBox.SelectedValue == null)
            {
                ShowError("Выберите бренд.");
                return;
            }
            if (Has60CheckBox.IsChecked == false && Has120CheckBox.IsChecked == false)
            {
                ShowError("Должен быть хотя бы один вариант объёма.");
                return;
            }
            if (!decimal.TryParse(AlcoholPercentTextBox.Text.Trim(), out decimal alcoholPercent) || alcoholPercent < 0 || alcoholPercent > 100)
            {
                ShowError("Процент спирта должен быть числом от 0 до 100.");
                return;
            }

            decimal? price60 = null, price120 = null;
            int? stock60 = null, stock120 = null;

            if (Has60CheckBox.IsChecked == true)
            {
                if (!decimal.TryParse(Price60TextBox.Text.Trim(), out decimal p60) || p60 <= 0)
                { ShowError("Цена 60 мл должна быть положительным числом."); return; }
                if (!int.TryParse(Stock60TextBox.Text.Trim(), out int s60) || s60 < 0)
                { ShowError("Остаток 60 мл должен быть целым неотрицательным числом."); return; }
                price60 = p60; stock60 = s60;
            }
            if (Has120CheckBox.IsChecked == true)
            {
                if (!decimal.TryParse(Price120TextBox.Text.Trim(), out decimal p120) || p120 <= 0)
                { ShowError("Цена 120 мл должна быть положительным числом."); return; }
                if (!int.TryParse(Stock120TextBox.Text.Trim(), out int s120) || s120 < 0)
                { ShowError("Остаток 120 мл должен быть целым неотрицательным числом."); return; }
                price120 = p120; stock120 = s120;
            }

            using (var db = new AppDbContext())
            {
                Product product;
                if (_isNew)
                {
                    product = new Product { CreatedAt = DateTime.Now };
                    db.Products.Add(product);
                }
                else
                {
                    product = db.Products
                        .Include(p => p.Variants)
                        .Include(p => p.Images)
                        .Include(p => p.ProductNotes)
                        .First(p => p.ProductID == _product.ProductID);
                }

                product.ProductName = productName;
                product.BrandID = (int)BrandComboBox.SelectedValue;
                product.Description = DescriptionTextBox.Text.Trim();

                if (_isNew) db.SaveChanges();

                UpdateVariant(db, product, 60, price60, stock60, alcoholPercent);
                UpdateVariant(db, product, 120, price120, stock120, alcoholPercent);

                if (Has60CheckBox.IsChecked == false)
                    db.ProductVariants.RemoveRange(db.ProductVariants.Where(v => v.ProductID == product.ProductID && v.VolumeML == 60));
                if (Has120CheckBox.IsChecked == false)
                    db.ProductVariants.RemoveRange(db.ProductVariants.Where(v => v.ProductID == product.ProductID && v.VolumeML == 120));

                // Обновление изображения
                string imageFileName = ImageFileNameTextBox.Text.Trim();
                if (!string.IsNullOrEmpty(imageFileName))
                {
                    var existingImages = db.ProductImages.Where(img => img.ProductID == product.ProductID).ToList();
                    if (existingImages.Any())
                    {
                        var mainImg = existingImages.FirstOrDefault(img => img.IsMain) ?? existingImages.First();
                        mainImg.ImageFileName = imageFileName;
                        mainImg.IsMain = true;
                        foreach (var img in existingImages.Where(i => i.ImageID != mainImg.ImageID))
                            db.ProductImages.Remove(img);
                    }
                    else
                    {
                        db.ProductImages.Add(new ProductImage { ProductID = product.ProductID, ImageFileName = imageFileName, IsMain = true });
                    }
                }

                // Ноты
                var currentNotes = db.ProductNotes.Where(pn => pn.ProductID == product.ProductID).ToList();
                db.ProductNotes.RemoveRange(currentNotes);
                foreach (Note note in NotesListBox.SelectedItems)
                    db.ProductNotes.Add(new ProductNote { ProductID = product.ProductID, NoteID = note.NoteID });

                db.SaveChanges();
            }

            DialogResult = true;
            Close();
        }

        private void UpdateVariant(AppDbContext db, Product product, int volume, decimal? price, int? stock, decimal alcoholPercent)
        {
            if (price == null || stock == null) return;
            var variant = db.ProductVariants.FirstOrDefault(v => v.ProductID == product.ProductID && v.VolumeML == volume);
            if (variant == null)
            {
                variant = new ProductVariant { ProductID = product.ProductID, VolumeML = volume, CreatedAt = DateTime.Now };
                db.ProductVariants.Add(variant);
            }
            variant.Price = price.Value;
            variant.StockQuantity = stock.Value;
            variant.AlcoholPercent = alcoholPercent;
        }

        private void ShowError(string message)
        {
            ErrorMessage.Text = message;
            ErrorMessage.Visibility = Visibility.Visible;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}