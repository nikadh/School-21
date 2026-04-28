using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace PerfumeWarehouseWPF
{
    public partial class CustomSetWindow : Window
    {
        private ObservableCollection<ProductItem> _allProducts;
        public ObservableCollection<ProductItem> SelectedProducts { get; } = new ObservableCollection<ProductItem>();

        public CustomSetWindow()
        {
            InitializeComponent();
            LoadProducts();
            ProductsListBox.ItemsSource = _allProducts;
            ProductsListBox.SelectionChanged += ProductsListBox_SelectionChanged;
            UpdateSelectionCount();
        }

        private void LoadProducts()
        {
            using (var db = new AppDbContext())
            {
                var products = db.Products
                    .Where(p => p.ProductName != "Авторский набор пробников")
                    .Include(p => p.Images)
                    .OrderBy(p => p.ProductName)
                    .ToList();
                _allProducts = new ObservableCollection<ProductItem>(
                    products.Select(p => new ProductItem
                    {
                        ProductID = p.ProductID,
                        ProductName = p.ProductName,
                        ImageFileName = p.Images.FirstOrDefault()?.ImageFileName
                    })
                );
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var search = SearchTextBox.Text.Trim().ToLower();
            var view = CollectionViewSource.GetDefaultView(_allProducts);
            view.Filter = item =>
            {
                var product = item as ProductItem;
                return string.IsNullOrEmpty(search) || product.ProductName.ToLower().Contains(search);
            };
        }

        private void ProductsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (ProductItem item in e.AddedItems)
                if (!SelectedProducts.Contains(item)) SelectedProducts.Add(item);
            foreach (ProductItem item in e.RemovedItems)
                SelectedProducts.Remove(item);
            UpdateSelectionCount();
        }

        private void UpdateSelectionCount()
        {
            SelectionCountText.Text = $"Выбрано: {SelectedProducts.Count} / 6";
        }

        private void AddToCartButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedProducts.Count != 6)
            {
                MessageBox.Show("Необходимо выбрать ровно 6 ароматов.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (SelectedProducts.Select(p => p.ProductID).Distinct().Count() != 6)
            {
                MessageBox.Show("Выбранные ароматы должны быть разными.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }

    public class ProductItem : INotifyPropertyChanged
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public string ImageFileName { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string prop) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}