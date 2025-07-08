namespace PinPoint
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            listView1 = new ListView();
            columnHeader1 = new ColumnHeader();
            splitter1 = new Splitter();
            richTextBox1 = new RichTextBox();
            splitter2 = new Splitter();
            mapPanel = new Panel();
            mapPictureBox = new PictureBox();
            btnToggleFit = new Button();
            menuStrip1 = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            NewToolStripMenuItem = new ToolStripMenuItem();
            LoadToolStripMenuItem = new ToolStripMenuItem();
            SaveToolStripMenuItem = new ToolStripMenuItem();
            ExitToolStripMenuItem = new ToolStripMenuItem();
            contextMenuStrip1 = new ContextMenuStrip(components);
            renameToolStripMenuItem = new ToolStripMenuItem();
            deleteToolStripMenuItem = new ToolStripMenuItem();
            editDescriptionToolStripMenuItem = new ToolStripMenuItem();
            mapPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)mapPictureBox).BeginInit();
            menuStrip1.SuspendLayout();
            contextMenuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // listView1
            // 
            listView1.Columns.AddRange(new ColumnHeader[] { columnHeader1 });
            listView1.Dock = DockStyle.Left;
            listView1.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 204);
            listView1.FullRowSelect = true;
            listView1.Location = new Point(0, 24);
            listView1.MultiSelect = false;
            listView1.Name = "listView1";
            listView1.Size = new Size(200, 428);
            listView1.TabIndex = 0;
            listView1.UseCompatibleStateImageBehavior = false;
            listView1.View = View.Details;
            // 
            // columnHeader1
            // 
            columnHeader1.Text = "-- Наименование объекта --";
            // 
            // splitter1
            // 
            splitter1.Location = new Point(200, 24);
            splitter1.Name = "splitter1";
            splitter1.Size = new Size(4, 428);
            splitter1.TabIndex = 1;
            splitter1.TabStop = false;
            // 
            // richTextBox1
            // 
            richTextBox1.Dock = DockStyle.Bottom;
            richTextBox1.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 204);
            richTextBox1.Location = new Point(204, 356);
            richTextBox1.Name = "richTextBox1";
            richTextBox1.ReadOnly = true;
            richTextBox1.Size = new Size(598, 96);
            richTextBox1.TabIndex = 2;
            richTextBox1.Text = "";
            // 
            // splitter2
            // 
            splitter2.Dock = DockStyle.Bottom;
            splitter2.Location = new Point(204, 352);
            splitter2.Name = "splitter2";
            splitter2.Size = new Size(598, 4);
            splitter2.TabIndex = 3;
            splitter2.TabStop = false;
            // 
            // mapPanel
            // 
            mapPanel.AutoScroll = true;
            mapPanel.Controls.Add(mapPictureBox);
            mapPanel.Dock = DockStyle.Fill;
            mapPanel.Location = new Point(204, 24);
            mapPanel.Name = "mapPanel";
            mapPanel.Size = new Size(598, 328);
            mapPanel.TabIndex = 4;
            // 
            // mapPictureBox
            // 
            mapPictureBox.Location = new Point(0, 0);
            mapPictureBox.Name = "mapPictureBox";
            mapPictureBox.Size = new Size(676, 327);
            mapPictureBox.SizeMode = PictureBoxSizeMode.AutoSize;
            mapPictureBox.TabIndex = 0;
            mapPictureBox.TabStop = false;
            // 
            // btnToggleFit
            // 
            btnToggleFit.FlatStyle = FlatStyle.System;
            btnToggleFit.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 204);
            btnToggleFit.Location = new Point(755, 40);
            btnToggleFit.Name = "btnToggleFit";
            btnToggleFit.Padding = new Padding(2, 0, 0, 0);
            btnToggleFit.Size = new Size(28, 28);
            btnToggleFit.TabIndex = 1;
            btnToggleFit.Text = "⛶";
            btnToggleFit.UseVisualStyleBackColor = true;
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(802, 24);
            menuStrip1.TabIndex = 5;
            menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { NewToolStripMenuItem, LoadToolStripMenuItem, SaveToolStripMenuItem, ExitToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(48, 20);
            fileToolStripMenuItem.Text = "Файл";
            // 
            // NewToolStripMenuItem
            // 
            NewToolStripMenuItem.Name = "NewToolStripMenuItem";
            NewToolStripMenuItem.Size = new Size(132, 22);
            NewToolStripMenuItem.Text = "Новый";
            // 
            // LoadToolStripMenuItem
            // 
            LoadToolStripMenuItem.Name = "LoadToolStripMenuItem";
            LoadToolStripMenuItem.Size = new Size(132, 22);
            LoadToolStripMenuItem.Text = "Загрузить";
            // 
            // SaveToolStripMenuItem
            // 
            SaveToolStripMenuItem.Name = "SaveToolStripMenuItem";
            SaveToolStripMenuItem.Size = new Size(132, 22);
            SaveToolStripMenuItem.Text = "Сохранить";
            // 
            // ExitToolStripMenuItem
            // 
            ExitToolStripMenuItem.Name = "ExitToolStripMenuItem";
            ExitToolStripMenuItem.Size = new Size(132, 22);
            ExitToolStripMenuItem.Text = "Выход";
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.Items.AddRange(new ToolStripItem[] { renameToolStripMenuItem, deleteToolStripMenuItem, editDescriptionToolStripMenuItem });
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.Size = new Size(211, 70);
            // 
            // renameToolStripMenuItem
            // 
            renameToolStripMenuItem.Name = "renameToolStripMenuItem";
            renameToolStripMenuItem.Size = new Size(210, 22);
            renameToolStripMenuItem.Text = "Переименовать";
            // 
            // deleteToolStripMenuItem
            // 
            deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            deleteToolStripMenuItem.Size = new Size(210, 22);
            deleteToolStripMenuItem.Text = "Удалить";
            // 
            // editDescriptionToolStripMenuItem
            // 
            editDescriptionToolStripMenuItem.Name = "editDescriptionToolStripMenuItem";
            editDescriptionToolStripMenuItem.Size = new Size(210, 22);
            editDescriptionToolStripMenuItem.Text = "Редактировать описание";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(802, 452);
            Controls.Add(btnToggleFit);
            Controls.Add(mapPanel);
            Controls.Add(splitter2);
            Controls.Add(richTextBox1);
            Controls.Add(splitter1);
            Controls.Add(listView1);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            Name = "Form1";
            Text = "PinPoint (beta)";
            WindowState = FormWindowState.Maximized;
            mapPanel.ResumeLayout(false);
            mapPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)mapPictureBox).EndInit();
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            contextMenuStrip1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ListView listView1;
        private Splitter splitter1;
        private RichTextBox richTextBox1;
        private Splitter splitter2;
        private ColumnHeader columnHeader1;
        private Panel mapPanel;
        private PictureBox mapPictureBox;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem NewToolStripMenuItem;
        private ToolStripMenuItem LoadToolStripMenuItem;
        private ToolStripMenuItem SaveToolStripMenuItem;
        private ToolStripMenuItem ExitToolStripMenuItem;
        private ContextMenuStrip contextMenuStrip1;
        private ToolStripMenuItem renameToolStripMenuItem;
        private ToolStripMenuItem deleteToolStripMenuItem;
        private Button btnToggleFit;
        private ToolStripMenuItem editDescriptionToolStripMenuItem;
    }
}
