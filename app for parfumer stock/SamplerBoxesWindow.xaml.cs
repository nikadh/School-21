using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;

namespace PerfumeWarehouseWPF
{
    public partial class SamplerBoxesWindow : Window
    {
        public SamplerBoxesWindow()
        {
            InitializeComponent();
            LoadBoxes();
        }

        private void LoadBoxes()
        {
            var boxViewModels = new ObservableCollection<SamplerBoxViewModel>();

            using (var db = new AppDbContext())
            {
                var boxes = db.SamplerBoxes
                    .Include(b => b.Items.Select(i => i.Product))
                    .Where(b => b.IsActive == true)
                    .ToList();

                foreach (var box in boxes)
                {
                    boxViewModels.Add(new SamplerBoxViewModel(box));
                }
            }

            BoxesItemsControl.ItemsSource = boxViewModels;
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadBoxes();
        }
    }

    public class SamplerBoxViewModel : INotifyPropertyChanged
    {
        private readonly SamplerBox _box;
        private BitmapImage _imageSource;

        public event PropertyChangedEventHandler PropertyChanged;

        public SamplerBoxViewModel(SamplerBox box)
        {
            _box = box;
            LoadImage();
        }

        private void LoadImage()
        {
            string fileName = _box.ImageFileName ?? "no_image.png";
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

        public string BoxName => _box.BoxName;
        public string Description => _box.Description ?? "Описание отсутствует";
        public BitmapImage ImageSource => _imageSource;
        public string PriceText => $"{_box.Price:N0} ₽";
        public string StockText => _box.StockQuantity == 0 ? "НЕТ В НАЛИЧИИ" : $"В наличии: {_box.StockQuantity} шт.";
        public string StockColor => _box.StockQuantity == 0 ? "Red" : "Gray";

        public ObservableCollection<BoxItemViewModel> Items =>
            new ObservableCollection<BoxItemViewModel>(
                _box.Items.Select(i => new BoxItemViewModel
                {
                    DisplayText = $"{i.Product?.ProductName ?? "—"} ({i.VolumeML} мл) x{i.Quantity}"
                })
            );

        protected void OnPropertyChanged(string prop) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }

    public class BoxItemViewModel
    {
        public string DisplayText { get; set; }
    }
}