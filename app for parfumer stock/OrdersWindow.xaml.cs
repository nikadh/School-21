using System;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PerfumeWarehouseWPF
{
    public partial class OrdersWindow : Window
    {
        private ObservableCollection<OrderListItem> allOrders;
        private Order selectedOrder;

        public OrdersWindow()
        {
            InitializeComponent();
            LoadOrders();
            LoadStatuses();
        }

        private void LoadOrders()
        {
            using (var db = new AppDbContext())
            {
                var orders = db.Orders
                    .Include(o => o.Customer)
                    .Include(o => o.Status)
                    .OrderByDescending(o => o.OrderDate)
                    .ToList();

                allOrders = new ObservableCollection<OrderListItem>(
                    orders.Select(o => new OrderListItem
                    {
                        OrderID = o.OrderID,
                        OrderDate = o.OrderDate,
                        CustomerFullName = o.Customer != null
                            ? $"{o.Customer.LastName} {o.Customer.FirstName} {o.Customer.MiddleName ?? ""}".Trim()
                            : "—",
                        DisplayText = $"Заказ №{o.OrderID} от {o.OrderDate:dd.MM.yyyy}"
                    })
                );

                OrdersListBox.ItemsSource = allOrders;
            }
        }

        private void LoadStatuses()
        {
            using (var db = new AppDbContext())
            {
                var statuses = db.OrderStatuses.ToList();
                StatusComboBox.ItemsSource = statuses;
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(SearchTextBox.Text.Trim(), out int orderId))
            {
                var found = allOrders.FirstOrDefault(o => o.OrderID == orderId);
                if (found != null)
                {
                    OrdersListBox.SelectedItem = found;
                    OrdersListBox.ScrollIntoView(found);
                }
                else
                {
                    MessageBox.Show($"Заказ №{orderId} не найден.", "Поиск", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("Введите корректный номер заказа.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            SearchTextBox.Text = "";
            OrdersListBox.SelectedItem = null;
            ClearDetails();
            OrdersListBox.ItemsSource = allOrders;
        }

        private void OrdersListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (OrdersListBox.SelectedItem is OrderListItem selected)
            {
                LoadOrderDetails(selected.OrderID);
            }
            else
            {
                ClearDetails();
            }
        }

        private void LoadOrderDetails(int orderId)
        {
            using (var db = new AppDbContext())
            {
                selectedOrder = db.Orders
                    .Include(o => o.Customer)
                    .Include(o => o.Status)
                    .Include(o => o.OrderItems.Select(oi => oi.Variant.Product))
                    .FirstOrDefault(o => o.OrderID == orderId);

                if (selectedOrder == null)
                {
                    ClearDetails();
                    return;
                }

                // Клиент
                string customerInfo = selectedOrder.Customer != null
                    ? $"{selectedOrder.Customer.LastName} {selectedOrder.Customer.FirstName} {selectedOrder.Customer.MiddleName ?? ""}\n" +
                      $"Телефон: {selectedOrder.Customer.Phone ?? "—"}\n" +
                      $"Email: {selectedOrder.Customer.Email ?? "—"}"
                    : "Клиент не указан";
                CustomerInfoText.Text = customerInfo;

                // Статус
                StatusComboBox.SelectedValue = selectedOrder.StatusID;

                // Сумма
                TotalAmountText.Text = $"{selectedOrder.TotalAmount:N0} ₽ (скидка: {selectedOrder.DiscountAmount:N0} ₽, итого: {selectedOrder.FinalAmount:N0} ₽)";

                // Комментарий
                CommentTextBox.Text = selectedOrder.Comment ?? "";

                // Товары
                var items = selectedOrder.OrderItems.Select(oi => new OrderItemDetail
                {
                    ProductName = oi.Variant?.Product?.ProductName ?? "Неизвестный товар",
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice
                }).ToList();
                OrderItemsControl.ItemsSource = items;
            }
        }

        private void ClearDetails()
        {
            CustomerInfoText.Text = "";
            StatusComboBox.SelectedIndex = -1;
            TotalAmountText.Text = "";
            CommentTextBox.Text = "";
            OrderItemsControl.ItemsSource = null;
            selectedOrder = null;
        }

        private void SaveStatusButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedOrder == null)
            {
                MessageBox.Show("Выберите заказ.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (StatusComboBox.SelectedValue == null)
            {
                MessageBox.Show("Выберите статус.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int newStatusId = (int)StatusComboBox.SelectedValue;
            using (var db = new AppDbContext())
            {
                var order = db.Orders.Find(selectedOrder.OrderID);
                if (order != null)
                {
                    order.StatusID = newStatusId;
                    db.SaveChanges();
                    MessageBox.Show("Статус обновлён.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    // Обновляем данные в окне
                    LoadOrders();
                    LoadOrderDetails(order.OrderID);
                }
            }
        }

        private void SaveCommentButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedOrder == null)
            {
                MessageBox.Show("Выберите заказ.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            string newComment = CommentTextBox.Text;
            using (var db = new AppDbContext())
            {
                var order = db.Orders.Find(selectedOrder.OrderID);
                if (order != null)
                {
                    order.Comment = newComment;
                    db.SaveChanges();
                    MessageBox.Show("Комментарий сохранён.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadOrders();
            ClearDetails();
            OrdersListBox.SelectedItem = null;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }

    public class OrderListItem
    {
        public int OrderID { get; set; }
        public DateTime? OrderDate { get; set; }
        public string CustomerFullName { get; set; }
        public string DisplayText { get; set; }
    }

    public class OrderItemDetail
    {
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}