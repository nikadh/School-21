using System.Windows;

namespace DocxToHtmlConverter
{
    public partial class LoginWindow : Window
    {
        // Конструктор окна входа
        public LoginWindow()
        {
            InitializeComponent(); // Инициализация компонентов XAML (генерируется IDE)
        }

        // Обработчик нажатия кнопки "Начать работу"
        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            // Создание и отображение главного окна
            var mainWindow = new MainWindow();
            mainWindow.Show();

            // Закрытие текущего окна входа
            this.Close();
        }
    }
}