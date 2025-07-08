using System.Windows.Forms;

namespace PinPoint
{
    public partial class EditDescriptionForm : Form
    {        
        // Храним переданный объект маркера для редактирования его описания
        private Marker marker;

        // Свойство для получения описания метки (RTF или текст)
        public string Description { get; private set; } = string.Empty;           

        // Конструктор формы, принимает маркер для редактирования
        public EditDescriptionForm(Marker marker)
        {
            InitializeComponent(); 
            this.marker = marker; // Сохраняем переданный объект маркера
            this.Text = $"Редактирование описания - {marker.Name}"; // Устанавливаем заголовок формы, включая имя маркера
            
            // Восстанавливаем размер окна
            this.Size = RestoreTempSize();

            try
            {
                // Пытаемся загрузить описание в формате RTF
                richTextBoxDescription.Rtf = marker.Description;
            }
            catch
            {
                // Если загрузка RTF не удалась, отображаем описание как обычный текст
                // Проверяем, что строка не пустая или не состоит только из пробелов, прежде чем использовать Text
                if (!string.IsNullOrWhiteSpace(marker.Description)) 
                {
                    richTextBoxDescription.Text = marker.Description;
                }
            }            
            
            // Настраиваем элементы управления (в частности ToolStrip для работы с текстом)
            SetupToolStrip();
            // Настраиваем элемент выбор цвета текста
            SetupColorComboBox();

            // Назначаем обработчики событий для кнопок "Сохранить" и "Отмена"
            btnSave.Click += BtnSave_Click;
            btnCancel.Click += BtnCancel_Click;

            // Подключаем обработчик событий KeyDown для RichTextBox
            richTextBoxDescription.PreviewKeyDown += RichTextBoxDescription_PreviewKeyDown;

            // Обновление интерфейса при смене выделения текста или перемещении курсора
            richTextBoxDescription.SelectionChanged += RichTextBox_SelectionChanged;
            // Сохраняем размеры окна при его закрытии
            this.FormClosing += (s, e) => SaveTempSize();

        }

        // Восстановление размеров окна редактирования
        private Size RestoreTempSize()
        {
            // Если первый запуск и значение размера отсутствует - задаем стандартный размер, иначе загружаем сохраненный
            return Form1.tempDescriptionWindowSize == Size.Empty ? new Size(900, 460) : Form1.tempDescriptionWindowSize;
        }
        // Сохранение размеров окна редактирования
        private void SaveTempSize()
        {
            Form1.tempDescriptionWindowSize = this.Size;
        }

        private void RichTextBoxDescription_PreviewKeyDown(object? sender, PreviewKeyDownEventArgs e)
        {
            // Проверяем, что была нажата клавиша Tab
            if (e.KeyCode == Keys.Tab)
            {
                // Помечаем Tab как ввод, чтобы предотвратить переключение фокуса
                e.IsInputKey = true;
            }
        }        

        private void SetupToolStrip()
        {            
            // Настраиваем выпадающий список шрифтов
            fontComboBox.Items.Clear(); // Очищаем список перед добавлением новых элементов
            foreach (var font in FontFamily.Families)
            {
                fontComboBox.Items.Add(font.Name); // Добавляем название каждого шрифта в список
            }

            // Настраиваем выпадающий список размеров шрифта
            sizeComboBox.Items.Clear();
            for (int i = 8; i <= 32; i += 2) // Добавляем значения размеров шрифта от 8 до 48 с шагом 2
            {
                sizeComboBox.Items.Add(i.ToString());
            }

            // Назначаем обработчики событий для изменения шрифта и размера текста
            fontComboBox.SelectedIndexChanged += (s, e) =>
            {
                if (fontComboBox.SelectedItem is string fontName)
                    ChangeFont(fontName, null);
            };
            sizeComboBox.SelectedIndexChanged += (s, e) =>
            {
                if (int.TryParse(sizeComboBox.SelectedItem?.ToString(), out int size))
                    ChangeFont(null, size);
            };

            // Подключаем обработчики событий
            boldButton.Click += (s, e) => ToggleStyle(FontStyle.Bold, boldButton);
            italicButton.Click += (s, e) => ToggleStyle(FontStyle.Italic, italicButton);
            underlineButton.Click += (s, e) => ToggleStyle(FontStyle.Underline, underlineButton);

            // Подписываемся на обработку ввода вручную
            sizeComboBox.KeyDown += SizeComboBox_KeyDown;

            // Устанавливаем значения по умолчанию
            SetDefaultFontSettings();
        }

        private void SetDefaultFontSettings()
        {
            if (richTextBoxDescription.SelectionFont != null)
            {
                // Устанавливаем текущий шрифт
                fontComboBox.SelectedItem = richTextBoxDescription.SelectionFont.FontFamily.Name;

                // Устанавливаем текущий размер
                sizeComboBox.SelectedItem = ((int)richTextBoxDescription.SelectionFont.Size).ToString();
                sizeComboBox.Text = ((int)richTextBoxDescription.SelectionFont.Size).ToString();
            }
            else
            {
                // Если нет выделенного текста, выбираем стандартные значения
                fontComboBox.SelectedItem = "Arial"; // Шрифт по умолчанию
                sizeComboBox.SelectedItem = "12";    // Размер по умолчанию
                sizeComboBox.Text = "12";
            }
        }

