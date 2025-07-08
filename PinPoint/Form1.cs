using System.Drawing.Drawing2D;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Windows.Forms;

namespace PinPoint
{
    public partial class Form1 : Form
    {
        private bool isMouseOverRichTextBox = false; // ����, ������������� ��������� ���� ��� RichTextBox
        // ���������� ������ ���� �� ����� ���������� ����������
        public static Size tempDescriptionWindowSize = Size.Empty;
        // ���� ��� ������������, ���������� �� ������ �������������� �����
        private bool isDragging = false;

        // ���� ��� ������������ ����� ����������� �����, ���� ��� ���
        internal bool isDraggingWas = false;

        // ���� ��� ������������ ������ ����������� �������
        private bool isMovingMarker = false;

        // ��������� ��������������� ������� ���� (��� ������� �������� ��� �������������� �����)
        private Point lastMousePosition;

        // ���������� ��� ������������ ���������� ��������� ���� (�������, �����������)
        private FormWindowState lastWindowState = FormWindowState.Normal;

        // ������, � ������� �������� ��� ����� �� �����
        private List<Marker> markers = new List<Marker>();
        
        // ���� ��� ������������ ������ ����������� �����
        private bool isAutoFitMode = false; // false - �����������, true - ����������

        private bool isSorted = false; // ���� ��� ������������ ��������� ����������
        private bool isListModified = false; // ���� ��� ������������ ��������� � ������

        // ����������� ���� �������
        private ContextMenuStrip markerContextMenu;

        // ������ ��� �������� ����������� � ������� �������
        private System.Windows.Forms.Timer moveTimer = new System.Windows.Forms.Timer();
        private Point targetPosition;
        private const int animationSpeed = 10;

        public Form1()
        {
            InitializeComponent();

            // ������������� �� ������� ��������� �������� ���������, ����� �������������� ���������
            listView1.SizeChanged += OnSizeChangedOrSplitterMoved;
            splitter1.SplitterMoved += OnSizeChangedOrSplitterMoved;
            mapPanel.SizeChanged += MapPanel_SizeChanged;
            listView1.ColumnWidthChanging += ListView1_ColumnWidthChanging; // ��������� ��������� �������

            // ������������� �� ������� ��������� ������ � ������ �����
            listView1.SelectedIndexChanged += ListView1_SelectedIndexChanged;

            //������������� �� ������� ���� �� ������
            listView1.DoubleClick += ListView1_DoubleClick;

            //������������� �� ������� ����� �� ����� ������
            listView1.ColumnClick += ListView1_ColumnClick;

            // ������������� ��������� ��������� ����������� ������
            ResizeLastColumn();

            // ��������� �����
            LoadMap("Map.png");                      

            // ������������� �� ������� ��� ��������� �������������� ����� �����
            mapPictureBox.MouseDown += MapPictureBox_MouseDown;
            mapPictureBox.MouseMove += MapPictureBox_MouseMove;
            mapPictureBox.MouseUp += MapPictureBox_MouseUp;

            // ������������� �� ������� ��� ��������� �������� � RichTextBox1
            richTextBox1.MouseWheel += RichTextBox1_MouseWheel;
            richTextBox1.MouseEnter += (s, e) => isMouseOverRichTextBox = true;
            richTextBox1.MouseLeave += (s, e) => isMouseOverRichTextBox = false;

            mapPictureBox.MouseWheel += PictureBox_MouseWheel;            

            // ������������� �� ������� �������� ����� ��� ���������� ����� �����
            mapPictureBox.MouseDoubleClick += MapPictureBox_MouseDoubleClick;

            // ������������� �� ������� ��������� ������� �����
            this.Resize += Form_Resize;

            //��� �������� ���� ���������� ����� �������
            this.FormClosing += Form1_FormClosing;

            // ���������� ����������� ���� � ���������� ��������������, �������� � ��. ��� ������
            listView1.ContextMenuStrip = contextMenuStrip1;
            renameToolStripMenuItem.Click += RenameToolStripMenuItem_Click;
            deleteToolStripMenuItem.Click += DeleteToolStripMenuItem_Click;
            editDescriptionToolStripMenuItem.Click += EditDescriptionToolStripMenuItem_Click;

            // ���������� ������� ����
            SaveToolStripMenuItem.Click += SaveToolStripMenuItem_Click;
            LoadToolStripMenuItem.Click += LoadToolStripMenuItem_Click;
            NewToolStripMenuItem.Click += NewToolStripMenuItem_Click;
            ExitToolStripMenuItem.Click += ExitToolStripMenuItem_Click;

            // ������� ����������� ���� "�����������" ��� �������
            markerContextMenu = new ContextMenuStrip();
            var moveMenuItem = new ToolStripMenuItem("�����������");
            moveMenuItem.Click += MoveMarkerMenuItem_Click;
            // ��������� ����� ��������� ����
            markerContextMenu.Opening += (s, e) =>
            {
                // ���� �� � ���������� ������, �������� ����������� ����
                if (isAutoFitMode) {e.Cancel = true;}
            };

            markerContextMenu.Items.Add(moveMenuItem);

            PositionFitButton();
            // ������������� �� ��������� ������� �����, ����� ������ ���������� �� ����� �����
            this.Resize += (sender, e) => PositionFitButton();

            // ������������� �� ��������� ��������� ���������
            // mapPanel.Layout += (sender, e) => PositionFitButton();

            btnToggleFit.Click += (s, e) => ToggleFitMode();    // ���� �� ������ �������� ����� ������������ �������
            btnToggleFit.Click += (s, e) => listView1.Focus();  // ���������� ����� ������            

            mapPictureBox.SizeChanged += MapPictureBox_SizeChanged;

            moveTimer.Interval = 20; // ��� ������ ��������, ��� ������� ��������
            moveTimer.Tick += MoveTimer_Tick;

            this.MouseDown += Form1_MouseDown;
            mapPanel.MouseDown += Form1_MouseDown;
            listView1.MouseDown += Form1_MouseDown;
            richTextBox1.MouseDown += Form1_MouseDown;
            richTextBox1.MouseDown += RestoreFocus;
            btnToggleFit.MouseDown += Form1_MouseDown;
            menuStrip1.MouseDown += Form1_MouseDown;

            // ���������� ���������� ������� ��� ������ ���� "����"
            fileToolStripMenuItem.Click += (sender, e) => {Form1_MouseDown(sender, FakeMouseEventArgs());};

            this.KeyPreview = true; // ��� ����� ������ ������� ������ �� ���������
            this.KeyDown += Form1_KeyDown;

            ToggleFitMode();
            
        }

