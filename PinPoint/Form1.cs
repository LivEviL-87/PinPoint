using System.Drawing.Drawing2D;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Windows.Forms;

namespace PinPoint
{
    public partial class Form1 : Form
    {
        private bool isMouseOverRichTextBox = false; // Флаг, отслеживающий положение мыши над RichTextBox
        // Запоминаем размер окна на время выполнения приложения
        public static Size tempDescriptionWindowSize = Size.Empty;
        // Флаг для отслеживания, происходит ли сейчас перетаскивание карты
        private bool isDragging = false;

        // Флаг для отслеживания факта перемещения карты, было или нет
        internal bool isDraggingWas = false;

        // Флаг для отслеживания режима перемещения маркера
        private bool isMovingMarker = false;

        // Последняя зафиксированная позиция мыши (для расчёта смещения при перетаскивании карты)
        private Point lastMousePosition;

        // Переменная для отслеживания последнего состояния окна (обычное, развернутое)
        private FormWindowState lastWindowState = FormWindowState.Normal;

        // Список, в котором хранятся все метки на карте
        private List<Marker> markers = new List<Marker>();
        
        // Флаг для отслеживания режима отображения карты
        private bool isAutoFitMode = false; // false - стандартный, true - адаптивный

        private bool isSorted = false; // Флаг для отслеживания состояния сортировки
        private bool isListModified = false; // Флаг для отслеживания изменений в списке

        // Контекстное меню маркера
        private ContextMenuStrip markerContextMenu;

        // Таймер для плавного перемещения к позиции маркера
        private System.Windows.Forms.Timer moveTimer = new System.Windows.Forms.Timer();
        private Point targetPosition;
        private const int animationSpeed = 10;

        public Form1()
        {
            InitializeComponent();

            // Подписываемся на события изменения размеров элементов, чтобы корректировать интерфейс
            listView1.SizeChanged += OnSizeChangedOrSplitterMoved;
            splitter1.SplitterMoved += OnSizeChangedOrSplitterMoved;
            mapPanel.SizeChanged += MapPanel_SizeChanged;
            listView1.ColumnWidthChanging += ListView1_ColumnWidthChanging; // Запрещаем изменение вручную

            // Подписываемся на событие изменения выбора в списке меток
            listView1.SelectedIndexChanged += ListView1_SelectedIndexChanged;

            //Подписываемся на двойной клик по списку
            listView1.DoubleClick += ListView1_DoubleClick;

            //Подписываемся на событие клика по шапке списка
            listView1.ColumnClick += ListView1_ColumnClick;

            // Устанавливаем начальные параметры отображения списка
            ResizeLastColumn();

            // Загружаем карту
            LoadMap("Map.png");                      

            // Подписываемся на события для обработки перетаскивания карты мышью
            mapPictureBox.MouseDown += MapPictureBox_MouseDown;
            mapPictureBox.MouseMove += MapPictureBox_MouseMove;
            mapPictureBox.MouseUp += MapPictureBox_MouseUp;

            // Подписываемся на события для обработки действий с RichTextBox1
            richTextBox1.MouseWheel += RichTextBox1_MouseWheel;
            richTextBox1.MouseEnter += (s, e) => isMouseOverRichTextBox = true;
            richTextBox1.MouseLeave += (s, e) => isMouseOverRichTextBox = false;

            mapPictureBox.MouseWheel += PictureBox_MouseWheel;            

            // Подписываемся на событие двойного клика для добавления новой метки
            mapPictureBox.MouseDoubleClick += MapPictureBox_MouseDoubleClick;

            // Подписываемся на событие изменения размера формы
            this.Resize += Form_Resize;

            //При закрытии окна приложения через крестик
            this.FormClosing += Form1_FormClosing;

            // Подключаем контекстное меню и обработчик переименования, удаления и тп. для списка
            listView1.ContextMenuStrip = contextMenuStrip1;
            renameToolStripMenuItem.Click += RenameToolStripMenuItem_Click;
            deleteToolStripMenuItem.Click += DeleteToolStripMenuItem_Click;
            editDescriptionToolStripMenuItem.Click += EditDescriptionToolStripMenuItem_Click;

            // Обработчик пунктов меню
            SaveToolStripMenuItem.Click += SaveToolStripMenuItem_Click;
            LoadToolStripMenuItem.Click += LoadToolStripMenuItem_Click;
            NewToolStripMenuItem.Click += NewToolStripMenuItem_Click;
            ExitToolStripMenuItem.Click += ExitToolStripMenuItem_Click;

            // Создаем контекстное меню "Переместить" для маркера
            markerContextMenu = new ContextMenuStrip();
            var moveMenuItem = new ToolStripMenuItem("Переместить");
            moveMenuItem.Click += MoveMarkerMenuItem_Click;
            // Проверяем перед открытием меню
            markerContextMenu.Opening += (s, e) =>
            {
                // Если мы в адаптивном режиме, отменяем отображение меню
                if (isAutoFitMode) {e.Cancel = true;}
            };

            markerContextMenu.Items.Add(moveMenuItem);

            PositionFitButton();
            // Подписываемся на изменение размера формы, чтобы кнопка оставалась на своем месте
            this.Resize += (sender, e) => PositionFitButton();

            // Подписываемся на изменение состояния прокрутки
            // mapPanel.Layout += (sender, e) => PositionFitButton();

            btnToggleFit.Click += (s, e) => ToggleFitMode();    // Клик по кнопке вызывает метод переключения режимов
            btnToggleFit.Click += (s, e) => listView1.Focus();  // Возвращаем фокус списку            

            mapPictureBox.SizeChanged += MapPictureBox_SizeChanged;

            moveTimer.Interval = 20; // Чем меньше значение, тем плавнее анимация
            moveTimer.Tick += MoveTimer_Tick;

            this.MouseDown += Form1_MouseDown;
            mapPanel.MouseDown += Form1_MouseDown;
            listView1.MouseDown += Form1_MouseDown;
            richTextBox1.MouseDown += Form1_MouseDown;
            richTextBox1.MouseDown += RestoreFocus;
            btnToggleFit.MouseDown += Form1_MouseDown;
            menuStrip1.MouseDown += Form1_MouseDown;

            // Подключаем обработчик события для пункта меню "Файл"
            fileToolStripMenuItem.Click += (sender, e) => {Form1_MouseDown(sender, FakeMouseEventArgs());};

            this.KeyPreview = true; // даём форме ловить нажатия клавиш до контролов
            this.KeyDown += Form1_KeyDown;

            ToggleFitMode();
            
        }

