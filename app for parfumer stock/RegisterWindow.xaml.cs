using System;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;

namespace PerfumeWarehouseWPF
{
    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            ErrorMessage.Visibility = Visibility.Collapsed;

            string lastName = LastNameTextBox.Text.Trim();
            string firstName = FirstNameTextBox.Text.Trim();
            string middleName = MiddleNameTextBox.Text.Trim();
            string passport = PassportTextBox.Text.Trim();
            string phone = PhoneTextBox.Text.Trim();
            string email = EmailTextBox.Text.Trim();
            string login = LoginTextBox.Text.Trim();
            string password = PasswordBox.Password;
            string confirm = ConfirmPasswordBox.Password;

            if (string.IsNullOrEmpty(lastName) || string.IsNullOrEmpty(firstName) ||
                string.IsNullOrEmpty(passport) || string.IsNullOrEmpty(email) ||
                string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                ShowError("Все поля со звёздочкой обязательны");
                return;
            }

            if (!IsValidName(lastName) || !IsValidName(firstName))
            {
                ShowError("Фамилия и имя должны начинаться с заглавной буквы и содержать только кириллицу");
                return;
            }
            if (!string.IsNullOrEmpty(middleName) && !IsValidName(middleName))
            {
                ShowError("Отчество должно начинаться с заглавной буквы и содержать только кириллицу");
                return;
            }
            if (!Regex.IsMatch(passport, @"^\d{4}\s?\d{6}$"))
            {
                ShowError("Паспорт: 10 цифр (допустим пробел после 4-й)");
                return;
            }
            if (!string.IsNullOrEmpty(phone) && !Regex.IsMatch(phone, @"^(\+7|8)\d{10}$"))
            {
                ShowError("Телефон: +7XXXXXXXXXX или 8XXXXXXXXXX (11 цифр)");
                return;
            }
            if (!Regex.IsMatch(email, @"^[a-zA-Z][a-zA-Z0-9._%+-]*@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"))
            {
                ShowError("Некорректный email");
                return;
            }
            if (password != confirm)
            {
                ShowError("Пароли не совпадают");
                return;
            }
            if (password.Length < 6)
            {
                ShowError("Пароль минимум 6 символов");
                return;
            }

            using (var db = new AppDbContext())
            {
                if (db.Employees.Any(x => x.LoginID == login))
                {
                    ShowError("Логин уже используется");
                    return;
                }
                if (db.Employees.Any(x => x.Email == email))
                {
                    ShowError("Email уже используется");
                    return;
                }
                if (db.Employees.Any(x => x.PassportNumber == passport))
                {
                    ShowError("Паспорт уже зарегистрирован");
                    return;
                }

                var role = db.Roles.FirstOrDefault(r => r.RoleName == "storekeeper");
                if (role == null)
                {
                    role = new Role { RoleName = "storekeeper" };
                    db.Roles.Add(role);
                    db.SaveChanges();
                }

                db.Employees.Add(new Employee
                {
                    LoginID = login,
                    LastName = lastName,
                    FirstName = firstName,
                    MiddleName = string.IsNullOrEmpty(middleName) ? null : middleName,
                    PassportNumber = passport,
                    Phone = string.IsNullOrEmpty(phone) ? null : phone,
                    Email = email,
                    PasswordHash = PasswordHelper.HashPassword(password),
                    RoleID = role.RoleID,
                    CreatedAt = DateTime.Now
                });
                db.SaveChanges();
            }

            MessageBox.Show("Сотрудник успешно зарегистрирован!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ShowError(string msg)
        {
            ErrorMessage.Text = msg;
            ErrorMessage.Visibility = Visibility.Visible;
        }

        private bool IsValidName(string name)
        {
            return Regex.IsMatch(name, @"^[А-ЯЁ][а-яё]+$");
        }
    }
}