        private void PictureBox_MouseWheel(object? sender, MouseEventArgs e)
        {
            ((HandledMouseEventArgs)e).Handled = true; // ��������� ��������� �����������                                                       
        }

        private void RichTextBox1_MouseWheel(object? sender, MouseEventArgs e)
        {
            if (!isMouseOverRichTextBox) return;

            // ��������� ������ ���������� UI
            richTextBox1.SuspendLayout();

            // ��������� ����� ������ ��� ���������
            int currentLine = richTextBox1.GetLineFromCharIndex(richTextBox1.SelectionStart);
            int newLine = e.Delta > 0 ? Math.Max(0, currentLine - 3) : Math.Min(richTextBox1.Lines.Length - 1, currentLine + 3);
                        
            if (newLine < 0) newLine = 0;

            // ������������� ������ �� ������ ����� ������
            richTextBox1.SelectionStart = richTextBox1.GetFirstCharIndexFromLine(newLine);

            // ���������� ���������� ��������� UI
            richTextBox1.ResumeLayout();

        }

        private void RestoreFocus(object? sender, MouseEventArgs e) //���������� ����� ������
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
                // ����� ������ �����������
                CancelMarkerMove();
            }
        }

        // ����� ������ �����������
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
            // ������� � ���������� ������ MouseEventArgs � ���������� �������
            return new MouseEventArgs(
                MouseButtons.Left, // ����� ������ ����
                1,                 // ���� ����
                0,                 // ���������� X
                0,                 // ���������� Y
                0                  // ������ ���������
            );
        }

        private void Form1_MouseDown(object? sender, MouseEventArgs e)
        {                        
            // ����� ������ �����������
            CancelMarkerMove();
        }

        // ���� ������ ����� "�����������" � ��������� �������
        private void MoveMarkerMenuItem_Click(object? sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {                
               isMovingMarker = true;
               mapPictureBox.Cursor = Cursors.Cross; // ����������, ��� ��� ����� �� �����                
            }
        }


        // ��� ������� ����� �� ������
        private void ListView1_DoubleClick(object? sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0) return; // ���������, ���� �� ���������� �������

            ListViewItem selectedItem = listView1.SelectedItems[0];
            if (selectedItem.Tag is Marker marker) // ���������, ��� � Tag �������� ������ �����
            {
                CenterMapOnMarker(marker.Position);
            }
        }

        // �������� ������������� � ��������� �� �������
        internal void CenterMapOnMarker(Point markerPosition)
        {
            // ���������� �������� �������, �������� ������� ����� � ������
            int centerX = markerPosition.X - mapPanel.Width / 2;
            int centerY = markerPosition.Y - mapPanel.Height / 2;

            // ������������ ����������, ����� �� ����� �� ������� �����
            centerX = Math.Max(0, Math.Min(centerX, mapPictureBox.Width - mapPanel.Width));
            centerY = Math.Max(0, Math.Min(centerY, mapPictureBox.Height - mapPanel.Height));

            // ������������� ������� ������� � ��������� ��������
            targetPosition = new Point(centerX, centerY);
            moveTimer.Start();
        }

        private void MoveTimer_Tick(object? sender, EventArgs e) // ����������� � ���������� �������
        {
            // ������� ������� ��������� (� ������ �����)
            int currentX = -mapPanel.AutoScrollPosition.X;
            int currentY = -mapPanel.AutoScrollPosition.Y;

            // ��������� ���������� �� ����
            int deltaX = targetPosition.X - currentX;
            int deltaY = targetPosition.Y - currentY;

            // ���� ���������� ��������� � ��������� ��������
            if (Math.Abs(deltaX) < 2 && Math.Abs(deltaY) < 2)
            {
                mapPanel.AutoScrollPosition = targetPosition; // ��������� �����
                moveTimer.Stop();
                return;
            }

            // ��� ����� � ����, ��� ������ ��� �����������
            int speedX = Math.Max(1, Math.Abs(deltaX) / animationSpeed);
            int speedY = Math.Max(1, Math.Abs(deltaY) / animationSpeed);

            // ��������� � ������� ����
            int newX = currentX + Math.Sign(deltaX) * speedX;
            int newY = currentY + Math.Sign(deltaY) * speedY;

            mapPanel.AutoScrollPosition = new Point(newX, newY);
        }

        private void Form1_FormClosing(object? sender, FormClosingEventArgs e)
        {
            if (listView1.SelectedItems.Count != 0) // ���� ������ �� ������
            {
                // ���������, ���� �� ��������� � ������
                if (isListModified)
                {
                    // ���������� ���������� ���� ��� �������������
                    DialogResult result = MessageBox.Show("� ��� �������� ������������� ���������.\n�� ������������� ������ ����� �� ����������?",
                                             "������������� ������",
                                             MessageBoxButtons.YesNo,
                                             MessageBoxIcon.Question);

                    // ���� ������������ ����� "���"
                    if (result == DialogResult.No)
                    {
                        e.Cancel = true; // �������� �������� �����
                    }
                }
            }

            Form1_MouseDown(sender, FakeMouseEventArgs());
        }

        private void ListView1_ColumnClick(object? sender, ColumnClickEventArgs e)
        {            
            // ���������, ��������� �� ������ � ����������
            if (!isSorted)  // ���� ��, �� ��������� �� �����������
            {
                ToggleSorting(SortOrder.Ascending);
                isSorted = true;
            }
            else            // ���� ���, ��������� �� �������� (��������������)
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
            
            Close(); // ��������� �����
            
        }

        private void NewToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            // ���� ���� ������������ ���������, ���������� � ������������
            if (isListModified)
            {
                DialogResult result = MessageBox.Show(
                    "������� ����� ������?\n��� ������������� ��������� ����� ��������.",
                    "��������������",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                // ���� ������������ ����� "���", ��������� �������� ������ ������
                if (result == DialogResult.No)
                {
                    return;
                }
            }

            // ������� ������
            markers.Clear();                // ������� ������ ��������
            listView1.Items.Clear();        // ������� ������ � ListView
            mapPictureBox.Controls.Clear(); // ������� ������� � �����
            richTextBox1.Clear();           // ������� ��������� ���� ��������
            isListModified = false;         // ���������� ���� ���������, ��� ��� ������ ����� ������
            UpdateWindowTitle("");
        }

        private void LoadToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            // ���� ���� ���������, ���������� � ������������
            if (isListModified)
            {
                DialogResult result = MessageBox.Show(
                    "� ��� ���� ������������ ���������.\n���������� ��� ����������?",
                    "��������������",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                // ���� ������������ ����� "���", ��������� �������� ��������
                if (result == DialogResult.No)
                {
                    return;
                }
            }

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "������ �������� (*.jdat)|*.jdat|��� ����� (*.*)|*.*";
                openFileDialog.Title = "��������� ������";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    LoadMarkers(openFileDialog.FileName); // �������� ����� ��������
                    isListModified = false; // ���������� ���� ��������� ����� �������� ��������
                }
            }
        }

        private void SaveToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog()) 
            {
                saveFileDialog.Filter = "������ �������� (*.jdat)|*.jdat|��� ����� (*.*)|*.*";
                saveFileDialog.Title = "��������� ������";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    SaveMarkers(saveFileDialog.FileName); // �������� ����� ����������
                    isListModified = false; // ���������� ���� ��������� ����� ��������� ����������
                }
            }                
        }

        private void SaveMarkers (string filePath)
        {
            string json = JsonSerializer.Serialize(markers, new JsonSerializerOptions { 
                WriteIndented = true, 
                IgnoreReadOnlyProperties = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping  // ������� ������� ��� �������������, � ����������
            });
            File.WriteAllText(filePath, json);            
            UpdateWindowTitle(filePath);
        }

        private void LoadMarkers(string filePath)
        {
            try
            {
                string json = File.ReadAllText(filePath); // ������ JSON �� �����
                markers = JsonSerializer.Deserialize<List<Marker>>(json) ?? new List<Marker>();

                // ������� ListView � ��������� ������
                listView1.Items.Clear();
                mapPictureBox.Controls.Clear(); // ������� ������ �������                

                foreach (var marker in markers)
                {                    
                    AddMarkerToListView(marker);  // ��������� � ������
                    CreateMarkerPanel(marker);    // ������ ������ �� �����
                }
                richTextBox1.Clear(); // ������� �������� �������

                if (isAutoFitMode)
                {
                    UpdateMarkerPositions();
                }
                                
                UpdateWindowTitle(filePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"������ �������� �����: {ex.Message}", "������", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ����� ��������� ������ ���������� ������� ListView ���, ����� �� ������� ��� ���������� ������
        private void ResizeLastColumn()
        {
            if (listView1.Columns.Count == 0) return;

            int totalWidth = listView1.ClientSize.Width; // ����� ������ ������
            int usedWidth = 0;

            // ������� ������ ���� ��������, ����� ����������
            for (int i = 0; i < listView1.Columns.Count - 1; i++)
            {
                usedWidth += listView1.Columns[i].Width;
            }

            listView1.Columns[listView1.Columns.Count - 1].Width = Math.Max(totalWidth - usedWidth, 50);
        }

        private void OnSizeChangedOrSplitterMoved(object? sender, EventArgs e)
        {
            ResizeLastColumn(); // �������� �������� ������ ������� ������ ��� ��������� ��������
            PositionMap(); // ���������� �����
        }

        // ��������� ��������� ������ ���������� ������� �������
        private void ListView1_ColumnWidthChanging(object? sender, ColumnWidthChangingEventArgs e)
        {
            if (e.ColumnIndex == listView1.Columns.Count - 1) // ���� ��� ��������� �������
            {
                e.Cancel = true; // �������� ��������� ������
                e.NewWidth = listView1.Columns[e.ColumnIndex].Width; // ���������� ������ ������
            }
        }

        // ��������� ����������� �����
        private void LoadMap(string path)
        {
            try
            {
                // ���������, ���������� �� ���� �����
                if (File.Exists(path))
                {
                    // ���� ���� ����������, ��������� �����
                    mapPictureBox.Image = Image.FromFile(path);
                    // ���������� �����
                    PositionMap();
                    // ������� �� ������, ��� ��� �� ������ �������
                    return;
                }

                // ���� ���� �� ������, ���������� ��������� �� ������
                MessageBox.Show("���� Map.png �� ������.", "������", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                // ������������ ����� ������ ��� �������� �����
                MessageBox.Show($"������ ��� �������� �����: {ex.Message}", "������", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // ���� ���-�� ����� �� ���, ��������� ����������
            mapPictureBox.Image?.Dispose(); // �� ������, ���� ����� �������� ����������� � �������, ������ �� �����
            Environment.Exit(1);
        }

        // ����� ���������� ����� � ������, ���� ����� ������ ������
        private void PositionMap()
        {
            if (mapPictureBox.Image == null) return;

            // ���������� �������������� ��������� (����� ����������� ������ ������)
            mapPanel.AutoScroll = false;
            mapPanel.AutoScrollPosition = new Point(0, 0);

            // �������� ������� �����
            int mapWidth = mapPictureBox.Image.Width;
            int mapHeight = mapPictureBox.Image.Height;
            // �������� ������� ������
            int panelWidth = mapPanel.ClientSize.Width;
            int panelHeight = mapPanel.ClientSize.Height;
                        
            // ��������� ����� ���������� ��� �������������
            int startX = Math.Max((panelWidth - mapWidth) / 2, 0);
            int startY = Math.Max((panelHeight - mapHeight) / 2, 0);
            
            // ������������� ����� ��������� �����
            mapPictureBox.Location = new Point(startX, startY);

            // �������� �������������, ���� ����� ������ ������
            mapPanel.AutoScroll = mapWidth > panelWidth || mapHeight > panelHeight;

        }

        // �������� ������� �����
        private void ResetMapPosition()
        {
            if (mapPictureBox.Image == null) return;
            mapPictureBox.Location = new Point(0, 0);
        }

        // ��������� ����� ��������� ��������� ��� ��������� �������� �����
        private void MapPanel_SizeChanged(object? sender, EventArgs e)
        {
            PositionMap();
            UpdateMarkerPositions();
        }

        // �������� �������������� ����� ��� ����������� ������� (���� ������� ����� �����������)
        // ��������� ������� ������ ���� �� �����
        private void MapPictureBox_MouseDown(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // ���� ������� ����� ����������� ������� � � ������ ������ ���� �� ���� �������
                if (isMovingMarker && listView1.SelectedItems.Count > 0)
                {
                    // �������� ������ ��������� ������� ������
                    var selectedItem = listView1.SelectedItems[0];

                    // ���������, ��� Tag �� ����� null � �������� ������ ���� Marker
                    if (selectedItem.Tag is Marker marker)
                    {
                        // ��������� ������� ������� ������� ������� ����
                        marker.Position = e.Location;

                        // ���� ��������������� ������� (RoundMarker), ����� �������� ��� ���������
                        foreach (Control ctrl in mapPictureBox.Controls)
                        {
                            // ���������, ��� ��� ������ RoundMarker � ��� ��� Id ��������� � Id �������
                            if (ctrl is RoundMarker rm && rm.MarkerData.Id == marker.Id)
                            {
                                // ������� ���������� ������ ���, ����� �� ������������� �� �����������
                                rm.Location = new Point(
                                    marker.Position.X - RoundMarker.MarkerSize / 2,
                                    marker.Position.Y - RoundMarker.MarkerSize / 2);
                                isListModified = true;
                                break; // ����� ������ ������ � ������� �� �����
                            }
                        }

                        // ��������� ����� �����������
                        isMovingMarker = false;
                        mapPictureBox.Cursor = Cursors.Default;
                        isDraggingWas = true; // ������������� ����� ��������� ��� MouseUp
                    }
                }
                else
                {
                    // ���� ����� ����������� �� ������� � ��������� ������� �������������� �����
                    isDragging = true;
                    isDraggingWas = false;
                    moveTimer.Stop(); // ������������� ������� �������� �����
                    lastMousePosition = mapPanel.PointToClient(Cursor.Position); // ��������� ������� ����
                }
            }
        }

        // ����������� ����� ��� ��������� ����
        private void MapPictureBox_MouseMove(object? sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                isDraggingWas = true; // �������� ��� ���� �������������� �����

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
            if (!isDraggingWas) // ���� �������������� ������ 
            {
                DeselectMarker(e);
            }            
            
        }

        private void DeselectMarker(MouseEventArgs e) 
        {
            // ���� ��� ����� ���� � ��� ������ �����-���� �������            
            if (e.Button == MouseButtons.Left && listView1.SelectedItems.Count > 0) 
            {                
                    listView1.SelectedItems.Clear(); // ���������� �����
                    richTextBox1.Clear();           // ������� ����� ��������
                    ResetMarkersColor();                
            }
        }

        // ����� ��� ��������� �������
        private void MapPictureBox_MouseDoubleClick(object? sender, MouseEventArgs e)
        {
            if (mapPictureBox.Image == null || isAutoFitMode) return; // ��������� �������� �������� � ���������� ������
                                                                      // ���������, ��� ������� ���� �������� ������ ����� ������� ����
            if (e.Button != MouseButtons.Left) return;                // ����������� ������ ������� ����� ������, ����� �������

            // �������� ������� �����������
            Size imageSize = mapPictureBox.Image.Size;
            Size boxSize = mapPictureBox.ClientSize;

            // ���������, ��� ���� ������ ����������� (� �� ������ � PictureBox)
            Point? absolutePosition = GetAbsoluteCoordinates(e.Location);
            if (absolutePosition == null) return; // ���� ���������� ��� �����������, �������

            // ������ ����� �����
            Marker newMarker = new Marker(absolutePosition.Value);
            markers.Add(newMarker);
            AddMarkerToListView(newMarker);
            CreateMarkerPanel(newMarker);
            SelectListViewItemByMarker(newMarker.Id);

            isDraggingWas = true; // ����� ���������� ����� MouseUp ������, ��� ����� ���� ��������������, ����� �� ���������� ���������
        }

        // ��������� �������� ���������� � ���������� ���������� �����������
        private Point? GetAbsoluteCoordinates(Point clickPosition)
        {
            if (mapPictureBox.Image == null) return null;

            Size imageSize = mapPictureBox.Image.Size;
            Size boxSize = mapPictureBox.ClientSize;

            // ������������ ������� �����������
            float scale = Math.Min((float)boxSize.Width / imageSize.Width, (float)boxSize.Height / imageSize.Height);

            // ��������� ������� (���� �������� ������, ��� PictureBox)
            int offsetX = (boxSize.Width - (int)(imageSize.Width * scale)) / 2;
            int offsetY = (boxSize.Height - (int)(imageSize.Height * scale)) / 2;

            // ���������, �������� �� ������ �����������
            if (clickPosition.X < offsetX || clickPosition.X > offsetX + imageSize.Width * scale ||
                clickPosition.Y < offsetY || clickPosition.Y > offsetY + imageSize.Height * scale)
            {
                return null; // ���� ��� ��� �����������
            }

            // ��������� ���������� � ����������
            int absoluteX = (int)((clickPosition.X - offsetX) / scale);
            int absoluteY = (int)((clickPosition.Y - offsetY) / scale);

            return new Point(absoluteX, absoluteY);
        }

        private void AddMarkerToListView(Marker marker)
        {
            // ��������� �����: ��� + ����������
            string itemText = $"{marker.Name}";

            // ������ ����� ������� ��� ListView
            ListViewItem item = new ListViewItem(itemText);

            // ��������� ������ Marker � Tag ��� ������������� �������������
            item.Tag = marker;

            // ��������� ������� � ListView
            listView1.Items.Add(item);

            // ���������� ����, ��� ��� ������� ���������� ��� ���� �������
            isSorted = false;
            isListModified = true; // ��������, ��� ���� ���������
        }

        private void CreateMarkerPanel(Marker marker)
        {
            RoundMarker markerPanel = new RoundMarker(marker);

            markerPanel.MarkerMouseClicked += Marker_MouseClicked; // ������������� �� ������� ����� MouseEventArgs

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
            // ���������, ���� ���� ������������ �� ������������������ ��������� � ����������
            if (lastWindowState == FormWindowState.Maximized && this.WindowState == FormWindowState.Normal)
            {
                // ���������� ���������� �����
                ResetMapPosition();
            }

            // ��������� ������� ��������� ����
            lastWindowState = this.WindowState;

            Form1_MouseDown(sender, FakeMouseEventArgs());
        }

        // ���������� ��������� ������ � ListView
        private void ListView1_SelectedIndexChanged(object? sender, EventArgs e)
        {            
            CancelMarkerMove(); // C��������� ����� ����������� ��� ������ ������� �������

            if (listView1.SelectedItems.Count > 0)
            {
                ListViewItem selectedItem = listView1.SelectedItems[0];

                if (selectedItem.Tag is Marker selectedMarker)
                {
                    HighlightMarker(selectedMarker);
                    richTextBox1.ResetText(); // �������� ����������
                    richTextBox1.Rtf = selectedMarker.Description;
                    richTextBox1.SelectionStart = 0;
                    richTextBox1.ScrollToCaret(); // �������� ��������� � ������
                    richTextBox1.Invalidate();
                }
            }            
        }

        // ����� ��� ��������� �������
        private void HighlightMarker(Marker marker)
        {
            // ������� ���������� ���� ���� ��������
            ResetMarkersColor();

            // ������� ��������������� ������
            foreach (Control control in mapPictureBox.Controls)
            {
                if (control is RoundMarker roundMarker && roundMarker.MarkerData == marker)
                {
                    roundMarker.Highlight(); // �������� ������
                    break;
                }
            }
        }

        // C��������� ���� ���� ��������
        private void ResetMarkersColor ()
        {            
            foreach (Control control in mapPictureBox.Controls)
            {
                if (control is RoundMarker roundMarker)
                {
                    roundMarker.ResetColor(); // ���������� ����
                }
            }
        }

        // ���������� ������� ����� ���� �� �������
        private void Marker_MouseClicked(object? sender, MouseEventArgs e)
        {
            // ���������, ��� �������� ������� � ��� ������ ���� RoundMarker
            if (sender is RoundMarker markerPanel)
            {
                // �������� ��������� ������ (�������� ������ Marker)
                var marker = markerPanel.MarkerData;

                // ���� ������ ����� ������ ����
                if (e.Button == MouseButtons.Left)
                {                    
                    SelectMarker(marker); // �������� ������
                }

                // ���� ������ ������ ������ ����
                else if (e.Button == MouseButtons.Right)
                {
                    // �������� ������� ��������� ������ �� ListView
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
                    item.EnsureVisible(); // ��������� � ����������� ��������
                    HighlightMarker(marker); // �������� ������ �� �����
                    break;
                }
            }
        }

        // ����� ������� �� ��� ID � ����� SelectMarker
        private void SelectListViewItemByMarker(Guid markerId)
        {
            foreach (var marker in markers)
            {
                if (marker.Id == markerId)
                {
                    SelectMarker(marker);
                    return; // ��������� ����� ����� ����� ������ �������
                }
            }
        }

        private void RenameToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0) return;

            ListViewItem selectedItem = listView1.SelectedItems[0];
            Marker? marker = selectedItem.Tag as Marker;

            if (marker == null) return;

            // ������ ����� ��������������
            RenameForm renameForm = new RenameForm(marker.Name);

            if (renameForm.ShowDialog() == DialogResult.OK)
            {
                // ��������� ����� � ListView
                marker.Name = renameForm.NewName;
                selectedItem.Text = renameForm.NewName;

                // ���������� ����, ��� ��� ������� ���������� ��� ���� �������
                isSorted = false;
                isListModified = true; // ��������, ��� ���� ���������
            }
        }

        private void DeleteToolStripMenuItem_Click(object? sender, EventArgs e)
        {

            DialogResult result = MessageBox.Show(
                    "�������?\n�� �������?",
                    "��������������",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                // ���� ������������ ����� "���", ������ �� ������
                if (result == DialogResult.No)
                {
                    return;
                }

            if (listView1.SelectedItems.Count == 0) return;

            ListViewItem selectedItem = listView1.SelectedItems[0];
            if (selectedItem.Tag is not Marker selectedMarker) return;

            // ������� �� ������ markers
            markers.RemoveAll(m => m.Id == selectedMarker.Id);

            // ������� �� listView1
            listView1.Items.Remove(selectedItem);

            // ������� ������� ������ � �����
            RemoveMarkerFromMap(selectedMarker);

            // ���������� ����, ��� ��� ������� ���������� ��� ���� �������
            isSorted = false;
            isListModified = true; // ��������, ��� ���� ���������
        }

        private void EditDescriptionToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0) return;

            ListViewItem selectedItem = listView1.SelectedItems[0];
            if (selectedItem.Tag is not Marker marker) return;

            // ��������� ���� �������������� � �������� ������
            EditDescriptionForm editForm = new EditDescriptionForm(marker);

            if (editForm.ShowDialog() == DialogResult.OK)
            {
                richTextBox1.Rtf = marker.Description;
                isListModified = true; // ��������, ��� ���� ���������
            }
        }

                
        private void RemoveMarkerFromMap(Marker marker)
        {   // ������� ��������������� ������� ������
            // ���������� �� ���� ��������� �����
            foreach (Control control in mapPictureBox.Controls)
            {
                // ���������, ��� ������� ��� ������ roundmarker
                if (control is RoundMarker roundMarker)
                {
                    // ��������� ���������� Id �������
                    if (roundMarker.MarkerData.Id == marker.Id)
                    {
                        mapPictureBox.Controls.Remove(roundMarker); // ������� � �����
                        roundMarker.Dispose(); // ����������� �������
                        break; // ����� ������ ������ � ������� �� �����
                    }
                }
            }
        }

        private void PositionFitButton()
        {
            // �������� ������ ���������� ������� �����
            var clientSize = this.ClientSize;
            btnToggleFit.Location = new Point(clientSize.Width - btnToggleFit.Width - 22, 28);
            
        }

        private void ToggleFitMode()
        {
            isAutoFitMode = !isAutoFitMode; // ����������� ���������            

            if (isAutoFitMode)
            {
                mapPanel.AutoScroll = true;  // ��������� ����������
                mapPanel.AutoScroll = false; // � ����� ����� ���������
                // �������� ���������� �����
                mapPictureBox.SizeMode = PictureBoxSizeMode.Zoom;
                mapPictureBox.Dock = DockStyle.Fill; // ����������� PictureBox                
            }
            else
            {
                // ���������� � ����������� �����
                mapPictureBox.SizeMode = PictureBoxSizeMode.Normal;
                mapPictureBox.Dock = DockStyle.None; // ��������� �������������� ����������                

                // ��������������� ������������ ������ �����
                if (mapPictureBox.Image != null)
                {
                    mapPictureBox.Size = mapPictureBox.Image.Size;
                }
                                
                PositionMap(); // ���������� �����                
            }
            UpdateMarkerPositions();

            // ���� ���� ��������� �����, ���������� �� ���
            if (!isAutoFitMode && listView1.SelectedItems.Count > 0 && listView1.SelectedItems[0].Tag is Marker selectedMarker)
            {
                CenterMapOnMarker(selectedMarker.Position);
            }            
        }

        // ����� ��� ��������� ��������� �������� ��� ��������� ������� ����
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

        // ������� ���������� ��������� � �������� ����������
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
        public string Name { get; set; } = "�����"; // ������������� ��� �� ���������
        public Point Position { get; set; }
        public string Description { get; set; } = string.Empty; // ��������� �������� ������
                
        // �����������, ������� ��������� ���������� � ���������� ��� � ����
        public Marker(Point position)
        {
            Id = Guid.NewGuid(); // ���������� �������������
            Position = position;
            Name = $"����� ({position.X}, {position.Y})"; // ��� � ������������
        }
    }

    public class RoundMarker : Panel
    {
        public static readonly int MarkerSize = 15; // ������ ����� � ��������

        private Color _currentColor;

        // ���� �� ���������
        public Color DefaultColor { get; set; } = Color.Black;
        // ���� ���������
        public Color HighlightColor { get; set; } = Color.Red;

        public Marker MarkerData { get; }                   // �������� ��� �������� ������ � �������
        public event MouseEventHandler? MarkerMouseClicked; // ������� ����� � ����������� � ������ ����

        public RoundMarker(Marker marker)
        {
            MarkerData = marker;
            _currentColor = DefaultColor; // ������������� ��������� ����

            this.Size = new Size(MarkerSize, MarkerSize); // ������ �����
            this.BackColor = Color.Transparent; // ������ ��� ����������
            this.Paint += RoundMarker_Paint;

            this.MouseEnter += (s, e) => this.Cursor = Cursors.Hand; // ���. �������� �� ����
            this.MouseLeave += (s, e) => this.Cursor = Cursors.Default; // ���������� ������� ������            

            // ��������� �������� ����� �� �������
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
            using (Brush brush = new SolidBrush(_currentColor)) // ���������� ������� ����
            {
                e.Graphics.FillEllipse(brush, 0, 0, this.Width - 1, this.Height - 1);
            }
            using (Pen pen = new Pen(Color.Black, 1))
            {
                e.Graphics.DrawEllipse(pen, 0, 0, this.Width - 1, this.Height - 1);
            }
        }
                
        // ����� ��� ��������� �������
        public void Highlight()
        {
            _currentColor = HighlightColor;
            this.Invalidate();
        }

        // ����� �����
        public void ResetColor()
        {
            _currentColor = DefaultColor;
            this.Invalidate();
        }                
    }
}