        private void PictureBox_MouseWheel(object? sender, MouseEventArgs e)
        {
            ((HandledMouseEventArgs)e).Handled = true; // Блокируем прокрутку изображения                                                       
        }

        private void RichTextBox1_MouseWheel(object? sender, MouseEventArgs e)
        {
            if (!isMouseOverRichTextBox) return;

            // Отключаем лишние обновления UI
            richTextBox1.SuspendLayout();

            // Вычисляем новую строку для прокрутки
            int currentLine = richTextBox1.GetLineFromCharIndex(richTextBox1.SelectionStart);
            int newLine = e.Delta > 0 ? Math.Max(0, currentLine - 3) : Math.Min(richTextBox1.Lines.Length - 1, currentLine + 3);
                        
            if (newLine < 0) newLine = 0;

            // Устанавливаем курсор на начало новой строки
            richTextBox1.SelectionStart = richTextBox1.GetFirstCharIndexFromLine(newLine);

            // Возвращаем нормальную отрисовку UI
            richTextBox1.ResumeLayout();

        }

        private void RestoreFocus(object? sender, MouseEventArgs e) //возвращаем фокус списку
        {
            listView1.Focus();
        }

        private void UpdateWindowTitle(string currentFileName)
        {
            string appName = "PinPoint (beta)";
            if (!string.IsNullOrEmpty(currentFileName))
                this.Text = $"{appName}  //  {Path.GetFileName(currentFileName)}";
            else
                this.Text = appName;
        }

