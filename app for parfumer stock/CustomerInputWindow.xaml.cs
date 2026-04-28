using System;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace PerfumeWarehouseWPF
{
    public partial class CustomerInputWindow : Window
    {
        public Customer SelectedCustomer { get; private set; }
        public string Comment => CommentTextBox.Text.Trim();

        public CustomerInputWindow()
        {
            InitializeComponent();
            LoadExistingCustomers();
        }

        private void LoadExistingCustomers()
        {
            using (var db = new AppDbContext())
            {
                var customers = db.Customers
                    .OrderBy(c => c.LastName)
                    .ThenBy(c => c.FirstName)
                    .ToList();

                var items = customers.Select(c => new CustomerComboItem
                {
                    CustomerID = c.CustomerID,
                    DisplayName = $"{c.LastName} {c.FirstName} {c.MiddleName ?? ""} ({c.Phone})"
                }).ToList();

                items.Insert(0, new CustomerComboItem { CustomerID = 0, DisplayName = "-- Выберите клиента --" });

                ExistingCustomerComboBox.ItemsSource = items;
                ExistingCustomerComboBox.SelectedIndex = 0;
            }
        }

        private void ExistingCustomerComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ExistingCustomerComboBox.SelectedItem is CustomerComboItem item && item.CustomerID > 0)
            {
                using (var db = new AppDbContext())
                {
                    var customer = db.Customers.Find(item.CustomerID);
                    if (customer != null)
                    {
                        LastNameTextBox.Text = customer.LastName;
                        FirstNameTextBox.Text = customer.FirstName;
                        MiddleNameTextBox.Text = customer.MiddleName ?? "";
                        PhoneTextBox.Text = customer.Phone;
                        EmailTextBox.Text = customer.Email ?? "";
                    }
                }
            }
            else
            {
                LastNameTextBox.Text = "";
                FirstNameTextBox.Text = "";
                MiddleNameTextBox.Text = "";
                PhoneTextBox.Text = "";
                EmailTextBox.Text = "";
            }
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            // Проверяем, выбран ли существующий клиент
            if (ExistingCustomerComboBox.SelectedItem is CustomerComboItem item && item.CustomerID > 0)
            {
                using (var db = new AppDbContext())
                {
                    SelectedCustomer = db.Customers.Find(item.CustomerID);
                }
                DialogResult = true;
                Close();
                return;
            }

            // Валидация полей нового клиента
            string lastName = LastNameTextBox.Text.Trim();
            string firstName = FirstNameTextBox.Text.Trim();
            string phone = PhoneTextBox.Text.Trim();

            if (string.IsNullOrEmpty(lastName) || string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(phone))
            {
                MessageBox.Show("Фамилия, имя и телефон обязательны.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!Regex.IsMatch(lastName, @"^[А-ЯЁ][а-яё]+$") || !Regex.IsMatch(firstName, @"^[А-ЯЁ][а-яё]+$"))
            {
                MessageBox.Show("Фамилия и имя должны начинаться с заглавной буквы и содержать только кириллицу.",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string middleName = MiddleNameTextBox.Text.Trim();
            if (!string.IsNullOrEmpty(middleName) && !Regex.IsMatch(middleName, @"^[А-ЯЁ][а-яё]+$"))
            {
                MessageBox.Show("Отчество должно начинаться с заглавной буквы и содержать только кириллицу.",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!Regex.IsMatch(phone, @"^(\+7|8)\d{10}$"))
            {
                MessageBox.Show("Телефон должен быть в формате +7XXXXXXXXXX или 8XXXXXXXXXX (11 цифр).",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string email = EmailTextBox.Text.Trim();
            if (!string.IsNullOrEmpty(email) && !Regex.IsMatch(email, @"^[a-zA-Z][a-zA-Z0-9._%+-]*@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"))
            {
                MessageBox.Show("Некорректный email.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Создаём нового клиента
            using (var db = new AppDbContext())
            {
                var newCustomer = new Customer
                {
                    LastName = lastName,
                    FirstName = firstName,
                    MiddleName = string.IsNullOrEmpty(middleName) ? null : middleName,
                    Phone = phone,
                    Email = string.IsNullOrEmpty(email) ? null : email,
                    CreatedAt = DateTime.Now
                };

                db.Customers.Add(newCustomer);
                db.SaveChanges();

                SelectedCustomer = newCustomer;
            }

            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private class CustomerComboItem
        {
            public int CustomerID { get; set; }
            public string DisplayName { get; set; }
        }
    }
}