        private void SizeComboBox_SelectedIndexChanged(object? sender, EventArgs e)
        {
            // Если выбран новый размер шрифта, применяем его
            if (sizeComboBox.SelectedItem != null && int.TryParse(sizeComboBox.SelectedItem.ToString(), out int newSize))
            {
                ChangeFont(null, newSize); // Изменяем только размер шрифта, оставляя имя шрифта без изменений
            }
        }

        private void FontComboBox_SelectedIndexChanged(object? sender, EventArgs e)
        {
            // Если выбран новый шрифт, применяем его
            if (fontComboBox.SelectedItem != null)
            {
                ChangeFont(fontComboBox.SelectedItem.ToString(), null); // Изменяем только имя шрифта, оставляя размер без изменений
                richTextBoxDescription.Focus(); // Переводим фокус на richTextBoxDescription для продолжения ввода текста
            }
        }

        private void ChangeFont(string? fontName, float? fontSize)
        {
            int start = richTextBoxDescription.SelectionStart;  // Позиция начала выделения
            int length = richTextBoxDescription.SelectionLength;// Длина выделения

            if (length > 0)
            {
                for (int i = 0; i < length; i++)    //Проходим по каждому выделенному символу
                {
                    richTextBoxDescription.Select(start + i, 1);
                    var font = richTextBoxDescription.SelectionFont ?? richTextBoxDescription.Font;

                    string newFontName = fontName ?? font.FontFamily.Name;
                    float newSize = fontSize ?? font.Size;

                    richTextBoxDescription.SelectionFont = new Font(newFontName, newSize, font.Style);
                }

                richTextBoxDescription.Select(start, length); // Восстанавливаем выделение
            }
            else
            {   
                //Если ничего не выделено, устанавливаем шрифт для текущей позиции курсора
                var font = richTextBoxDescription.SelectionFont ?? richTextBoxDescription.Font;
                string newFontName = fontName ?? font.FontFamily.Name;
                float newSize = fontSize ?? font.Size;                 
                richTextBoxDescription.SelectionFont = new Font(newFontName, newSize, font.Style);
            }

            richTextBoxDescription.Focus();
        }

        private void SizeComboBox_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (int.TryParse(sizeComboBox.Text, out int newSize)) // Проверяем, число ли это
                {
                    newSize = Math.Min(newSize, 32); // Ограничиваем размер

                    ChangeFont(null, newSize); // Применяем размер

                    sizeComboBox.Text = newSize.ToString(); // Обновляем поле
                }
                else
                {
                    // Проверяем, есть ли выделенный текст со шрифтом
                    if (richTextBoxDescription.SelectionFont != null)
                    {
                        sizeComboBox.Text = ((int)richTextBoxDescription.SelectionFont.Size).ToString();
                    }
                    else
                    {
                        sizeComboBox.Text = "12"; // Значение по умолчанию
                    }
                }

                e.SuppressKeyPress = true; // Предотвращаем системный звук
                                           
