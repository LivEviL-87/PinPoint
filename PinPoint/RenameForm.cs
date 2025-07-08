using System;
using System.Windows.Forms;

namespace PinPoint
{
    public partial class RenameForm : Form
    {
        public string NewName { get; private set; } = string.Empty;

        public RenameForm(string currentName)
        {
            InitializeComponent();
            textBoxRename.Text = currentName;

            btnApply.Click += (_, _) => ApplyRename();
            btnCancel.Click += (_, _) => CancelRename();

            // Подписываемся на событие изменения текста
            textBoxRename.TextChanged += TextBoxRename_TextChanged;
        }

        private void ApplyRename()
        {
            NewName = textBoxRename.Text.Trim();
            if (string.IsNullOrWhiteSpace(NewName)) return;

            DialogResult = DialogResult.OK;
            Close();
        }

        private void CancelRename()
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void TextBoxRename_TextChanged(object? sender, EventArgs e)
        {
            ValidateInput();
        }

        private void ValidateInput()
        {
            // Отключаем кнопку "Применить", если поле пустое
            btnApply.Enabled = !string.IsNullOrWhiteSpace(textBoxRename.Text);
        }

    }
}
