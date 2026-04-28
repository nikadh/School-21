using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace DocxToHtmlConverter
{
    public partial class MainWindow : Window
    {
        // Словарь для преобразования стилей Word в HTML-теги
        private readonly Dictionary<string, string> _styleMapping = new Dictionary<string, string>
        {
            {"Heading1", "h1"}, {"Heading2", "h2"}, {"Heading3", "h3"},
            {"Title", "h1"}, {"Subtitle", "h2"}, {"ListParagraph", "li"},
            {"Quote", "blockquote"}
        };
        // Конструктор главного окна
        public MainWindow() 
        {
            InitializeComponent(); // Инициализация компонентов XAML (генерируется IDE)
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e) // Обработчик кнопки "Обзор"
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Word Documents|*.docx",
                Title = "Выберите DOCX файл"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                FilePathTextBox.Text = openFileDialog.FileName;  // Заполнение пути к файлу
            }
        }

        private async void ConvertButton_Click(object sender, RoutedEventArgs e)  // Обработчик кнопки "Конвертировать в HTML" (асинхронный)
        {
            string filePath = FilePathTextBox.Text;

            if (string.IsNullOrWhiteSpace(filePath))
            {
                MessageBox.Show("Пожалуйста, укажите путь к файлу", "Ошибка", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!File.Exists(filePath))
            {
                MessageBox.Show("Указанный файл не существует", "Ошибка", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                ConvertButton.IsEnabled = false;  // Блокировка кнопки
                ProgressBar.Visibility = Visibility.Visible;// Показ прогресс-бара

                string outputPath = Path.ChangeExtension(filePath, ".html"); 
                await ConvertDocxToHtmlAsync(filePath, outputPath); // Асинхронная конвертация


                var metadata = await ExtractMetadataAsync(filePath); // Извлечение метаданных
                DisplayMetadata(metadata); //отображение метаданных

                MessageBox.Show($"Файл успешно конвертирован:\n{outputPath}",
                    "Готово", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при конвертации: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                ConvertButton.IsEnabled = true; // Разблокировка кнопки
                ProgressBar.Visibility = Visibility.Collapsed; // Скрытие прогресс-бара
            }
        }
        // Обработчик кнопки "Выход"
        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new LoginWindow(); // Возврат к окну входа
            loginWindow.Show();
            this.Close();
        }
        // Обработчик кнопки "Готово"
        private void DoneButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();// Завершение приложения
        }
        // Асинхронная конвертация DOCX в HTML, позволяет запускать операции параллельно 
        private async Task ConvertDocxToHtmlAsync(string inputPath, string outputPath)
        {
            await Task.Run(() =>
            {
                try
                {
                    string htmlContent = GenerateHtmlFromDocx(inputPath);// Генерация HTML
                    File.WriteAllText(outputPath, htmlContent, Encoding.UTF8);// Сохранение файла
                }
                catch (Exception ex)
                {
                    throw new Exception("Ошибка при конвертации в HTML", ex);
                }
            });
        }
        // Генерация HTML из DOCX
        private string GenerateHtmlFromDocx(string filePath)
        {
            var html = new StringBuilder();
            var ns = XNamespace.Get("http://schemas.openxmlformats.org/wordprocessingml/2006/main");

            using (var archive = ZipFile.OpenRead(filePath))
            {
                var documentEntry = archive.GetEntry("word/document.xml");
                if (documentEntry == null)
                    throw new Exception("Не удалось найти document.xml в архиве");

                XDocument doc;
                using (var stream = documentEntry.Open()) 
                {
                    doc = XDocument.Load(stream); // Загрузка XML-документа
                }

                var styles = LoadStyles(archive); //ЗАгрузка стилей WORD

                // Формирование HTML-заголовка
                html.AppendLine("<!DOCTYPE html>");
                html.AppendLine("<html lang=\"ru\">");
                html.AppendLine("<head>");
                html.AppendLine("<meta charset=\"UTF-8\">");
                html.AppendLine("<meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
                html.AppendLine("<title>Конвертированный документ</title>");
                html.AppendLine("<style>");
                html.AppendLine(CreateEnhancedCss()); // Добавление CSS
                html.AppendLine("</style>");
                html.AppendLine("</head>");
                html.AppendLine("<body>");

                var body = doc.Root?.Element(ns + "body");
                if (body != null)
                {
                    bool isFirstParagraph = true;
                    foreach (var element in body.Elements()) 
                    {
                        if (!isFirstParagraph) html.AppendLine();
                        ProcessElement(element, html, ns, styles); // Обработка элементов документа
                        isFirstParagraph = false;
                    }
                }

                html.AppendLine("</body>");
                html.AppendLine("</html>");
            }

            return html.ToString();
        }
        // Создание CSS для HTML-документа
        private string CreateEnhancedCss()
        {//Стили  для документа
            return @"
                body { 
                    font-family: 'Segoe UI', Arial, sans-serif; 
                    line-height: 1.6; 
                    margin: 0;
                    padding: 2em;
                    color: #333;
                    background-color: #f9f9f9;
                    font-weight: normal;
                }
                h1 { 
                    font-size: 2.2em; 
                    color: #2c3e50;
                    border-bottom: 2px solid #3498db;
                    padding-bottom: 0.3em;
                    margin-top: 1.5em;
                    margin-bottom: 1em;
                }
                h2 { 
                    font-size: 1.8em; 
                    color: #2980b9;
                    margin-top: 1.3em;
                    margin-bottom: 0.8em;
                }
                h3 { 
                    font-size: 1.4em; 
                    color: #16a085;
                    margin-top: 1.1em;
                    margin-bottom: 0.6em;
                }
                p { 
                    margin: 1em 0;
                    text-align: justify;
                }
                li { 
                    margin: 0.5em 0;
                    padding-left: 0.5em;
                }
                table {
                    border-collapse: collapse;
                    width: 100%;
                    margin: 1.5em 0;
                    box-shadow: 0 1px 3px rgba(0,0,0,0.1);
                }
                th {
                    background-color: #3498db;
                    color: white;
                    text-align: left;
                    padding: 0.8em;
                }
                td {
                    border: 1px solid #ddd;
                    padding: 0.8em;
                    background-color: #fff;
                }
                tr:nth-child(even) td {
                    background-color: #f2f2f2;
                }
                blockquote {
                    border-left: 4px solid #3498db;
                    padding: 1em 1.5em;
                    margin: 1.5em 0;
                    background-color: #f8f9fa;
                    color: #555;
                }
                strong { 
                    font-weight: 700;
                    color: #2c3e50;
                }
                em { 
                    font-style: italic;
                    color: #e74c3c;
                }
                u { 
                    text-decoration: underline;
                    text-decoration-color: #3498db;
                }
                s { 
                    text-decoration: line-through;
                    color: #95a5a6;
                }
                @media (max-width: 768px) {
                    body { padding: 1em; }
                    h1 { font-size: 1.8em; }
                    table { display: block; overflow-x: auto; }
                }
            ";
        }
        // Загрузка стилей из Word
        private Dictionary<string, XElement> LoadStyles(ZipArchive archive)
        {
            var styles = new Dictionary<string, XElement>();
            var stylesEntry = archive.GetEntry("word/styles.xml");
            if (stylesEntry != null)
            {
                using (var stream = stylesEntry.Open())
                {
                    var doc = XDocument.Load(stream);
                    var ns = doc.Root.GetDefaultNamespace();
                    foreach (var style in doc.Descendants(ns + "style"))
                    {
                        var styleId = style.Attribute(ns + "styleId")?.Value;
                        if (styleId != null)
                        {
                            styles[styleId] = style;
                        }
                    }
                }
            }
            return styles;
        }

        private void ProcessElement(XElement element, StringBuilder html, XNamespace ns, Dictionary<string, XElement> styles)
        {
            switch (element.Name.LocalName) // Обработка элементов документа (параграфы, таблицы и т.д.)
            {
                case "p":
                    ProcessParagraph(element, html, ns, styles);
                    break;
                case "tbl":
                    ProcessTable(element, html, ns);
                    break;
                case "br":
                    html.AppendLine("<br/>");
                    break;
                default:
                    break;
            }
        }

        // Обработка параграфа и таблицы
        private void ProcessParagraph(XElement paragraph, StringBuilder html, XNamespace ns, Dictionary<string, XElement> styles)
        {
            var pPr = paragraph.Element(ns + "pPr");
            string styleId = pPr?.Element(ns + "pStyle")?.Attribute(ns + "val")?.Value;

            var runs = paragraph.Elements(ns + "r");
            if (!runs.Any()) return;

            string htmlTag = "p";
            string style = "";

            if (styleId != null && _styleMapping.ContainsKey(styleId))
            {
                htmlTag = _styleMapping[styleId];
            }

            var jc = pPr?.Element(ns + "jc");
            if (jc != null)
            {
                string align = jc.Attribute(ns + "val")?.Value;
                switch (align)
                {
                    case "left": style += "text-align: left;"; break;
                    case "center": style += "text-align: center;"; break;
                    case "right": style += "text-align: right;"; break;
                    case "both": style += "text-align: justify;"; break;
                }
            }

            var ind = pPr?.Element(ns + "ind");
            if (ind != null)
            {
                var left = ind.Attribute(ns + "left")?.Value;
                var right = ind.Attribute(ns + "right")?.Value;
                var firstLine = ind.Attribute(ns + "firstLine")?.Value;
                var hanging = ind.Attribute(ns + "hanging")?.Value;

                if (left != null) style += $"margin-left: {ConvertTwipsToPx(left)}px;";
                if (right != null) style += $"margin-right: {ConvertTwipsToPx(right)}px;";

                if (firstLine != null)
                {
                    style += $"text-indent: {ConvertTwipsToPx(firstLine)}px;";
                }
                else if (hanging != null)
                {
                    style += $"text-indent: -{ConvertTwipsToPx(hanging)}px; padding-left: {ConvertTwipsToPx(hanging)}px;";
                }
            }

            if (!string.IsNullOrEmpty(style))
            {
                html.Append($"<{htmlTag} style=\"{style}\">");
            }
            else
            {
                html.Append($"<{htmlTag}>");
            }

            foreach (var run in runs)
            {
                ProcessRun(run, html, ns);
            }

            html.Append($"</{htmlTag}>");
        }

        private void ProcessRun(XElement run, StringBuilder html, XNamespace ns)
        {
            if (run.Element(ns + "br") != null)
            {
                html.Append("<br/>");
                return;
            }

            var textElement = run.Element(ns + "t");
            if (textElement == null) return;

            string text = textElement.Value;
            if (string.IsNullOrEmpty(text)) return;

            var rPr = run.Element(ns + "rPr");

            bool isBold = rPr?.Element(ns + "b") != null;
            bool isItalic = rPr?.Element(ns + "i") != null;
            bool isUnderline = rPr?.Element(ns + "u") != null;
            bool isStrike = rPr?.Element(ns + "strike") != null;

            if (isBold) html.Append("<strong>");
            if (isItalic) html.Append("<em>");
            if (isUnderline) html.Append("<u>");
            if (isStrike) html.Append("<s>");

            html.Append(WebUtility.HtmlEncode(text));

            if (isStrike) html.Append("</s>");
            if (isUnderline) html.Append("</u>");
            if (isItalic) html.Append("</em>");
            if (isBold) html.Append("</strong>");
        }

        private int ConvertTwipsToPx(string twipsValue)
        {
            if (int.TryParse(twipsValue, out int twips))
            {
                return (int)Math.Round(twips / 20.0 * 96.0 / 72.0);
            }
            return 0;
        }
        // обработка таблицы
        private void ProcessTable(XElement table, StringBuilder html, XNamespace ns)
        {
            html.AppendLine("<table>");

            foreach (var row in table.Elements(ns + "tr"))
            {
                html.Append("<tr>");

                foreach (var cell in row.Elements(ns + "tc"))
                {
                    var tcPr = cell.Element(ns + "tcPr");
                    string cellStyle = "";

                    var tcW = tcPr?.Element(ns + "tcW");
                    if (tcW != null)
                    {
                        string width = tcW.Attribute(ns + "w")?.Value;
                        if (width != null)
                        {
                            cellStyle += $"width: {ConvertTwipsToPx(width)}px;";
                        }
                    }

                    var vAlign = tcPr?.Element(ns + "vAlign");
                    if (vAlign != null)
                    {
                        string align = vAlign.Attribute(ns + "val")?.Value;
                        if (align == "center")
                        {
                            cellStyle += "vertical-align: middle;";
                        }
                    }

                    html.Append($"<td style=\"{cellStyle}\">");

                    foreach (var paragraph in cell.Elements(ns + "p"))
                    {
                        foreach (var run in paragraph.Elements(ns + "r"))
                        {
                            ProcessRun(run, html, ns);
                        }
                        html.Append("<br/>");
                    }

                    html.Append("</td>");
                }

                html.AppendLine("</tr>");
            }

            html.AppendLine("</table>");
        }
        // Извлечение метаданных документа
        private async Task<DocumentMetadata> ExtractMetadataAsync(string filePath)
        {
            return await Task.Run(() =>
            {
                var metadata = new DocumentMetadata();

                try
                {
                    using (var archive = ZipFile.OpenRead(filePath))
                    {
                        var coreEntry = archive.GetEntry("docProps/core.xml");
                        if (coreEntry != null)
                        {
                            using (var stream = coreEntry.Open())
                            {
                                var doc = XDocument.Load(stream);
                                var ns = doc.Root.GetDefaultNamespace();
                                var dc = XNamespace.Get("http://purl.org/dc/elements/1.1/");
                                var dcterms = XNamespace.Get("http://purl.org/dc/terms/");

                                metadata.Title = doc.Root.Element(dc + "title")?.Value;
                                metadata.Author = doc.Root.Element(dc + "creator")?.Value;
                                metadata.CreationDate = ParseDate(doc.Root.Element(dcterms + "created")?.Value);
                                metadata.ModificationDate = ParseDate(doc.Root.Element(dcterms + "modified")?.Value);
                            }
                        }

                        var appEntry = archive.GetEntry("docProps/app.xml");
                        if (appEntry != null)
                        {
                            using (var stream = appEntry.Open())
                            {
                                var doc = XDocument.Load(stream);
                                var ns = doc.Root.GetDefaultNamespace();

                                if (int.TryParse(doc.Root.Element(ns + "Words")?.Value, out int words))
                                    metadata.WordCount = words;
                                if (int.TryParse(doc.Root.Element(ns + "Pages")?.Value, out int pages))
                                    metadata.PageCount = pages;
                                if (int.TryParse(doc.Root.Element(ns + "TotalTime")?.Value, out int minutes))
                                    metadata.TotalEditingTime = FormatEditingTime(minutes);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Ошибка при извлечении метаданных", ex);
                }

                return metadata;
            });
        }

        private string FormatEditingTime(int totalMinutes)
        {
            if (totalMinutes < 60) return $"{totalMinutes} минут";
            int hours = totalMinutes / 60;
            int minutes = totalMinutes % 60;
            if (hours < 24) return $"{hours} час. {minutes} мин.";
            int days = hours / 24;
            hours = hours % 24;
            return $"{days} дн. {hours} час. {minutes} мин.";
        }

        private DateTime? ParseDate(string dateString)
        {
            return DateTime.TryParse(dateString, out DateTime result) ? result : (DateTime?)null;
        }
        //отображение метаданных
        private void DisplayMetadata(DocumentMetadata metadata)
{
    try
    {
        if (MetadataTextBox == null || metadata == null)
            return;

        var sb = new StringBuilder();
        sb.AppendLine($"Название: {metadata.Title ?? "Не указано"}");
        sb.AppendLine($"Автор: {metadata.Author ?? "Неизвестен"}");
        sb.AppendLine($"Дата создания: {metadata.CreationDate?.ToString("dd.MM.yyyy HH:mm") ?? "Неизвестна"}");
        sb.AppendLine($"Дата изменения: {metadata.ModificationDate?.ToString("dd.MM.yyyy HH:mm") ?? "Неизвестна"}");
        sb.AppendLine($"Время редактирования: {metadata.TotalEditingTime ?? "Неизвестно"}");
        sb.AppendLine($"Количество слов: {metadata.WordCount?.ToString() ?? "Неизвестно"}");
        sb.AppendLine($"Количество страниц: {metadata.PageCount?.ToString() ?? "Неизвестно"}"); // Класс для хранения метаданных документа

                // Безопасное обновление UI
                Dispatcher.Invoke(() => 
        {
            MetadataTextBox.Text = sb.ToString();
            MetadataTextBox.ScrollToHome(); // Автоматическая прокрутка в начало
        });
    }
    catch (Exception ex)
    {
        Dispatcher.Invoke(() => 
        {
            MetadataTextBox.Text = $"Ошибка при загрузке метаданных: {ex.Message}";
        });
    }
}
    }

    public class DocumentMetadata
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public DateTime? CreationDate { get; set; }
        public DateTime? ModificationDate { get; set; }
        public string TotalEditingTime { get; set; }
        public int? WordCount { get; set; }
        public int? PageCount { get; set; }
    }
}