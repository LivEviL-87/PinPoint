namespace PinPoint
{
    partial class EditDescriptionForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditDescriptionForm));
            richTextBoxDescription = new RichTextBox();
            btnSave = new Button();
            btnCancel = new Button();
            tableLayoutPanel1 = new TableLayoutPanel();
            flowLayoutPanel1 = new FlowLayoutPanel();
            panel1 = new Panel();
            groupBox2 = new GroupBox();
            groupBox1 = new GroupBox();
            toolStrip = new ToolStrip();
            toolStripLabel1 = new ToolStripLabel();
            fontComboBox = new ToolStripComboBox();
            toolStripSeparator1 = new ToolStripSeparator();
            toolStripLabel2 = new ToolStripLabel();
            sizeComboBox = new ToolStripComboBox();
            toolStripSeparator2 = new ToolStripSeparator();
            toolStripLabel3 = new ToolStripLabel();
            colorComboBox = new ToolStripComboBox();
            toolStripSeparator3 = new ToolStripSeparator();
            boldButton = new ToolStripButton();
            toolStripSeparator4 = new ToolStripSeparator();
            italicButton = new ToolStripButton();
            toolStripSeparator5 = new ToolStripSeparator();
            underlineButton = new ToolStripButton();
            tableLayoutPanel1.SuspendLayout();
            flowLayoutPanel1.SuspendLayout();
            panel1.SuspendLayout();
            groupBox1.SuspendLayout();
            toolStrip.SuspendLayout();
            SuspendLayout();
            // 
            // richTextBoxDescription
            // 
            richTextBoxDescription.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            richTextBoxDescription.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 204);
            richTextBoxDescription.Location = new Point(7, 3);
            richTextBoxDescription.Name = "richTextBoxDescription";
            richTextBoxDescription.Size = new Size(837, 308);
            richTextBoxDescription.TabIndex = 0;
            richTextBoxDescription.Text = "";
            // 
            // btnSave
            // 
            btnSave.Anchor = AnchorStyles.Left;
            btnSave.Location = new Point(5, 5);
            btnSave.Margin = new Padding(5);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(84, 23);
            btnSave.TabIndex = 1;
            btnSave.Text = "Сохранить";
            btnSave.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            btnCancel.Anchor = AnchorStyles.Right;
            btnCancel.Location = new Point(189, 5);
            btnCancel.Margin = new Padding(5);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(84, 23);
            btnCancel.TabIndex = 2;
            btnCancel.Text = "Отмена";
            btnCancel.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(richTextBoxDescription, 0, 0);
            tableLayoutPanel1.Controls.Add(flowLayoutPanel1, 0, 1);
            tableLayoutPanel1.Location = new Point(6, 18);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.Padding = new Padding(4, 0, 4, 4);
            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            tableLayoutPanel1.Size = new Size(851, 358);
            tableLayoutPanel1.TabIndex = 3;
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            flowLayoutPanel1.Controls.Add(btnSave);
            flowLayoutPanel1.Controls.Add(panel1);
            flowLayoutPanel1.Controls.Add(btnCancel);
            flowLayoutPanel1.Location = new Point(569, 317);
            flowLayoutPanel1.Margin = new Padding(0, 3, 0, 3);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new Size(278, 34);
            flowLayoutPanel1.TabIndex = 1;
            // 
            // panel1
            // 
            panel1.Controls.Add(groupBox2);
            panel1.Location = new Point(97, 3);
            panel1.Name = "panel1";
            panel1.Size = new Size(84, 27);
            panel1.TabIndex = 5;
            // 
            // groupBox2
            // 
            groupBox2.Location = new Point(3, 4);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(78, 13);
            groupBox2.TabIndex = 0;
            groupBox2.TabStop = false;
            // 
            // groupBox1
            // 
            groupBox1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            groupBox1.Controls.Add(tableLayoutPanel1);
            groupBox1.Location = new Point(14, 31);
            groupBox1.Margin = new Padding(5, 0, 5, 5);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(863, 379);
            groupBox1.TabIndex = 4;
            groupBox1.TabStop = false;
            // 
            // toolStrip
            // 
            toolStrip.GripStyle = ToolStripGripStyle.Hidden;
            toolStrip.Items.AddRange(new ToolStripItem[] { toolStripLabel1, fontComboBox, toolStripSeparator1, toolStripLabel2, sizeComboBox, toolStripSeparator2, toolStripLabel3, colorComboBox, toolStripSeparator3, boldButton, toolStripSeparator4, italicButton, toolStripSeparator5, underlineButton });
            toolStrip.Location = new Point(0, 0);
            toolStrip.Name = "toolStrip";
            toolStrip.Padding = new Padding(10, 8, 1, 0);
            toolStrip.Size = new Size(891, 32);
            toolStrip.TabIndex = 5;
            toolStrip.Text = "toolStrip1";
            // 
            // toolStripLabel1
            // 
            toolStripLabel1.Name = "toolStripLabel1";
            toolStripLabel1.Size = new Size(46, 21);
            toolStripLabel1.Text = "Шрифт";
            // 
            // fontComboBox
            // 
            fontComboBox.DropDownHeight = 200;
            fontComboBox.FlatStyle = FlatStyle.Standard;
            fontComboBox.IntegralHeight = false;
            fontComboBox.Name = "fontComboBox";
            fontComboBox.Size = new Size(140, 24);
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(6, 24);
            // 
            // toolStripLabel2
            // 
            toolStripLabel2.Name = "toolStripLabel2";
            toolStripLabel2.Size = new Size(47, 21);
            toolStripLabel2.Text = "Размер";
            // 
            // sizeComboBox
            // 
            sizeComboBox.AutoSize = false;
            sizeComboBox.FlatStyle = FlatStyle.Standard;
            sizeComboBox.Name = "sizeComboBox";
            sizeComboBox.Size = new Size(43, 23);
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(6, 24);
            // 
            // toolStripLabel3
            // 
            toolStripLabel3.Name = "toolStripLabel3";
            toolStripLabel3.Size = new Size(33, 21);
            toolStripLabel3.Text = "Цвет";
            // 
            // colorComboBox
            // 
            colorComboBox.AutoSize = false;
            colorComboBox.FlatStyle = FlatStyle.Standard;
            colorComboBox.Name = "colorComboBox";
            colorComboBox.Size = new Size(43, 23);
            // 
            // toolStripSeparator3
            // 
            toolStripSeparator3.Name = "toolStripSeparator3";
            toolStripSeparator3.Size = new Size(6, 24);
            // 
            // boldButton
            // 
            boldButton.AutoSize = false;
            boldButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
            boldButton.Font = new Font("Segoe UI Black", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 204);
            boldButton.Image = (Image)resources.GetObject("boldButton.Image");
            boldButton.ImageTransparentColor = Color.Magenta;
            boldButton.Margin = new Padding(2, 1, 0, 2);
            boldButton.Name = "boldButton";
            boldButton.Size = new Size(23, 21);
            boldButton.Text = "B";
            boldButton.ToolTipText = "Жирный";
            // 
            // toolStripSeparator4
            // 
            toolStripSeparator4.Name = "toolStripSeparator4";
            toolStripSeparator4.Size = new Size(6, 24);
            // 
            // italicButton
            // 
            italicButton.AutoSize = false;
            italicButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
            italicButton.Font = new Font("Courier New", 12F, FontStyle.Italic, GraphicsUnit.Point, 204);
            italicButton.Image = (Image)resources.GetObject("italicButton.Image");
            italicButton.ImageTransparentColor = Color.Magenta;
            italicButton.Margin = new Padding(1, 1, 1, 2);
            italicButton.Name = "italicButton";
            italicButton.Size = new Size(23, 21);
            italicButton.Text = "I";
            italicButton.ToolTipText = "Курсив";
            // 
            // toolStripSeparator5
            // 
            toolStripSeparator5.Name = "toolStripSeparator5";
            toolStripSeparator5.Size = new Size(6, 24);
            // 
            // underlineButton
            // 
            underlineButton.AutoSize = false;
            underlineButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
            underlineButton.Font = new Font("Tahoma", 9.75F, FontStyle.Underline, GraphicsUnit.Point, 204);
            underlineButton.Image = (Image)resources.GetObject("underlineButton.Image");
            underlineButton.ImageTransparentColor = Color.Magenta;
            underlineButton.Margin = new Padding(2, 1, 0, 2);
            underlineButton.Name = "underlineButton";
            underlineButton.Size = new Size(23, 21);
            underlineButton.Text = "U";
            underlineButton.ToolTipText = "Подчеркнутый";
            // 
            // EditDescriptionForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(891, 424);
            Controls.Add(toolStrip);
            Controls.Add(groupBox1);
            MinimizeBox = false;
            MinimumSize = new Size(500, 240);
            Name = "EditDescriptionForm";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "  Редактирование описания";
            tableLayoutPanel1.ResumeLayout(false);
            flowLayoutPanel1.ResumeLayout(false);
            panel1.ResumeLayout(false);
            groupBox1.ResumeLayout(false);
            toolStrip.ResumeLayout(false);
            toolStrip.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private RichTextBox richTextBoxDescription;
        private Button btnSave;
        private Button btnCancel;
        private TableLayoutPanel tableLayoutPanel1;
        private GroupBox groupBox1;
        private FlowLayoutPanel flowLayoutPanel1;
        private Panel panel1;
        private GroupBox groupBox2;
        private ToolStrip toolStrip;
        private ToolStripLabel toolStripLabel1;
        private ToolStripComboBox fontComboBox;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripLabel toolStripLabel2;
        private ToolStripComboBox sizeComboBox;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripLabel toolStripLabel3;
        private ToolStripComboBox colorComboBox;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripButton boldButton;
        private ToolStripSeparator toolStripSeparator4;
        private ToolStripButton italicButton;
        private ToolStripSeparator toolStripSeparator5;
        private ToolStripButton underlineButton;
    }
}