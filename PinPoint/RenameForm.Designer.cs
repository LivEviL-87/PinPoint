namespace PinPoint
{
    partial class RenameForm
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
            textBoxRename = new TextBox();
            btnApply = new Button();
            btnCancel = new Button();
            groupBox1 = new GroupBox();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // textBoxRename
            // 
            textBoxRename.BorderStyle = BorderStyle.FixedSingle;
            textBoxRename.Location = new Point(15, 22);
            textBoxRename.Name = "textBoxRename";
            textBoxRename.Size = new Size(280, 23);
            textBoxRename.TabIndex = 0;
            // 
            // btnApply
            // 
            btnApply.Location = new Point(15, 53);
            btnApply.Margin = new Padding(5);
            btnApply.Name = "btnApply";
            btnApply.Size = new Size(84, 23);
            btnApply.TabIndex = 1;
            btnApply.Text = "Применить";
            btnApply.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            btnCancel.Location = new Point(211, 53);
            btnCancel.Margin = new Padding(5);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(84, 23);
            btnCancel.TabIndex = 2;
            btnCancel.Text = "Отмена";
            btnCancel.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(textBoxRename);
            groupBox1.Controls.Add(btnCancel);
            groupBox1.Controls.Add(btnApply);
            groupBox1.Location = new Point(12, 7);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(310, 88);
            groupBox1.TabIndex = 3;
            groupBox1.TabStop = false;
            groupBox1.Text = "Введите новое имя";
            // 
            // RenameForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(334, 106);
            ControlBox = false;
            Controls.Add(groupBox1);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "RenameForm";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "  Переименовать";
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TextBox textBoxRename;
        private Button btnApply;
        private Button btnCancel;
        private GroupBox groupBox1;
    }
}