        private void Form1_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                // Сброс режима перемещения
                CancelMarkerMove();
            }
        }

        // Сброс режима перемещения
        private void CancelMarkerMove()
        {
            if (isMovingMarker)
            {
                isMovingMarker = false;
                mapPictureBox.Cursor = Cursors.Default;
            }
        }

        private MouseEventArgs FakeMouseEventArgs()
        {
            // Создаем и возвращаем объект MouseEventArgs с фиктивными данными
            return new MouseEventArgs(
                MouseButtons.Left, // Левая кнопка мыши
                1,                 // Один клик
                0,                 // Координата X
                0,                 // Координата Y
                0                  // Колесо прокрутки
            );
        }

        private void Form1_MouseDown(object? sender, MouseEventArgs e)
        {                        
            // Сброс режима перемещения
            CancelMarkerMove();
        }

        // Если выбран пункт "переместить" в контексте маркера
        private void MoveMarkerMenuItem_Click(object? sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {                
               isMovingMarker = true;
               mapPictureBox.Cursor = Cursors.Cross; // Показываем, что ждём клика по карте                
            }
        }


        // При двойном клике по списку
        private void ListView1_DoubleClick(object? sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0) return; // Проверяем, есть ли выделенный элемент

            ListViewItem selectedItem = listView1.SelectedItems[0];
            if (selectedItem.Tag is Marker marker) // Проверяем, что в Tag хранится объект метки
            {
                CenterMapOnMarker(marker.Position);
            }
        }

        // Красивое центрирование с анимацией на маркере
        internal void CenterMapOnMarker(Point markerPosition)
        {
            // Определяем желаемую позицию, учитывая размеры карты и панели
            int centerX = markerPosition.X - mapPanel.Width / 2;
            int centerY = markerPosition.Y - mapPanel.Height / 2;

            // Корректируем координаты, чтобы не выйти за пределы карты
            centerX = Math.Max(0, Math.Min(centerX, mapPictureBox.Width - mapPanel.Width));
            centerY = Math.Max(0, Math.Min(centerY, mapPictureBox.Height - mapPanel.Height));

            // Устанавливаем целевую позицию и запускаем анимацию
            targetPosition = new Point(centerX, centerY);
            moveTimer.Start();
        }

        private void MoveTimer_Tick(object? sender, EventArgs e) // Перемещение к выбранному маркеру
        {
            // Текущая позиция прокрутки (с учётом знака)
            int currentX = -mapPanel.AutoScrollPosition.X;
            int currentY = -mapPanel.AutoScrollPosition.Y;

            // Вычисляем расстояние до цели
            int deltaX = targetPosition.X - currentX;
            int deltaY = targetPosition.Y - currentY;

            // Если расстояние небольшое — завершаем анимацию
            if (Math.Abs(deltaX) < 2 && Math.Abs(deltaY) < 2)
            {
                mapPanel.AutoScrollPosition = targetPosition; // Финальная точка
                moveTimer.Stop();
                return;
            }

            // Чем ближе к цели, тем меньше шаг перемещения
            int speedX = Math.Max(1, Math.Abs(deltaX) / animationSpeed);
            int speedY = Math.Max(1, Math.Abs(deltaY) / animationSpeed);

            // Двигаемся в сторону цели
            int newX = currentX + Math.Sign(deltaX) * speedX;
            int newY = currentY + Math.Sign(deltaY) * speedY;

            mapPanel.AutoScrollPosition = new Point(newX, newY);
        }

        private void Form1_FormClosing(object? sender, FormClosingEventArgs e)
        {
            if (listView1.SelectedItems.Count != 0) // Если список не пустой
            {
                // Проверяем, были ли изменения в списке
                if (isListModified)
                {
                    // Отображаем диалоговое окно для подтверждения
                    DialogResult result = MessageBox.Show("У вас остались несохраненные изменения.\nВы действительно хотите выйти из приложения?",
                                             "Подтверждение выхода",
                                             MessageBoxButtons.YesNo,
                                             MessageBoxIcon.Question);

                    // Если пользователь нажал "Нет"
                    if (result == DialogResult.No)
                    {
                        e.Cancel = true; // Отменяем закрытие формы
                    }
                }
            }

            Form1_MouseDown(sender, FakeMouseEventArgs());
        }

        private void ListView1_ColumnClick(object? sender, ColumnClickEventArgs e)
        {            
            // Проверяем, нуждается ли список в сортировке
            if (!isSorted)  // Если да, то сортируем по возрастанию
            {
                ToggleSorting(SortOrder.Ascending);
                isSorted = true;
            }
            else            // Если нет, сортируем по убыванию (переворачиваем)
            {
                ToggleSorting(SortOrder.Descending);
                isSorted = false;
            }
            
            Form1_MouseDown(sender, FakeMouseEventArgs());
        }

        private void ToggleSorting(SortOrder sortOrder)
        {
            listView1.Sorting = sortOrder;
            listView1.Sort();
            listView1.Sorting = SortOrder.None;
        }

        private void ExitToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            
            Close(); // Закрываем форму
            
        }

        private void NewToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            // Если есть несохранённые изменения, спрашиваем у пользователя
            if (isListModified)
            {
                DialogResult result = MessageBox.Show(
                    "Создать новый список?\nВсе несохраненные изменения будут потеряны.",
                    "Предупреждение",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                // Если пользователь нажал "Нет", прерываем создание нового списка
                if (result == DialogResult.No)
                {
                    return;
                }
            }

            // Очищаем данные
            markers.Clear();                // Очищаем список маркеров
            listView1.Items.Clear();        // Очищаем список в ListView
            mapPictureBox.Controls.Clear(); // Удаляем маркеры с карты
            richTextBox1.Clear();           // Очищаем текстовое поле описания
            isListModified = false;         // Сбрасываем флаг изменений, так как создан новый список
            UpdateWindowTitle("");
        }

        private void LoadToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            // Если были изменения, спрашиваем у пользователя
            if (isListModified)
            {
                DialogResult result = MessageBox.Show(
                    "У вас есть несохранённые изменения.\nПродолжить без сохранения?",
                    "Предупреждение",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                // Если пользователь нажал "Нет", прерываем операцию загрузки
                if (result == DialogResult.No)
                {
                    return;
                }
            }

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Список объектов (*.jdat)|*.jdat|Все файлы (*.*)|*.*";
                openFileDialog.Title = "Загрузить список";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    LoadMarkers(openFileDialog.FileName); // Вызываем метод загрузки
                    isListModified = false; // Сбрасываем флаг изменений после успешной загрузки
                }
            }
        }

        private void SaveToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog()) 
            {
                saveFileDialog.Filter = "Список объектов (*.jdat)|*.jdat|Все файлы (*.*)|*.*";
                saveFileDialog.Title = "Сохранить список";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    SaveMarkers(saveFileDialog.FileName); // Вызываем метод сохранения
                    isListModified = false; // Сбрасываем флаг изменений после успешного сохранения
                }
            }                
        }

        private void SaveMarkers (string filePath)
        {
            string json = JsonSerializer.Serialize(markers, new JsonSerializerOptions { 
                WriteIndented = true, 
                IgnoreReadOnlyProperties = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping  // Выводим символы без экранирования, с кириллицей
            });
            File.WriteAllText(filePath, json);            
            UpdateWindowTitle(filePath);
        }

        private void LoadMarkers(string filePath)
        {
            try
            {
                string json = File.ReadAllText(filePath); // Читаем JSON из файла
                markers = JsonSerializer.Deserialize<List<Marker>>(json) ?? new List<Marker>();

                // Очищаем ListView и обновляем список
                listView1.Items.Clear();
                mapPictureBox.Controls.Clear(); // Убираем старые маркеры                

                foreach (var marker in markers)
                {                    
                    AddMarkerToListView(marker);  // Добавляем в список
                    CreateMarkerPanel(marker);    // Создаём маркер на карте
                }
                richTextBox1.Clear(); // Очищаем описание маркера

                if (isAutoFitMode)
                {
                    UpdateMarkerPositions();
                }
                                
                UpdateWindowTitle(filePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки файла: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Метод подгоняет ширину последнего столбца ListView так, чтобы он занимал всю оставшуюся ширину
        private void ResizeLastColumn()
        {
            if (listView1.Columns.Count == 0) return;

            int totalWidth = listView1.ClientSize.Width; // Общая ширина списка
            int usedWidth = 0;

            // Считаем ширину всех столбцов, кроме последнего
            for (int i = 0; i < listView1.Columns.Count - 1; i++)
            {
                usedWidth += listView1.Columns[i].Width;
            }

            listView1.Columns[listView1.Columns.Count - 1].Width = Math.Max(totalWidth - usedWidth, 50);
        }

        private void OnSizeChangedOrSplitterMoved(object? sender, EventArgs e)
        {
            ResizeLastColumn(); // Вызываем пересчёт ширины колонок списка при изменении размеров
            PositionMap(); // Центрируем карту
        }

        // Запрещаем изменение ширины последнего столбца вручную
        private void ListView1_ColumnWidthChanging(object? sender, ColumnWidthChangingEventArgs e)
        {
            if (e.ColumnIndex == listView1.Columns.Count - 1) // Если это последний столбец
            {
                e.Cancel = true; // Отменяем изменение ширины
                e.NewWidth = listView1.Columns[e.ColumnIndex].Width; // Возвращаем старую ширину
            }
        }

        // Загружаем изображение карты
        private void LoadMap(string path)
        {
            try
            {
                // Проверяем, существует ли файл карты
                if (File.Exists(path))
                {
                    // Если файл существует, загружаем карту
                    mapPictureBox.Image = Image.FromFile(path);
                    // Центрируем карту
                    PositionMap();
                    // Выходим из метода, так как всё прошло успешно
                    return;
                }

                // Если файл не найден, показываем сообщение об ошибке
                MessageBox.Show("Файл Map.png не найден.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                // Обрабатываем любые ошибки при загрузке карты
                MessageBox.Show($"Ошибка при загрузке карты: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // Если что-то пошло не так, закрываем приложение
            mapPictureBox.Image?.Dispose(); // На случай, если карта частично загрузилась с ошибкой, чистим за собой
            Environment.Exit(1);
        }

        // Метод центрирует карту в панели, если карта меньше панели
        private void PositionMap()
        {
            if (mapPictureBox.Image == null) return;

            // Сбрасываем автоматическую прокрутку (иначе сохраняется старый отступ)
            mapPanel.AutoScroll = false;
            mapPanel.AutoScrollPosition = new Point(0, 0);

            // Получаем размеры карты
            int mapWidth = mapPictureBox.Image.Width;
            int mapHeight = mapPictureBox.Image.Height;
            // Получаем размеры панели
            int panelWidth = mapPanel.ClientSize.Width;
            int panelHeight = mapPanel.ClientSize.Height;
                        
            // Вычисляем новые координаты для центрирования
            int startX = Math.Max((panelWidth - mapWidth) / 2, 0);
            int startY = Math.Max((panelHeight - mapHeight) / 2, 0);
            
            // Устанавливаем новое положение карты
            mapPictureBox.Location = new Point(startX, startY);

            // Включаем автопрокрутку, если карта больше панели
            mapPanel.AutoScroll = mapWidth > panelWidth || mapHeight > panelHeight;

        }

        // Обнуляем позицию карты
        private void ResetMapPosition()
        {
            if (mapPictureBox.Image == null) return;
            mapPictureBox.Location = new Point(0, 0);
        }

        // Добавляем вызов пересчёта координат при изменении размеров карты
        private void MapPanel_SizeChanged(object? sender, EventArgs e)
        {
            PositionMap();
            UpdateMarkerPositions();
        }

        // Начинаем перетаскивание карты или перемещение маркера (если включен режим перемещения)
        // Обработка нажатия кнопки мыши на карте
        private void MapPictureBox_MouseDown(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // Если включён режим перемещения маркера и в списке выбран хотя бы один элемент
                if (isMovingMarker && listView1.SelectedItems.Count > 0)
                {
                    // Получаем первый выбранный элемент списка
                    var selectedItem = listView1.SelectedItems[0];

                    // Проверяем, что Tag не равен null и содержит объект типа Marker
                    if (selectedItem.Tag is Marker marker)
                    {
                        // Обновляем позицию маркера данными события мыши
                        marker.Position = e.Location;

                        // Ищем соответствующий элемент (RoundMarker), чтобы обновить его положение
                        foreach (Control ctrl in mapPictureBox.Controls)
                        {
                            // Проверяем, что это именно RoundMarker и что его Id совпадает с Id маркера
                            if (ctrl is RoundMarker rm && rm.MarkerData.Id == marker.Id)
                            {
                                // Смещаем визуальный маркер так, чтобы он центрировался по координатам
                                rm.Location = new Point(
                                    marker.Position.X - RoundMarker.MarkerSize / 2,
                                    marker.Position.Y - RoundMarker.MarkerSize / 2);
                                isListModified = true;
                                break; // Нашли нужный маркер — выходим из цикла
                            }
                        }

                        // Завершаем режим перемещения
                        isMovingMarker = false;
                        mapPictureBox.Cursor = Cursors.Default;
                        isDraggingWas = true; // Предотвращает сброс выделения при MouseUp
                    }
                }
                else
                {
                    // Если режим перемещения не активен — запускаем обычное перетаскивание карты
                    isDragging = true;
                    isDraggingWas = false;
                    moveTimer.Stop(); // Останавливаем плавное движение карты
                    lastMousePosition = mapPanel.PointToClient(Cursor.Position); // Сохраняем позицию мыши
                }
            }
        }

        // Перемещение карты при удержании мыши
        private void MapPictureBox_MouseMove(object? sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                isDraggingWas = true; // Отмечаем что было перетаскивание карты

                Point currentMousePosition = mapPanel.PointToClient(Cursor.Position);
                int deltaX = currentMousePosition.X - lastMousePosition.X;
                int deltaY = currentMousePosition.Y - lastMousePosition.Y;

                mapPanel.AutoScrollPosition = new Point(
                    -mapPanel.AutoScrollPosition.X - deltaX,
                    -mapPanel.AutoScrollPosition.Y - deltaY
                );

                lastMousePosition = currentMousePosition;
            }
        }

        private void MapPictureBox_MouseUp(object? sender, MouseEventArgs e)
        {
            
            isDragging = false; 
            if (!isDraggingWas) // Если перетаскивания небыло 
            {
                DeselectMarker(e);
            }            
            
        }

        private void DeselectMarker(MouseEventArgs e) 
        {
            // Если был левый клик и был выбран какой-либо элемент            
            if (e.Button == MouseButtons.Left && listView1.SelectedItems.Count > 0) 
            {                
                    listView1.SelectedItems.Clear(); // Сбрасываем выбор
                    richTextBox1.Clear();           // Очищаем текст описания
                    ResetMarkersColor();                
            }
        }

        // Метод для установки маркера
        private void MapPictureBox_MouseDoubleClick(object? sender, MouseEventArgs e)
        {
            if (mapPictureBox.Image == null || isAutoFitMode) return; // Запрещаем создание маркеров в адаптивном режиме
                                                                      // Проверяем, что двойной клик выполнен именно левой кнопкой мыши
            if (e.Button != MouseButtons.Left) return;                // Обрабаываем только нажатие левой кнопки, иначе выходим

            // Получаем размеры изображения
            Size imageSize = mapPictureBox.Image.Size;
            Size boxSize = mapPictureBox.ClientSize;

            // Проверяем, что клик внутри изображения (а не просто в PictureBox)
            Point? absolutePosition = GetAbsoluteCoordinates(e.Location);
            if (absolutePosition == null) return; // Если координаты вне изображения, выходим

            // Создаём новую метку
            Marker newMarker = new Marker(absolutePosition.Value);
            markers.Add(newMarker);
            AddMarkerToListView(newMarker);
            CreateMarkerPanel(newMarker);
            SelectListViewItemByMarker(newMarker.Id);

            isDraggingWas = true; // Здесь заставляем метод MouseUp думать, как будто было перетаскивание, чтобы не сбросилось выделение
        }

        // Переводим экранные координаты в абсолютные координаты изображения
        private Point? GetAbsoluteCoordinates(Point clickPosition)
        {
            if (mapPictureBox.Image == null) return null;

            Size imageSize = mapPictureBox.Image.Size;
            Size boxSize = mapPictureBox.ClientSize;

            // Рассчитываем масштаб изображения
            float scale = Math.Min((float)boxSize.Width / imageSize.Width, (float)boxSize.Height / imageSize.Height);

            // Вычисляем отступы (если картинка меньше, чем PictureBox)
            int offsetX = (boxSize.Width - (int)(imageSize.Width * scale)) / 2;
            int offsetY = (boxSize.Height - (int)(imageSize.Height * scale)) / 2;

            // Проверяем, кликнули ли внутри изображения
            if (clickPosition.X < offsetX || clickPosition.X > offsetX + imageSize.Width * scale ||
                clickPosition.Y < offsetY || clickPosition.Y > offsetY + imageSize.Height * scale)
            {
                return null; // Клик был вне изображения
            }

            // Переводим координаты в абсолютные
            int absoluteX = (int)((clickPosition.X - offsetX) / scale);
            int absoluteY = (int)((clickPosition.Y - offsetY) / scale);

            return new Point(absoluteX, absoluteY);
        }

        private void AddMarkerToListView(Marker marker)
        {
            // Формируем текст: имя + координаты
            string itemText = $"{marker.Name}";

            // Создаём новый элемент для ListView
            ListViewItem item = new ListViewItem(itemText);

            // Сохраняем объект Marker в Tag для полследующего использования
            item.Tag = marker;

            // Добавляем элемент в ListView
            listView1.Items.Add(item);

            // Сбрасываем флаг, так как порядок сортировки мог быть нарушен
            isSorted = false;
            isListModified = true; // Помечаем, что были изменения
        }

        private void CreateMarkerPanel(Marker marker)
        {
            RoundMarker markerPanel = new RoundMarker(marker);

            markerPanel.MarkerMouseClicked += Marker_MouseClicked; // Подписываемся на событие клика MouseEventArgs

            markerPanel.Location = new Point(marker.Position.X - RoundMarker.MarkerSize / 2,
                                             marker.Position.Y - RoundMarker.MarkerSize / 2);

            mapPictureBox.Controls.Add(markerPanel);
        }
        
        private void Form_ResizeEnd(object? sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                ResetMapPosition();
            }
        }

        private void Form_Resize(object? sender, EventArgs e)
        {
            // Проверяем, если окно возвращается из максимизированного состояния в нормальное
            if (lastWindowState == FormWindowState.Maximized && this.WindowState == FormWindowState.Normal)
            {
                // Сбрасываем координаты карты
                ResetMapPosition();
            }

            // Обновляем текущее состояние окна
            lastWindowState = this.WindowState;

            Form1_MouseDown(sender, FakeMouseEventArgs());
        }

        // Обработчик изменения выбора в ListView
        private void ListView1_SelectedIndexChanged(object? sender, EventArgs e)
        {            
            CancelMarkerMove(); // Cбрасываем режим перемещения при выборе другого маркера

            if (listView1.SelectedItems.Count > 0)
            {
                ListViewItem selectedItem = listView1.SelectedItems[0];

                if (selectedItem.Tag is Marker selectedMarker)
                {
                    HighlightMarker(selectedMarker);
                    richTextBox1.ResetText(); // Очистить содержимое
                    richTextBox1.Rtf = selectedMarker.Description;
                    richTextBox1.SelectionStart = 0;
                    richTextBox1.ScrollToCaret(); // Сбросить прокрутку в начало
                    richTextBox1.Invalidate();
                }
            }            
        }

        // Метод для выделения маркера
        private void HighlightMarker(Marker marker)
        {
            // Сначала сбрасываем цвет всех маркеров
            ResetMarkersColor();

            // Находим соответствующий маркер
            foreach (Control control in mapPictureBox.Controls)
            {
                if (control is RoundMarker roundMarker && roundMarker.MarkerData == marker)
                {
                    roundMarker.Highlight(); // Выделяем цветом
                    break;
                }
            }
        }

        // Cбрасываем цвет всех маркеров
        private void ResetMarkersColor ()
        {            
            foreach (Control control in mapPictureBox.Controls)
            {
                if (control is RoundMarker roundMarker)
                {
                    roundMarker.ResetColor(); // Сбрасываем цвет
                }
            }
        }

        // Обработчик события клика мыши по маркеру
        private void Marker_MouseClicked(object? sender, MouseEventArgs e)
        {
            // Проверяем, что источник события — это объект типа RoundMarker
            if (sender is RoundMarker markerPanel)
            {
                // Получаем связанные данные (Получаем объект Marker)
                var marker = markerPanel.MarkerData;

                // Если нажата левая кнопка мыши
                if (e.Button == MouseButtons.Left)
                {                    
                    SelectMarker(marker); // Выделяем маркер
                }

                // Если нажата правая кнопка мыши
                else if (e.Button == MouseButtons.Right)
                {
                    // Получаем текущий выбранный маркер из ListView
                    var selected = listView1.SelectedItems.Count > 0
                        ? listView1.SelectedItems[0].Tag as Marker
                        : null;

                    if (selected == marker)
                    {
                        markerContextMenu.Show(Cursor.Position);
                    }
                }
            }
        }

        internal void SelectMarker(Marker marker)
        {
            foreach (ListViewItem item in listView1.Items)
            {
                if (item.Tag is Marker listMarker && listMarker.Id == marker.Id)
                {
                    item.Selected = true;
                    item.EnsureVisible(); // Прокрутка к выделенному элементу
                    HighlightMarker(marker); // Выделяем маркер на карте
                    break;
                }
            }
        }

        // поиск маркера по его ID и вызов SelectMarker
        private void SelectListViewItemByMarker(Guid markerId)
        {
            foreach (var marker in markers)
            {
                if (marker.Id == markerId)
                {
                    SelectMarker(marker);
                    return; // Завершаем метод сразу после выбора маркера
                }
            }
        }

        private void RenameToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0) return;

            ListViewItem selectedItem = listView1.SelectedItems[0];
            Marker? marker = selectedItem.Tag as Marker;

            if (marker == null) return;

            // Создаём форму переименования
            RenameForm renameForm = new RenameForm(marker.Name);

            if (renameForm.ShowDialog() == DialogResult.OK)
            {
                // Обновляем текст в ListView
                marker.Name = renameForm.NewName;
                selectedItem.Text = renameForm.NewName;

                // Сбрасываем флаг, так как порядок сортировки мог быть нарушен
                isSorted = false;
                isListModified = true; // Помечаем, что были изменения
            }
        }

        private void DeleteToolStripMenuItem_Click(object? sender, EventArgs e)
        {

            DialogResult result = MessageBox.Show(
                    "Удалить?\nВы уверены?",
                    "Предупреждение",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                // Если пользователь нажал "Нет", ничего не делаем
                if (result == DialogResult.No)
                {
                    return;
                }

            if (listView1.SelectedItems.Count == 0) return;

            ListViewItem selectedItem = listView1.SelectedItems[0];
            if (selectedItem.Tag is not Marker selectedMarker) return;

            // Удаляем из списка markers
            markers.RemoveAll(m => m.Id == selectedMarker.Id);

            // Удаляем из listView1
            listView1.Items.Remove(selectedItem);

            // Удаляем круглый маркер с карты
            RemoveMarkerFromMap(selectedMarker);

            // Сбрасываем флаг, так как порядок сортировки мог быть нарушен
            isSorted = false;
            isListModified = true; // Помечаем, что были изменения
        }

        private void EditDescriptionToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0) return;

            ListViewItem selectedItem = listView1.SelectedItems[0];
            if (selectedItem.Tag is not Marker marker) return;

            // Открываем окно редактирования и передаем маркер
            EditDescriptionForm editForm = new EditDescriptionForm(marker);

            if (editForm.ShowDialog() == DialogResult.OK)
            {
                richTextBox1.Rtf = marker.Description;
                isListModified = true; // Помечаем, что были изменения
            }
        }

                
        private void RemoveMarkerFromMap(Marker marker)
        {   // Находим соответствующий круглый маркер
            // Проходимся по всем контролам карты
            foreach (Control control in mapPictureBox.Controls)
            {
                // Проверяем, что элемент это именно roundmarker
                if (control is RoundMarker roundMarker)
                {
                    // Проверяем совпадение Id маркера
                    if (roundMarker.MarkerData.Id == marker.Id)
                    {
                        mapPictureBox.Controls.Remove(roundMarker); // Удаляем с карты
                        roundMarker.Dispose(); // Освобождаем ресурсы
                        break; // Нашли нужный маркер — выходим из цикла
                    }
                }
            }
        }

        private void PositionFitButton()
        {
            // Получаем размер клиентской области формы
            var clientSize = this.ClientSize;
            btnToggleFit.Location = new Point(clientSize.Width - btnToggleFit.Width - 22, 28);
            
        }

        private void ToggleFitMode()
        {
            isAutoFitMode = !isAutoFitMode; // Переключаем состояние            

            if (isAutoFitMode)
            {
                mapPanel.AutoScroll = true;  // Форсируем обновление
                mapPanel.AutoScroll = false; // А потом сразу отключаем
                // Включаем адаптивный режим
                mapPictureBox.SizeMode = PictureBoxSizeMode.Zoom;
                mapPictureBox.Dock = DockStyle.Fill; // Растягиваем PictureBox                
            }
            else
            {
                // Возвращаем в стандартный режим
                mapPictureBox.SizeMode = PictureBoxSizeMode.Normal;
                mapPictureBox.Dock = DockStyle.None; // Отключаем автоматическое растяжение                

                // Восстанавливаем оригинальный размер карты
                if (mapPictureBox.Image != null)
                {
                    mapPictureBox.Size = mapPictureBox.Image.Size;
                }
                                
                PositionMap(); // Центрируем карту                
            }
            UpdateMarkerPositions();

            // Если есть выбранная метка, центрируем на ней
            if (!isAutoFitMode && listView1.SelectedItems.Count > 0 && listView1.SelectedItems[0].Tag is Marker selectedMarker)
            {
                CenterMapOnMarker(selectedMarker.Position);
            }            
        }

        // Метод для пересчета координат маркеров при изменении размера окна
        private void UpdateMarkerPositions()
        {
            foreach (Control control in mapPictureBox.Controls)
            {
                if (control is RoundMarker markerPanel)
                {
                    Point absolute = markerPanel.MarkerData.Position;
                    Point? newPosition = GetScaledPosition(absolute);

                    if (newPosition != null)
                    {
                        markerPanel.Location = new Point(newPosition.Value.X - RoundMarker.MarkerSize / 2,
                                                         newPosition.Value.Y - RoundMarker.MarkerSize / 2);
                    }
                }
            }
        }

        // Перевод абсолютных координат в экранные координаты
        private Point? GetScaledPosition(Point absolutePosition)
        {
            if (mapPictureBox.Image == null) return null;

            Size imageSize = mapPictureBox.Image.Size;
            Size boxSize = mapPictureBox.ClientSize;

            float scale = Math.Min((float)boxSize.Width / imageSize.Width, (float)boxSize.Height / imageSize.Height);

            int offsetX = (boxSize.Width - (int)(imageSize.Width * scale)) / 2;
            int offsetY = (boxSize.Height - (int)(imageSize.Height * scale)) / 2;

            int scaledX = (int)(absolutePosition.X * scale) + offsetX;
            int scaledY = (int)(absolutePosition.Y * scale) + offsetY;

            return new Point(scaledX, scaledY);
        }

        private void MapPictureBox_SizeChanged(object? sender, EventArgs e)
        {
            UpdateMarkerPositions();
        }        
    }


    [Serializable]
    public class Marker
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = "Метка"; // Устанавливаем имя по умолчанию
        public Point Position { get; set; }
        public string Description { get; set; } = string.Empty; // Оставляем описание пустым
                
        // Конструктор, который принимает координаты и генерирует имя с ними
        public Marker(Point position)
        {
            Id = Guid.NewGuid(); // Уникальный идентификатор
            Position = position;
            Name = $"Метка ({position.X}, {position.Y})"; // Имя с координатами
        }
    }

    public class RoundMarker : Panel
    {
        public static readonly int MarkerSize = 15; // Размер метки в пикселях

        private Color _currentColor;

        // Цвет по умолчанию
        public Color DefaultColor { get; set; } = Color.Black;
        // Цвет выделения
        public Color HighlightColor { get; set; } = Color.Red;

        public Marker MarkerData { get; }                   // Свойство для хранения данных о маркере
        public event MouseEventHandler? MarkerMouseClicked; // Событие клика с информацией о кнопке мыши

        public RoundMarker(Marker marker)
        {
            MarkerData = marker;
            _currentColor = DefaultColor; // Устанавливаем начальный цвет

            this.Size = new Size(MarkerSize, MarkerSize); // Размер метки
            this.BackColor = Color.Transparent; // Делаем фон прозрачным
            this.Paint += RoundMarker_Paint;

            this.MouseEnter += (s, e) => this.Cursor = Cursors.Hand; // Доп. проверка на вход
            this.MouseLeave += (s, e) => this.Cursor = Cursors.Default; // Возвращаем обычный курсор            

            // Обработка двойного клика по маркеру
            this.MouseDoubleClick += (s, e) =>
            {
                if (this.FindForm() is Form1 form)
                {
                    form.CenterMapOnMarker(MarkerData.Position);
                    form.SelectMarker(MarkerData);
                    form.isDraggingWas = true;
                }
            };
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            MarkerMouseClicked?.Invoke(this, e);
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            SetRegion();
        }

        private void SetRegion()
        {
            GraphicsPath path = new GraphicsPath();
            path.AddEllipse(0, 0, this.Width - 1, this.Height - 1);
            this.Region = new Region(path);
        }

        private void RoundMarker_Paint(object? sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using (Brush brush = new SolidBrush(_currentColor)) // Используем текущий цвет
            {
                e.Graphics.FillEllipse(brush, 0, 0, this.Width - 1, this.Height - 1);
            }
            using (Pen pen = new Pen(Color.Black, 1))
            {
                e.Graphics.DrawEllipse(pen, 0, 0, this.Width - 1, this.Height - 1);
            }
        }
                
        // Метод для выделения маркера
        public void Highlight()
        {
            _currentColor = HighlightColor;
            this.Invalidate();
        }

        // Сброс цвета
        public void ResetColor()
        {
            _currentColor = DefaultColor;
            this.Invalidate();
        }                
    }
}
