using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Windows;

namespace PerfumeWarehouseWPF
{
    public partial class ProductDetailWindow : Window
    {
        public ProductDetailWindow(int productId)
        {
            InitializeComponent();
            LoadProductDetails(productId);
        }

        private void LoadProductDetails(int productId)
        {
            using (var db = new AppDbContext())
            {
                var product = db.Products
                    .Include(p => p.Brand)
                    .Include(p => p.Variants)
                    .Include(p => p.ProductNotes.Select(pn => pn.Note))
                    .FirstOrDefault(p => p.ProductID == productId);

                if (product == null)
                {
                    MessageBox.Show("Товар не найден.");
                    Close();
                    return;
                }

                DataContext = new ProductDetailViewModel
                {
                    ProductName = product.ProductName,
                    BrandName = product.Brand?.BrandName ?? "—",
                    Description = product.Description ?? "Описание отсутствует.",
                    AlcoholPercent = product.Variants.FirstOrDefault()?.AlcoholPercent.ToString("F1") + " %" ?? "—",
                    Notes = new ObservableCollection<NoteViewModel>(
                        product.ProductNotes.Select(pn => new NoteViewModel
                        {
                            NoteName = pn.Note.NoteName,
                            NoteType = pn.Note.NoteType ?? "—"
                        }))
                };
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }

    public class ProductDetailViewModel
    {
        public string ProductName { get; set; }
        public string BrandName { get; set; }
        public string AlcoholPercent { get; set; }
        public string Description { get; set; }
        public ObservableCollection<NoteViewModel> Notes { get; set; }
    }

    public class NoteViewModel
    {
        public string NoteName { get; set; }
        public string NoteType { get; set; }
    }
}