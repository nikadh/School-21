using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace PerfumeWarehouseWPF
{
    public partial class CreateOrderWindow : Window
    {
        private ObservableCollection<CartItem> cartItems = new ObservableCollection<CartItem>();
        private int _customBoxVariantId;

        public CreateOrderWindow()
        {
            InitializeComponent();
            LoadCatalog();
            CartListBox.ItemsSource = cartItems;

            using (var db = new AppDbContext())
            {
                var customBoxProduct = db.Products.FirstOrDefault(p => p.ProductName == "Авторский набор пробников");
                if (customBoxProduct != null)
                {
                    var variant = db.ProductVariants.FirstOrDefault(v => v.ProductID == customBoxProduct.ProductID && v.VolumeML == 2);
                    if (variant != null)
                        _customBoxVariantId = variant.VariantID;
                    else
                        MessageBox.Show("Вариант авторского набора (2 мл) не найден в БД.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    MessageBox.Show("Товар 'Авторский набор пробников' не найден в БД.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void LoadCatalog()
        {
            var catalogItems = new ObservableCollection<CatalogItemViewModel>();

            using (var db = new AppDbContext())
            {
                var products = db.Products
                    .Where(p => p.ProductName != "Авторский набор пробников")
                    .Include(p => p.Variants)
                    .Include(p => p.Images)
                    .ToList();

                foreach (var product in products)
                    catalogItems.Add(new CatalogItemViewModel(product));
            }

            CatalogItemsControl.ItemsSource = catalogItems;
        }

        private void QuantityTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Regex.IsMatch(e.Text, "^[0-9]+$");
        }

        private void AddToCartButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.Tag is CatalogItemViewModel item)
            {
                if (item.SelectedVariant == null)
                {
                    MessageBox.Show("Выберите объём.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (item.Quantity <= 0)
                {
                    MessageBox.Show("Введите количество больше 0.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (item.Quantity > item.SelectedVariant.StockQuantity)
                {
                    MessageBox.Show($"Недостаточно товара на складе. Доступно: {item.SelectedVariant.StockQuantity}",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var existing = cartItems.FirstOrDefault(c => c.VariantID == item.SelectedVariant.VariantID);
                if (existing != null)
                {
                    if (existing.Quantity + item.Quantity > item.SelectedVariant.StockQuantity)
                    {
                        MessageBox.Show($"Общее количество превышает остаток. Доступно: {item.SelectedVariant.StockQuantity}",
                            "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                    existing.Quantity += item.Quantity;
                    existing.UpdateDisplayText();
                }
                else
                {
                    cartItems.Add(new CartItem
                    {
                        VariantID = item.SelectedVariant.VariantID,
                        ProductName = item.ProductName,
                        VolumeML = item.SelectedVariant.VolumeML,
                        Quantity = item.Quantity,
                        UnitPrice = item.SelectedVariant.Price,
                        DisplayText = $"{item.ProductName} ({item.SelectedVariant.VolumeML} мл) x{item.Quantity} = {item.SelectedVariant.Price * item.Quantity:N0} ₽"
                    });
                }

                UpdateTotalAmount();
            }
        }

        private void CustomSetButton_Click(object sender, RoutedEventArgs e)
        {
            if (_customBoxVariantId == 0)
            {
                MessageBox.Show("Невозможно добавить авторский набор: вариант не найден.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var window = new CustomSetWindow();
            window.Owner = this;
            if (window.ShowDialog() == true)
            {
                var cartItem = new CartItem
                {
                    VariantID = _customBoxVariantId,
                    ProductName = "Авторский набор (6 пробников)",
                    VolumeML = 2,
                    Quantity = 1,
                    UnitPrice = 3000,
                    IsCustomBox = true,
                    CustomBoxProducts = window.SelectedProducts.Select(p => p.ProductID).ToList()
                };
                cartItem.UpdateDisplayText();
                cartItems.Add(cartItem);
                UpdateTotalAmount();
            }
        }

        private void UpdateTotalAmount()
        {
            decimal total = cartItems.Sum(c => c.UnitPrice * c.Quantity);
            TotalAmountText.Text = $"Итого: {total:N0} ₽";
        }

        private void ClearCartButton_Click(object sender, RoutedEventArgs e)
        {
            cartItems.Clear();
            UpdateTotalAmount();
        }

        private void CheckoutButton_Click(object sender, RoutedEventArgs e)
        {
            if (cartItems.Count == 0)
            {
                MessageBox.Show("Корзина пуста.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (var dbCheck = new AppDbContext())
            {
                foreach (var item in cartItems)
                {
                    if (!dbCheck.ProductVariants.Any(v => v.VariantID == item.VariantID))
                    {
                        MessageBox.Show($"Ошибка: вариант товара с ID {item.VariantID} не найден в базе.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
            }

            var customerWindow = new CustomerInputWindow();
            customerWindow.Owner = this;
            if (customerWindow.ShowDialog() == true)
            {
                using (var db = new AppDbContext())
                {
                    var order = new Order
                    {
                        CustomerID = customerWindow.SelectedCustomer.CustomerID,
                        OrderDate = DateTime.Now,
                        StatusID = 1,
                        TotalAmount = cartItems.Sum(c => c.Quantity * c.UnitPrice),
                        DiscountAmount = 0,
                        EmployeeID = 1,
                        Comment = customerWindow.Comment
                    };

                    db.Orders.Add(order);
                    db.SaveChanges();

                    foreach (var cartItem in cartItems)
                    {
                        db.OrderItems.Add(new OrderItem
                        {
                            OrderID = order.OrderID,
                            VariantID = cartItem.VariantID,
                            Quantity = cartItem.Quantity,
                            UnitPrice = cartItem.UnitPrice
                        });
                    }

                    var customItems = cartItems.Where(c => c.IsCustomBox).ToList();
                    if (customItems.Any())
                    {
                        var compositions = new List<string>();
                        foreach (var ci in customItems)
                        {
                            var productNames = ci.CustomBoxProducts.Select(id => db.Products.Find(id)?.ProductName ?? id.ToString());
                            compositions.Add($"{ci.ProductName}: {string.Join(", ", productNames)}");
                        }
                        order.Comment = (order.Comment ?? "") + " | " + string.Join("; ", compositions);
                    }

                    db.SaveChanges();

                    int newOrderId = order.OrderID;
                    MessageBox.Show($"Заказ №{newOrderId} успешно создан.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    DialogResult = true;
                    Close();
                }
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e) => Close();
    }

    // Вспомогательные классы 
    public class CatalogItemViewModel : INotifyPropertyChanged
    {
        private readonly Product _product;
        private ProductVariant _selectedVariant;
        private bool _isVolume60Selected;
        private bool _isVolume120Selected;
        private int _quantity = 1;
        private BitmapImage _imageSource;

        public event PropertyChangedEventHandler PropertyChanged;

        public CatalogItemViewModel(Product product)
        {
            _product = product;
            ProductName = product.ProductName;

            bool has60 = product.Variants.Any(v => v.VolumeML == 60);
            bool has120 = product.Variants.Any(v => v.VolumeML == 120);

            if (has60) IsVolume60Selected = true;
            else if (has120) IsVolume120Selected = true;

            VolumeGroupName = "Vol_" + product.ProductID;
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
                _imageSource = null;
            }
        }

        public string ProductName { get; }
        public string VolumeGroupName { get; }
        public BitmapImage ImageSource => _imageSource;
        public ProductVariant SelectedVariant { get; private set; }

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
                if (SelectedVariant != null)
                    return $"{SelectedVariant.Price:N0} ₽";
                return "—";
            }
        }

        public string StockInfo
        {
            get
            {
                if (SelectedVariant == null) return "";
                if (SelectedVariant.StockQuantity == 0) return "НЕТ В НАЛИЧИИ";
                return $"В наличии: {SelectedVariant.StockQuantity}";
            }
        }

        public string StockColor
        {
            get
            {
                if (SelectedVariant != null && SelectedVariant.StockQuantity == 0)
                    return "Red";
                return "Gray";
            }
        }

        public int Quantity
        {
            get => _quantity;
            set
            {
                if (_quantity != value)
                {
                    _quantity = value;
                    OnPropertyChanged(nameof(Quantity));
                }
            }
        }

        private void UpdateSelectedVariant()
        {
            int volume = IsVolume60Selected ? 60 : 120;
            SelectedVariant = _product.Variants.FirstOrDefault(v => v.VolumeML == volume);
            OnPropertyChanged(nameof(DisplayPrice));
            OnPropertyChanged(nameof(StockInfo));
            OnPropertyChanged(nameof(StockColor));
        }

        protected void OnPropertyChanged(string prop) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }

    public class CartItem : INotifyPropertyChanged
    {
        public int VariantID { get; set; }
        public string ProductName { get; set; }
        public int VolumeML { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public bool IsCustomBox { get; set; }
        public List<int> CustomBoxProducts { get; set; }

        private string _displayText;
        public string DisplayText
        {
            get => _displayText;
            set { _displayText = value; OnPropertyChanged(nameof(DisplayText)); }
        }

        public void UpdateDisplayText()
        {
            if (IsCustomBox)
                DisplayText = $"{ProductName} = {UnitPrice * Quantity:N0} ₽";
            else
                DisplayText = $"{ProductName} ({VolumeML} мл) x{Quantity} = {UnitPrice * Quantity:N0} ₽";
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string prop) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}