using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace PerfumeWarehouseWPF
{
    public partial class LoginWindow : Window
    {
        private int failedAttempts = 0;
        private DateTime? lockEndTime = null;

        public LoginWindow()
        {
            InitializeComponent();
            LoadLogo();
        }

        private void LoadLogo()
        {
            string logoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", "logo.png");
            if (File.Exists(logoPath))
            {
                try
                {
                    var bmp = new BitmapImage();
                    bmp.BeginInit();
                    bmp.UriSource = new Uri(logoPath, UriKind.Absolute);
                    bmp.CacheOption = BitmapCacheOption.OnLoad;
                    bmp.EndInit();
                    LogoImage.Source = bmp;
                }
                catch
                {
                    LogoImage.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                LogoImage.Visibility = Visibility.Collapsed;
            }
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (lockEndTime.HasValue && DateTime.Now < lockEndTime.Value)
            {
                var remaining = lockEndTime.Value - DateTime.Now;
                ShowError($"Вы заблокированы. Попробуйте через {remaining.Minutes} мин {remaining.Seconds} сек.");
                return;
            }

            string login = LoginTextBox.Text.Trim();
            string password = PasswordBox.Password;

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                ShowError("Введите логин и пароль");
                return;
            }

            using (var db = new AppDbContext())
            {
                var employee = db.Employees.Include("Role").FirstOrDefault(emp => emp.LoginID == login);
                if (employee != null && PasswordHelper.VerifyPassword(password, employee.PasswordHash))
                {
                    string roleName = employee.Role?.RoleName ?? "Сотрудник";
                    new MainWindow(roleName).Show();
                    Close();
                }
                else
                {
                    failedAttempts++;
                    int remaining = 3 - failedAttempts;
                    if (remaining > 0)
                        ShowError($"Неверный логин или пароль. Осталось попыток: {remaining}");
                    else
                    {
                        lockEndTime = DateTime.Now.AddMinutes(1);
                        ShowError("Вы заблокированы на 1 минуту.");
                        LoginButton.IsEnabled = false;
                        var timer = new DispatcherTimer { Interval = TimeSpan.FromMinutes(1) };
                        timer.Tick += (s, args) =>
                        {
                            LoginButton.IsEnabled = true;
                            ErrorMessage.Visibility = Visibility.Collapsed;
                            timer.Stop();
                        };
                        timer.Start();
                    }
                }
            }
        }

        private void ShowError(string msg)
        {
            ErrorMessage.Text = msg;
            ErrorMessage.Visibility = Visibility.Visible;
        }

        private void RegisterLink_Click(object sender, RoutedEventArgs e)
        {
            new RegisterWindow().ShowDialog();
        }
    }
}