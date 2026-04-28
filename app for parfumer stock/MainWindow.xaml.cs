using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace PerfumeWarehouseWPF
{
    public partial class MainWindow : Window
    {
        private ObservableCollection<ProductCardViewModel> _allProducts;
        private FilterSettings _currentFilter = new FilterSettings();

        
        public string RoleDisplay { get; set; }

     
        public MainWindow(string roleName)
        {
            InitializeComponent();
            RoleDisplay = $"Вы вошли как: {roleName}";
            DataContext = this; 
            LoadProducts();
        }

        private void LoadProducts()
        {
            using (var db = new AppDbContext())
            {
                var products = db.Products
                    .Include(p => p.Variants)
                    .Include(p => p.Images)
                    .Include(p => p.ProductNotes.Select(pn => pn.Note))
                    .ToList();

                _allProducts = new ObservableCollection<ProductCardViewModel>(
                    products.Select(p => new ProductCardViewModel(p))
                );
            }

            ApplyFiltersAndSearch();
        }

        private void ApplyFiltersAndSearch()
        {
            var filtered = _allProducts.AsEnumerable();

            string searchText = SearchTextBox.Text.Trim();
            if (!string.IsNullOrEmpty(searchText))
                filtered = filtered.Where(p => p.ProductName.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0);

            if (_currentFilter.Volume60 && !_currentFilter.Volume120)
                filtered = filtered.Where(p => p.HasVolume60);
            else if (!_currentFilter.Volume60 && _currentFilter.Volume120)
                filtered = filtered.Where(p => p.HasVolume120);

            if (_currentFilter.MinAlcohol > 0 || _currentFilter.MaxAlcohol < 100)
            {
                filtered = filtered.Where(p =>
                {
                    var variant60 = p.GetVariant(60);
                    var variant120 = p.GetVariant(120);
                    decimal? alc = null;
                    if (variant60 != null) alc = variant60.AlcoholPercent;
                    else if (variant120 != null) alc = variant120.AlcoholPercent;
                    return alc.HasValue && alc >= _currentFilter.MinAlcohol && alc <= _currentFilter.MaxAlcohol;
                });
            }

            if (_currentFilter.MinStock > 0 || _currentFilter.MaxStock < int.MaxValue)
            {
                filtered = filtered.Where(p =>
                {
                    var variant = p.GetVariant(p.IsVolume60Selected ? 60 : 120);
                    return variant != null && variant.StockQuantity >= _currentFilter.MinStock && variant.StockQuantity <= _currentFilter.MaxStock;
                });
            }

            if (_currentFilter.MinPrice > 0 || _currentFilter.MaxPrice < decimal.MaxValue)
            {
                filtered = filtered.Where(p =>
                {
                    var variant = p.GetVariant(p.IsVolume60Selected ? 60 : 120);
                    return variant != null && variant.Price >= _currentFilter.MinPrice && variant.Price <= _currentFilter.MaxPrice;
                });
            }

            if (_currentFilter.NoteIds.Any())
            {
                filtered = filtered.Where(p =>
                {
                    using (var db = new AppDbContext())
                    {
                        var productNotes = db.ProductNotes
                            .Where(pn => pn.ProductID == p.ProductId)
                            .Select(pn => pn.NoteID)
                            .ToList();
                        return _currentFilter.NoteIds.All(id => productNotes.Contains(id));
                    }
                });
            }

            ProductsItemsControl.ItemsSource = new ObservableCollection<ProductCardViewModel>(filtered);
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e) => ApplyFiltersAndSearch();
        private void ResetSearchButton_Click(object sender, RoutedEventArgs e)
        {
            SearchTextBox.Text = "";
            _currentFilter = new FilterSettings();
            ApplyFiltersAndSearch();
        }

        private void FilterButton_Click(object sender, RoutedEventArgs e)
        {
            var filterWindow = new FilterWindow(_currentFilter);
            filterWindow.Owner = this;
            if (filterWindow.ShowDialog() == true)
            {
                _currentFilter = filterWindow.FilterSettings;
                ApplyFiltersAndSearch();
            }
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            new LoginWindow().Show();
            Close();
        }

        private void OrdersButton_Click(object sender, RoutedEventArgs e)
        {
            var ordersWindow = new OrdersWindow();
            ordersWindow.Owner = this;
            ordersWindow.ShowDialog();
        }

      
        private void CreateOrderButton_Click(object sender, RoutedEventArgs e)
        {
            var createOrderWindow = new CreateOrderWindow();
            createOrderWindow.Owner = this;
            createOrderWindow.ShowDialog();
            LoadProducts();
        }

        private void DetailsButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.Tag is int productId)
            {
                var detailsWindow = new ProductDetailWindow(productId);
                detailsWindow.Owner = this;
                detailsWindow.ShowDialog();
            }
        }

        //   Управление товарами  
        private void AddProductButton_Click(object sender, RoutedEventArgs e)
        {
            var window = new ProductEditWindow();
            window.Owner = this;
            if (window.ShowDialog() == true)
                LoadProducts();
        }

        private void EditProductButton_Click(object sender, RoutedEventArgs e)
        {
            var selected = ProductsItemsControl.ItemsSource?.OfType<ProductCardViewModel>().FirstOrDefault(p => p.IsSelected);
            if (selected == null)
            {
                MessageBox.Show("Выберите товар для редактирования.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            using (var db = new AppDbContext())
            {
                var product = db.Products
                    .Include(p => p.Variants)
                    .Include(p => p.Images)
                    .Include(p => p.ProductNotes)
                    .FirstOrDefault(p => p.ProductID == selected.ProductId);
                if (product != null)
                {
                    var window = new ProductEditWindow(product);
                    window.Owner = this;
                    if (window.ShowDialog() == true)
                        LoadProducts();
                }
            }
        }

        private void DeleteProductButton_Click(object sender, RoutedEventArgs e)
        {
            var selected = ProductsItemsControl.ItemsSource?.OfType<ProductCardViewModel>().FirstOrDefault(p => p.IsSelected);
            if (selected == null)
            {
                MessageBox.Show("Выберите товар для удаления.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (MessageBox.Show($"Удалить товар '{selected.ProductName}'?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                using (var db = new AppDbContext())
                {
                    var product = db.Products.Find(selected.ProductId);
                    if (product != null)
                    {
                        db.Products.Remove(product);
                        db.SaveChanges();
                        LoadProducts();
                    }
                }
            }
        }

        private void ProductBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var border = sender as Border;
            var vm = border.DataContext as ProductCardViewModel;
            if (vm != null)
            {
                foreach (var item in ProductsItemsControl.ItemsSource.OfType<ProductCardViewModel>())
                    item.IsSelected = false;
                vm.IsSelected = true;
            }
        }
    }

    //  Фильтры                               
    public class FilterSettings
    {
        public bool Volume60 { get; set; } = true;
        public bool Volume120 { get; set; } = true;
        public decimal MinAlcohol { get; set; } = 0;
        public decimal MaxAlcohol { get; set; } = 100;
        public int MinStock { get; set; } = 0;
        public int MaxStock { get; set; } = int.MaxValue;
        public decimal MinPrice { get; set; } = 0;
        public decimal MaxPrice { get; set; } = decimal.MaxValue;
        public System.Collections.Generic.List<int> NoteIds { get; set; } = new System.Collections.Generic.List<int>();
    }

    //  ViewModel карточки    
    public class ProductCardViewModel : INotifyPropertyChanged
    {
        private readonly Product _product;
        private ProductVariant _selectedVariant;
        private bool _isVolume60Selected;
        private bool _isVolume120Selected;
        private BitmapImage _imageSource;
        private bool _isSelected;

        public event PropertyChangedEventHandler PropertyChanged;

        public ProductCardViewModel(Product product)
        {
            _product = product;
            ProductId = product.ProductID;
            ProductName = product.ProductName;

            HasVolume60 = product.Variants.Any(v => v.VolumeML == 60);
            HasVolume120 = product.Variants.Any(v => v.VolumeML == 120);

            if (HasVolume60) IsVolume60Selected = true;
            else if (HasVolume120) IsVolume120Selected = true;
            else IsVolume60Selected = true;

            VolumeGroupName = "Volume_" + product.ProductID;
            LoadImage();
            UpdateSelectedVariant();
        }

        private void LoadImage()
        {
            var mainImage = _product.Images.FirstOrDefault(img => img.IsMain)
                            ?? _product.Images.FirstOrDefault();
            string fileName = mainImage?.ImageFileName ?? "no_image.png";
            string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", fileName);

            try
            {
                var bmp = new BitmapImage();
                bmp.BeginInit();
                bmp.UriSource = new Uri(fullPath, UriKind.Absolute);
                bmp.CacheOption = BitmapCacheOption.OnLoad;
                bmp.EndInit();
                _imageSource = bmp;
            }
            catch
            {
                string fallbackPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", "no_image.png");
                try
                {
                    var bmp = new BitmapImage();
                    bmp.BeginInit();
                    bmp.UriSource = new Uri(fallbackPath, UriKind.Absolute);
                    bmp.CacheOption = BitmapCacheOption.OnLoad;
                    bmp.EndInit();
                    _imageSource = bmp;
                }
                catch { _imageSource = null; }
            }
        }

        public int ProductId { get; }
        public string ProductName { get; }
        public string VolumeGroupName { get; }
        public BitmapImage ImageSource => _imageSource;
        public bool HasVolume60 { get; }
        public bool HasVolume120 { get; }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
                }
            }
        }

        public ProductVariant GetVariant(int volume) => _product.Variants.FirstOrDefault(v => v.VolumeML == volume);

        public bool IsVolume60Selected
        {
            get => _isVolume60Selected;
            set
            {
                if (_isVolume60Selected != value)
                {
                    _isVolume60Selected = value;
                    if (value) IsVolume120Selected = false;
                    OnPropertyChanged(nameof(IsVolume60Selected));
                    UpdateSelectedVariant();
                }
            }
        }

        public bool IsVolume120Selected
        {
            get => _isVolume120Selected;
            set
            {
                if (_isVolume120Selected != value)
                {
                    _isVolume120Selected = value;
                    if (value) IsVolume60Selected = false;
                    OnPropertyChanged(nameof(IsVolume120Selected));
                    UpdateSelectedVariant();
                }
            }
        }

        public string DisplayPrice
        {
            get
            {
                if (_selectedVariant != null)
                    return $"{_selectedVariant.Price:N0} ₽";
                return "Цена не указана";
            }
        }

        public string StockInfo
        {
            get
            {
                if (_selectedVariant == null) return "";
                if (_selectedVariant.StockQuantity == 0) return "НЕТ В НАЛИЧИИ";
                return $"В наличии: {_selectedVariant.StockQuantity} шт.";
            }
        }

        public string StockColor
        {
            get
            {
                if (_selectedVariant != null && _selectedVariant.StockQuantity == 0) return "Red";
                return "Gray";
            }
        }

        private void UpdateSelectedVariant()
        {
            int volume = IsVolume60Selected ? 60 : 120;
            _selectedVariant = _product.Variants.FirstOrDefault(v => v.VolumeML == volume);
            OnPropertyChanged(nameof(DisplayPrice));
            OnPropertyChanged(nameof(StockInfo));
            OnPropertyChanged(nameof(StockColor));
        }

        protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
  

    public class SelectedToBorderConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value is bool b && b) ? System.Windows.Media.Brushes.DodgerBlue : System.Windows.Media.Brushes.LightGray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}