using System;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PerfumeWarehouseWPF
{
    public partial class FilterWindow : Window
    {
        public FilterSettings FilterSettings { get; private set; }
        private ObservableCollection<Note> allNotes;

        public FilterWindow(FilterSettings currentSettings)
        {
            InitializeComponent();
            FilterSettings = new FilterSettings
            {
                Volume60 = currentSettings.Volume60,
                Volume120 = currentSettings.Volume120,
                MinAlcohol = currentSettings.MinAlcohol,
                MaxAlcohol = currentSettings.MaxAlcohol,
                MinStock = currentSettings.MinStock,
                MaxStock = currentSettings.MaxStock,
                MinPrice = currentSettings.MinPrice,
                MaxPrice = currentSettings.MaxPrice,
                NoteIds = currentSettings.NoteIds.ToList()
            };

            Volume60CheckBox.IsChecked = FilterSettings.Volume60;
            Volume120CheckBox.IsChecked = FilterSettings.Volume120;
            MinAlcoholTextBox.Text = FilterSettings.MinAlcohol.ToString();
            MaxAlcoholTextBox.Text = FilterSettings.MaxAlcohol.ToString();
            MinStockTextBox.Text = FilterSettings.MinStock.ToString();
            MaxStockTextBox.Text = FilterSettings.MaxStock == int.MaxValue ? "9999" : FilterSettings.MaxStock.ToString();
            MinPriceTextBox.Text = FilterSettings.MinPrice.ToString();
            MaxPriceTextBox.Text = FilterSettings.MaxPrice == decimal.MaxValue ? "999999" : FilterSettings.MaxPrice.ToString();

            LoadNotes();
        }

        private void LoadNotes()
        {
            using (var db = new AppDbContext())
            {
                allNotes = new ObservableCollection<Note>(db.Notes.OrderBy(n => n.NoteName).ToList());
                NotesListBox.ItemsSource = allNotes;
                foreach (var note in allNotes)
                {
                    if (FilterSettings.NoteIds.Contains(note.NoteID))
                        NotesListBox.SelectedItems.Add(note);
                }
            }
        }

        private void NoteSearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string search = NoteSearchTextBox.Text.Trim().ToLower();
            if (string.IsNullOrEmpty(search))
            {
                NotesListBox.ItemsSource = allNotes.ToList();
            }
            else
            {
                NotesListBox.ItemsSource = allNotes.Where(n => n.NoteName.ToLower().Contains(search)).ToList();
            }
        }
        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            // Валидация алкоголя
            if (!decimal.TryParse(MinAlcoholTextBox.Text, out decimal minAlc) || minAlc < 0 || minAlc > 100)
            { MessageBox.Show("Минимальное содержание спирта должно быть числом от 0 до 100.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning); return; }
            if (!decimal.TryParse(MaxAlcoholTextBox.Text, out decimal maxAlc) || maxAlc < 0 || maxAlc > 100)
            { MessageBox.Show("Максимальное содержание спирта должно быть числом от 0 до 100.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning); return; }
            if (minAlc > maxAlc)
            { MessageBox.Show("Минимальное значение спирта не может быть больше максимального.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning); return; }

            // Валидация остатка
            if (!int.TryParse(MinStockTextBox.Text, out int minStock) || minStock < 0)
            { MessageBox.Show("Минимальный остаток должен быть целым неотрицательным числом.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning); return; }
            if (!int.TryParse(MaxStockTextBox.Text, out int maxStock) || maxStock < 0)
            { MessageBox.Show("Максимальный остаток должен быть целым неотрицательным числом.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning); return; }
            if (minStock > maxStock)
            { MessageBox.Show("Минимальный остаток не может быть больше максимального.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning); return; }

            // Валидация цены
            if (!decimal.TryParse(MinPriceTextBox.Text, out decimal minPrice) || minPrice < 0)
            { MessageBox.Show("Минимальная цена должна быть неотрицательным числом.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning); return; }
            if (!decimal.TryParse(MaxPriceTextBox.Text, out decimal maxPrice) || maxPrice < 0)
            { MessageBox.Show("Максимальная цена должна быть неотрицательным числом.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning); return; }
            if (minPrice > maxPrice)
            { MessageBox.Show("Минимальная цена не может быть больше максимальной.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning); return; }

            FilterSettings.Volume60 = Volume60CheckBox.IsChecked ?? false;
            FilterSettings.Volume120 = Volume120CheckBox.IsChecked ?? false;
            FilterSettings.MinAlcohol = minAlc;
            FilterSettings.MaxAlcohol = maxAlc;
            FilterSettings.MinStock = minStock;
            FilterSettings.MaxStock = maxStock;
            FilterSettings.MinPrice = minPrice;
            FilterSettings.MaxPrice = maxPrice;
            FilterSettings.NoteIds = NotesListBox.SelectedItems.Cast<Note>().Select(n => n.NoteID).ToList();

            DialogResult = true;
            Close();
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            FilterSettings = new FilterSettings();
            Volume60CheckBox.IsChecked = true;
            Volume120CheckBox.IsChecked = true;
            MinAlcoholTextBox.Text = "0";
            MaxAlcoholTextBox.Text = "100";
            MinStockTextBox.Text = "0";
            MaxStockTextBox.Text = "9999";
            MinPriceTextBox.Text = "0";
            MaxPriceTextBox.Text = "999999";
            NotesListBox.SelectedItems.Clear();
            NoteSearchTextBox.Text = "";
            NotesListBox.ItemsSource = allNotes;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}