                richTextBoxDescription.Focus(); // Переводим фокус на richTextBoxDescription для продолжения ввода текста
            }
        }

        private void SetupColorComboBox()
        {
            // Очищаем перед добавлением
            colorComboBox.Items.Clear();

            // Получаем внутренний ComboBox
            if (colorComboBox.Control is not ComboBox innerComboBox) return;

            // Настраиваем отображение элементов
            innerComboBox.DrawMode = DrawMode.OwnerDrawFixed;
            innerComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            innerComboBox.ItemHeight = 20;
            innerComboBox.DrawItem += ColorComboBox_DrawItem;

            // Заполняем цветами
            Color[] colors = { Color.Black, Color.DarkRed, Color.Red, Color.Orange, Color.Yellow,
                       Color.Lime, Color.Green, Color.Cyan, Color.Blue, Color.Purple };

            foreach (Color color in colors)
            {
                colorComboBox.Items.Add(color);
            }

            // Устанавливаем цвет по умолчанию
            colorComboBox.SelectedItem = colors[0];  // Чтобы был не null
            colorComboBox.SelectedIndexChanged += (s, e) =>
            {
                if (colorComboBox.SelectedItem is Color selectedColor)
                    ChangeColor(selectedColor);
            };
        }

        // Рисуем цветные квадраты в ToolStripComboBox
        private void ColorComboBox_DrawItem(object? sender, DrawItemEventArgs e)
        {
            if (e.Index < 0 || sender is not ComboBox comboBox) return;

            if (comboBox.Items[e.Index] is not Color color) return;

            e.DrawBackground();
            using (Brush brush = new SolidBrush(color))
            {
                e.Graphics.FillRectangle(brush, e.Bounds.Left + 2, e.Bounds.Top + 2, 16, e.Bounds.Height - 4);
            }

            e.Graphics.DrawRectangle(Pens.Black, e.Bounds.Left + 2, e.Bounds.Top + 2, 16, e.Bounds.Height - 4);
            e.DrawFocusRectangle();
        }

        // Применяем цвет к выделенному тексту
        private void ChangeColor(Color color)
        {
            int start = richTextBoxDescription.SelectionStart;
            int length = richTextBoxDescription.SelectionLength;

            if (length > 0)
            {
                for (int i = 0; i < length; i++)
                {
                    richTextBoxDescription.Select(start + i, 1);
                    richTextBoxDescription.SelectionColor = color;
                }

                richTextBoxDescription.Select(start, length);
            }
            else
            {
                richTextBoxDescription.SelectionColor = color;
            }

            richTextBoxDescription.Focus();
        }

        private void ToggleStyle(FontStyle style, ToolStripButton button)
        {
            int selectionStart = richTextBoxDescription.SelectionStart;
            int selectionLength = richTextBoxDescription.SelectionLength;

            if (selectionLength == 0)
            {
                // Если ничего не выделено — меняем стиль для будущего ввода
                Font currentFont = richTextBoxDescription.SelectionFont ?? richTextBoxDescription.Font;
                FontStyle newStyle = currentFont.Style.HasFlag(style) ? currentFont.Style & ~style : currentFont.Style | style;
                richTextBoxDescription.SelectionFont = new Font(currentFont, newStyle);
                button.Checked = newStyle.HasFlag(style);
            }
            else
            {
                // Проверяем, весь ли текст выделения уже имеет стиль
                bool allHasStyle = true;

                richTextBoxDescription.SelectionChanged -= RichTextBox_SelectionChanged; // временно отключим
                richTextBoxDescription.SuspendLayout();

                richTextBoxDescription.SelectionStart = selectionStart;
                richTextBoxDescription.SelectionLength = selectionLength;

                for (int i = 0; i < selectionLength; i++)
                {
                    richTextBoxDescription.Select(selectionStart + i, 1);
                    Font f = richTextBoxDescription.SelectionFont ?? richTextBoxDescription.Font;
                    if (!f.Style.HasFlag(style))
                    {
                        allHasStyle = false;
                        break;
                    }
                }

                // Применяем стиль ко всей выделенной области
                richTextBoxDescription.Select(selectionStart, selectionLength);
                for (int i = 0; i < selectionLength; i++)
                {
                    richTextBoxDescription.Select(selectionStart + i, 1);
                    Font f = richTextBoxDescription.SelectionFont ?? richTextBoxDescription.Font;
                    FontStyle newStyle = allHasStyle ? f.Style & ~style : f.Style | style;
                    richTextBoxDescription.SelectionFont = new Font(f, newStyle);
                }

                richTextBoxDescription.Select(selectionStart, selectionLength); // восстановим выделение
                richTextBoxDescription.ResumeLayout();
                richTextBoxDescription.SelectionChanged += RichTextBox_SelectionChanged;

                // Обновим состояние кнопки
                button.Checked = !allHasStyle;
            }

            richTextBoxDescription.Focus();
        }

        // Автоматическое обновление состояния кнопок в зависимости от положения курсора или выделения
        private void RichTextBox_SelectionChanged(object? sender, EventArgs e)
        {
            // Получаем текущий шрифт, если он есть
            Font? currentFont = richTextBoxDescription.SelectionFont;
            if (currentFont == null)
                return;

            // Обновляем состояние кнопок
            boldButton.Checked = currentFont.Bold;
            italicButton.Checked = currentFont.Italic;
            underlineButton.Checked = currentFont.Underline;

            // Обновляем шрифт в ComboBox
            fontComboBox.SelectedItem = currentFont.FontFamily.Name;

            // Обновляем размер
            UpdateSizeComboBox(currentFont.Size);

            // Обновляем цвет
            UpdateColorComboBox(richTextBoxDescription.SelectionColor);
        }

        // Обновляем состояние colorComboBox в зависимости от текущего цвета
        private void UpdateColorComboBox(Color color)
        {
            if (colorComboBox.Items.Contains(color))
            {
                colorComboBox.SelectedItem = color;
            }
            else
            {
                // Если цвет нестандартный — убираем выделение
                colorComboBox.SelectedIndex = -1;
            }
        }

        // Обновляем и отображаем актуальный размер, даже если его нет в списке
        private void UpdateSizeComboBox(float size)
        {
            string sizeText = ((int)size).ToString();

            if (sizeComboBox.Items.Contains(sizeText))
            {
                // Если размер есть в списке — устанавливаем SelectedItem
                sizeComboBox.SelectedItem = sizeText;
            }
            else
            {
                // Иначе просто обновляем текст вручную
                sizeComboBox.SelectedIndex = -1;
                sizeComboBox.Text = sizeText;
            }
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            // Сохраняем описание в формате RTF            
            if (!string.IsNullOrWhiteSpace(richTextBoxDescription.Rtf)) // Сохраняем только если RTF-данные не пусты
            {
                marker.Description = richTextBoxDescription.Rtf;
            }
            DialogResult = DialogResult.OK; // Устанавливаем результат диалога как OK
            Close();
        }

        private void BtnCancel_Click(object? sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel; // Устанавливаем результат диалога как Cancel
            Close();
        }
    }
}
