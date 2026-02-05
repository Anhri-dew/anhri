using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;

using System.IO;
using System.Globalization;

namespace MathCalculator_v2
{
    public partial class Calculator : Form
    {

        private readonly CultureInfo culture = CultureInfo.InvariantCulture;
        private bool isNewNumber = true;
        private double currentValue = 0;
        private string currentOperation = "";
        private bool isOperationPending = false;
        private bool isDarkTheme = false;
        private const int MaxSaves = 60;



        private Color lightFormColor = SystemColors.Control;
        private Color lightDisplayBackColor = SystemColors.Window;
        private Color lightDisplayTextColor = SystemColors.WindowText;
        private Color lightButtonBackColor = SystemColors.Control;
        private Color lightButtonTextColor = SystemColors.ControlText;

        
        private Color darkFormColor = Color.FromArgb(45, 45, 48);
        private Color darkDisplayBackColor = Color.FromArgb(30, 30, 30);
        private Color darkDisplayTextColor = Color.Lime;               
        private Color darkButtonBackColor = Color.FromArgb(63, 63, 70); 
        private Color darkButtonTextColor = Color.White;               
        public Calculator()
        {
            InitializeComponent();
            LoadThemeSetting();
            
        }

        
        private void NumberButton_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            string number = button.Text;

            if (txtDisplay.Text == "0" || isNewNumber)
            {
                txtDisplay.Text = number;
                isNewNumber = false;
            }
            else
            {
                txtDisplay.Text += number;
            }
        }

        
        private void OperationButton_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;

            string displayText = txtDisplay.Text.Replace(".", ",");

            if (txtDisplay.Text.EndsWith("."))
            {
                txtDisplay.Text = txtDisplay.Text.TrimEnd('.');
            }

            if (!isOperationPending)
            {
                Calculate();
            }


            currentOperation = button.Text;


            currentValue = double.Parse(txtDisplay.Text, culture);


            isNewNumber = true;
            isOperationPending = true;
        }

        
        private void btnEquals_Click(object sender, EventArgs e)
        {
            string displayText = txtDisplay.Text.Replace(".", ",");
            if (txtDisplay.Text.EndsWith("."))
            {
                txtDisplay.Text = txtDisplay.Text.TrimEnd('.');
            }

            Calculate();


            currentOperation = "";
            isNewNumber = true;
            isOperationPending = false;
        }

       
        private void btnClear_Click(object sender, EventArgs e)
        {

            txtDisplay.Text = "0";
            currentValue = 0;
            currentOperation = "";
            isNewNumber = true;
            isOperationPending = false;
        }


       
        private void btnClearEntry_Click(object sender, EventArgs e)
        {

            txtDisplay.Text = "0";
            isNewNumber = true;
        }


        
        private void btnDecimal_Click(object sender, EventArgs e)
        {

            if (isNewNumber)
            {
                txtDisplay.Text = "0.";
                isNewNumber = false;
                isOperationPending = false;
            }

            else if (!txtDisplay.Text.Contains("."))
            {
                txtDisplay.Text += ".";
            }

            else
            {

                System.Media.SystemSounds.Beep.Play();
            }
        }

       
        private void Calculate()
        {
            if (string.IsNullOrEmpty(currentOperation))
                return;

            try
            {
                if (txtDisplay.Text.EndsWith("."))
                {
                    txtDisplay.Text = txtDisplay.Text.TrimEnd('.');
                }

                string displayText = txtDisplay.Text.Replace(".", ",");
                double secondValue = double.Parse(txtDisplay.Text, culture);
                double result = 0;

                switch (currentOperation)
                {
                    case "+":
                        result = currentValue + secondValue;
                        break;
                    case "-":
                        result = currentValue - secondValue;
                        break;
                    case "*":
                        result = currentValue * secondValue;
                        break;
                    case "/":
                        if (secondValue == 0)
                        {
                            throw new DivideByZeroException();
                        }
                        result = currentValue / secondValue;
                        break;
                }

                
                txtDisplay.Text = result.ToString(CultureInfo.InvariantCulture);

                currentValue = result;
            }
            catch (DivideByZeroException)
            {
                MessageBox.Show("Ошибка: деление на ноль!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtDisplay.Text = "0";
            }
            catch (FormatException)
            {
                MessageBox.Show("Ошибка формата числа!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtDisplay.Text = "0";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtDisplay.Text = "0";
            }

            isNewNumber = true;
        }



        
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                
                string currentValue = txtDisplay.Text;

              
                DialogResult result = MessageBox.Show(
                    $"Сохранить значение: {currentValue}\n\nДобавить подпись к сохранению?",
                    "Сохранение",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Cancel)
                {
                    return; 
                }

                string label = "";

                if (result == DialogResult.Yes)
                {
                    
                    label = AskForLabel(currentValue);
                    if (label == null) return; 
                }

                
                SaveValueWithLabel(currentValue, label);

               
                string message = $"Сохранено: {currentValue}";
                if (!string.IsNullOrEmpty(label))
                {
                    message += $"\nПодпись: {label}";
                }

               
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private string AskForLabel(string value)
        {
           
            Form labelForm = new Form();
            labelForm.Text = "Добавить подпись";
            labelForm.Size = new Size(350, 200);
            labelForm.StartPosition = FormStartPosition.CenterParent;
            labelForm.FormBorderStyle = FormBorderStyle.FixedDialog;
            labelForm.MaximizeBox = false;
            labelForm.MinimizeBox = false;

           
            Label lblQuestion = new Label();
            lblQuestion.Text = $"Введите подпись для значения: {value}";
            lblQuestion.Location = new Point(20, 20);
            lblQuestion.Size = new Size(300, 40);
            lblQuestion.Font = new Font("Arial", 10);

            
            TextBox txtLabel = new TextBox();
            txtLabel.Location = new Point(20, 70);
            txtLabel.Size = new Size(290, 25);
            txtLabel.Font = new Font("Arial", 10);
            txtLabel.MaxLength = 50; 

            
            Panel buttonPanel = new Panel();
            buttonPanel.Dock = DockStyle.Bottom;
            buttonPanel.Height = 45;
            buttonPanel.Padding = new Padding(5);

           
            Button btnOK = new Button();
            btnOK.Text = "OK";
            btnOK.Size = new Size(80, 30);
            btnOK.Location = new Point(160, 5);
            btnOK.DialogResult = DialogResult.OK;

            
            Button btnCancel = new Button();
            btnCancel.Text = "Отмена";
            btnCancel.Size = new Size(80, 30);
            btnCancel.Location = new Point(245, 5);
            btnCancel.DialogResult = DialogResult.Cancel;

           
            buttonPanel.Controls.Add(btnOK);
            buttonPanel.Controls.Add(btnCancel);

            
            labelForm.Controls.Add(lblQuestion);
            labelForm.Controls.Add(txtLabel);
            labelForm.Controls.Add(buttonPanel);

           
            txtLabel.Focus();

            
            if (labelForm.ShowDialog() == DialogResult.OK)
            {
                
                return txtLabel.Text.Trim();
            }

            return null; 
        }

        private void SaveValueWithLabel(string value, string label)
        {
            List<string> allSaves = new List<string>();

            if (File.Exists("calculator_saves.txt"))
            {
                allSaves = File.ReadAllLines("calculator_saves.txt").ToList();
            }

            
            string timestamp = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");

            
            string newEntry;
            if (string.IsNullOrEmpty(label))
            {
                newEntry = $"{timestamp} = {value}";
            }
            else
            {
                newEntry = $"{timestamp} = {value} : {label}";
            }

           
            allSaves.Insert(0, newEntry);

            
            if (allSaves.Count > MaxSaves)
            {
                allSaves = allSaves.Take(MaxSaves).ToList();
            }

            
            File.WriteAllLines("calculator_saves.txt", allSaves);
        }






        private void SaveValueToFile(string value)
        {
           
            List<string> allSaves = new List<string>();

            
            if (File.Exists("calculator_saves.txt"))
            {
                
                string[] existingSaves = File.ReadAllLines("calculator_saves.txt");
                allSaves.AddRange(existingSaves);
            }

           
            string timestamp = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");
            string newEntry = $"{timestamp} = {value}";

            
            allSaves.Insert(0, newEntry);

           
            if (allSaves.Count > 20)
            {
                allSaves = allSaves.Take(20).ToList();
            }

            
            File.WriteAllLines("calculator_saves.txt", allSaves);
        }



        
        private void btnLoad_Click(object sender, EventArgs e)
        {
            try
            {
                if (!File.Exists("calculator_saves.txt"))
                {
                    MessageBox.Show("Нет сохраненных значений!\nСначала что-нибудь сохраните.", "Информация",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                List<string> allSaves = File.ReadAllLines("calculator_saves.txt").ToList();

                if (allSaves.Count == 0)
                {
                    MessageBox.Show("Файл сохранений пуст!\nСначала что-нибудь сохраните.", "Информация",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

               
                ShowSaveSelectionDialogWithLabels(allSaves);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ShowSaveSelectionDialogWithLabels(List<string> saves)
        {
            
            Form selectForm = new Form();
            selectForm.Text = "Выберите сохранение (всего: " + saves.Count + ")";
            selectForm.Size = new Size(500, 400); 
            selectForm.StartPosition = FormStartPosition.CenterParent;
            selectForm.FormBorderStyle = FormBorderStyle.FixedDialog;
            selectForm.MaximizeBox = false;
            selectForm.MinimizeBox = false;

            
            ListView listView = new ListView();
            listView.Dock = DockStyle.Fill;
            listView.View = View.Details; 
            listView.FullRowSelect = true; 
            listView.GridLines = true; 
            listView.Font = new Font("Arial", 10);

            
            listView.Columns.Add("Дата и время", 150);
            listView.Columns.Add("Значение", 100);
            listView.Columns.Add("Подпись", 200);

            
            foreach (string save in saves)
            {
                
                SaveEntry parsedEntry = ParseSaveEntry(save);

                
                ListViewItem item = new ListViewItem(parsedEntry.Timestamp);
                item.SubItems.Add(parsedEntry.Value);
                item.SubItems.Add(parsedEntry.Label);
                item.Tag = parsedEntry; 

                listView.Items.Add(item);
            }

            
            Panel buttonPanel = new Panel();
            buttonPanel.Dock = DockStyle.Bottom;
            buttonPanel.Height = 45;
            buttonPanel.Padding = new Padding(5);

            
            Button btnSelect = new Button();
            btnSelect.Text = "Выбрать";
            btnSelect.Size = new Size(80, 30);
            btnSelect.Location = new Point(320, 5);
            btnSelect.Click += (s, args) =>
            {
                if (listView.SelectedItems.Count > 0)
                {
                    selectForm.DialogResult = DialogResult.OK;
                }
            };

           
            Button btnSearch = new Button();
            btnSearch.Text = "Поиск";
            btnSearch.Size = new Size(80, 30);
            btnSearch.Location = new Point(5, 5);
            btnSearch.Click += (s, args) =>
            {
                SearchInSaves(listView);
            };

            
            Button btnCancel = new Button();
            btnCancel.Text = "Отмена";
            btnCancel.Size = new Size(80, 30);
            btnCancel.Location = new Point(405, 5);
            btnCancel.DialogResult = DialogResult.Cancel;

            
            Button btnDelete = new Button();
            btnDelete.Text = "Удалить";
            btnDelete.Size = new Size(80, 30);
            btnDelete.Location = new Point(235, 5);
            btnDelete.Click += (s, args) =>
            {
                if (listView.SelectedItems.Count > 0 &&
                    MessageBox.Show("Удалить выбранное сохранение?", "Подтверждение",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    listView.SelectedItems[0].Remove();
                    
                    UpdateSaveFile(listView);
                }
            };

            
            Button btnEdit = new Button();
            btnEdit.Text = "Изм. подпись";
            btnEdit.Size = new Size(100, 30);
            btnEdit.Location = new Point(120, 5);
            btnEdit.Click += (s, args) =>
            {
                if (listView.SelectedItems.Count > 0)
                {
                    EditSaveLabel(listView);
                }
            };

            
            buttonPanel.Controls.Add(btnSearch);
            buttonPanel.Controls.Add(btnEdit);
            buttonPanel.Controls.Add(btnDelete);
            buttonPanel.Controls.Add(btnSelect);
            buttonPanel.Controls.Add(btnCancel);

            
            listView.DoubleClick += (s, args) =>
            {
                if (listView.SelectedItems.Count > 0)
                {
                    selectForm.DialogResult = DialogResult.OK;
                    selectForm.Close();
                }
            };

            
            selectForm.Controls.Add(listView);
            selectForm.Controls.Add(buttonPanel);

            
            if (selectForm.ShowDialog() == DialogResult.OK && listView.SelectedItems.Count > 0)
            {
                
                SaveEntry selectedEntry = (SaveEntry)listView.SelectedItems[0].Tag;

               
                double value = SafeParseDouble(selectedEntry.Value);
                txtDisplay.Text = value.ToString(CultureInfo.InvariantCulture);
                isNewNumber = true;

                
                string message = $"Загружено: {value}";
                if (!string.IsNullOrEmpty(selectedEntry.Label))
                {
                    message += $"\nПодпись: {selectedEntry.Label}";
                }

                
            }
        }



        private class SaveEntry
        {
            public string Timestamp { get; set; }
            public string Value { get; set; }
            public string Label { get; set; }

            public override string ToString()
            {
                if (string.IsNullOrEmpty(Label))
                    return $"{Timestamp} = {Value}";
                else
                    return $"{Timestamp} = {Value} : {Label}";
            }
        }

        
        private SaveEntry ParseSaveEntry(string saveLine)
        {
            SaveEntry entry = new SaveEntry();

           
            int equalsIndex = saveLine.IndexOf(" = ");
            if (equalsIndex > 0)
            {
                entry.Timestamp = saveLine.Substring(0, equalsIndex).Trim();

                string rest = saveLine.Substring(equalsIndex + 3);

                
                int colonIndex = rest.IndexOf(" : ");
                if (colonIndex > 0)
                {
                    entry.Value = rest.Substring(0, colonIndex).Trim();
                    entry.Label = rest.Substring(colonIndex + 3).Trim();
                }
                else
                {
                    entry.Value = rest.Trim();
                    entry.Label = "";
                }
            }

            return entry;
        }

        
        private void SearchInSaves(ListView listView)
        {
            
            Form searchForm = new Form();
            searchForm.Text = "Поиск по подписям";
            searchForm.Size = new Size(300, 150);
            searchForm.StartPosition = FormStartPosition.CenterParent;

            
            TextBox txtSearch = new TextBox();
            txtSearch.Location = new Point(20, 20);
            txtSearch.Size = new Size(240, 25);
            txtSearch.Font = new Font("Arial", 10);
            

          
            Button btnSearch = new Button();
            btnSearch.Text = "Найти";
            btnSearch.Size = new Size(80, 30);
            btnSearch.Location = new Point(110, 60);
            btnSearch.DialogResult = DialogResult.OK;

            
            searchForm.Controls.Add(txtSearch);
            searchForm.Controls.Add(btnSearch);

            if (searchForm.ShowDialog() == DialogResult.OK)
            {
                string searchText = txtSearch.Text.ToLower();

                
                foreach (ListViewItem item in listView.Items)
                {
                    SaveEntry entry = (SaveEntry)item.Tag;

                    if (entry.Value.ToLower().Contains(searchText) ||
                        entry.Label.ToLower().Contains(searchText))
                    {
                        item.BackColor = Color.Yellow;
                        item.Selected = true;
                        item.EnsureVisible();
                    }
                    else
                    {
                        item.BackColor = Color.White;
                    }
                }



            }
        }




        

        
        private void EditSaveLabel(ListView listView)
        {
            ListViewItem selectedItem = listView.SelectedItems[0];
            SaveEntry entry = (SaveEntry)selectedItem.Tag;

            string newLabel = AskForLabel(entry.Value);
            if (newLabel != null)
            {
                entry.Label = newLabel;
                selectedItem.SubItems[2].Text = newLabel;

                
                UpdateSaveFile(listView);
            }
        }

        
        private void UpdateSaveFile(ListView listView)
        {
            List<string> saves = new List<string>();

            foreach (ListViewItem item in listView.Items)
            {
                SaveEntry entry = (SaveEntry)item.Tag;
                saves.Add(entry.ToString());
            }

            File.WriteAllLines("calculator_saves.txt", saves);
        }




        
        private double SafeParseDouble(string input)
        {
            
            input = input.Trim();

            
            if (string.IsNullOrEmpty(input))
                return 0;

            double result = 0;

            
            if (double.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
            {
                return result;
            }

            
            string withComma = input.Replace(".", ",");
            if (double.TryParse(withComma, out result))
            {
                return result;
            }

            
            string withDot = input.Replace(",", ".");
            if (double.TryParse(withDot, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
            {
                return result;
            }

            
            return 0;
        }

        
        private void ShowSaveSelectionDialog(List<string> saves)
        {
            
            Form selectForm = new Form();
            selectForm.Text = "Выберите сохраненное значение (всего: " + saves.Count + "). Для выбора значение двоеное нажатие ЛКМ";
            selectForm.Size = new Size(550, 350);
            selectForm.StartPosition = FormStartPosition.CenterParent;
            selectForm.FormBorderStyle = FormBorderStyle.FixedDialog;
            selectForm.MaximizeBox = false;
            selectForm.MinimizeBox = false;

            
            ListBox listBox = new ListBox();
            listBox.Dock = DockStyle.Fill;
            listBox.Font = new Font("Consolas", 10);

            
            foreach (string save in saves)
            {
                listBox.Items.Add(save);
            }

            
            if (listBox.Items.Count > 0)
            {
                listBox.SelectedIndex = 0;
            }

            
            Panel buttonPanel = new Panel();
            buttonPanel.Dock = DockStyle.Bottom;
            buttonPanel.Height = 45;
            buttonPanel.Padding = new Padding(5);

            
            Button btnSelect = new Button();
            btnSelect.Text = "Выбрать";
            btnSelect.Size = new Size(80, 30);
            btnSelect.Location = new Point(260, 5);
            btnSelect.Click += (s, args) =>
            {
                if (listBox.SelectedItem != null)
                {
                    selectForm.DialogResult = DialogResult.OK;
                }
            };

            
            Button btnCancel = new Button();
            btnCancel.Text = "Отмена";
            btnCancel.Size = new Size(80, 30);
            btnCancel.Location = new Point(345, 5);
            btnCancel.Click += (s, args) =>
            {
                selectForm.DialogResult = DialogResult.Cancel;
                selectForm.Close();
            };

            
            Button btnViewFile = new Button();
            btnViewFile.Text = "Открыть файл";
            btnViewFile.Size = new Size(100, 30);
            btnViewFile.Location = new Point(5, 5);
            btnViewFile.Click += (s, args) =>
            {
                try
                {
                    System.Diagnostics.Process.Start("notepad.exe", "calculator_saves.txt");
                }
                catch
                {
                    MessageBox.Show("Не удалось открыть файл", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            
            buttonPanel.Controls.Add(btnViewFile);
            buttonPanel.Controls.Add(btnSelect);
            buttonPanel.Controls.Add(btnCancel);

           
            listBox.DoubleClick += (s, args) =>
            {
                if (listBox.SelectedItem != null)
                {
                    selectForm.DialogResult = DialogResult.OK;
                    selectForm.Close();
                }
            };

            
            selectForm.Controls.Add(listBox);
            selectForm.Controls.Add(buttonPanel);

            
            if (selectForm.ShowDialog() == DialogResult.OK && listBox.SelectedItem != null)
            {
                string selectedSave = listBox.SelectedItem.ToString();

                
                int equalsIndex = selectedSave.LastIndexOf(" = ");
                if (equalsIndex > 0)
                {
                    string numberStr = selectedSave.Substring(equalsIndex + 3);

                    
                    double value = SafeParseDouble(numberStr);

                    
                    if (Math.Abs(value) < 0.00000001 &&
                        numberStr != "0" &&
                        !numberStr.StartsWith("0.") &&
                        !numberStr.StartsWith("0,"))
                    {
                        
                        MessageBox.Show($"Внимание: '{numberStr}' распознано как {value}\nВставляю оригинальный текст",
                            "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        txtDisplay.Text = numberStr;
                    }
                    else
                    {
                        
                        txtDisplay.Text = value.ToString(CultureInfo.InvariantCulture);
                    }

                    isNewNumber = true;

                    MessageBox.Show($"Загружено: {txtDisplay.Text}", "Успех",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }

        }


        private void ApplyTheme()
        {
            if (isDarkTheme)
            {
                ApplyDarkTheme();
            }
            else
            {
                ApplyLightTheme();
            }
        }

        
        private void ApplyDarkTheme()
        {
            
            this.BackColor = darkFormColor;

            
            txtDisplay.BackColor = darkDisplayBackColor;
            txtDisplay.ForeColor = darkDisplayTextColor;

            
            foreach (Control control in this.Controls)
            {
                if (control is Button button)
                {
                    
                    button.BackColor = darkButtonBackColor;
                    button.ForeColor = darkButtonTextColor;

                    
                    ApplySpecialButtonColors(button);
                }
            }
        }

        
        private void ApplyLightTheme()
        {
            
            this.BackColor = lightFormColor;

            
            txtDisplay.BackColor = lightDisplayBackColor;
            txtDisplay.ForeColor = lightDisplayTextColor;

            
            foreach (Control control in this.Controls)
            {
                if (control is Button button)
                {
                    button.BackColor = lightButtonBackColor;
                    button.ForeColor = lightButtonTextColor;

                    
                    ResetSpecialButtonColors(button);
                }
            }
        }

        
        private void ApplySpecialButtonColors(Button button)
        {
           
            switch (button.Name)
            {
                case "btnEquals":    
                    button.BackColor = Color.DarkBlue;
                    button.ForeColor = Color.White;
                    break;

                case "btnClear":     
                    button.BackColor = Color.DarkRed;
                    button.ForeColor = Color.White;
                    break;

                case "btnClearEntry": 
                    button.BackColor = Color.DarkOrange;
                    button.ForeColor = Color.Black;
                    break;

                case "btnSave":      
                    button.BackColor = Color.DarkGreen;
                    button.ForeColor = Color.White;
                    break;

                case "btnLoad":      
                    button.BackColor = Color.DarkCyan;
                    button.ForeColor = Color.White;
                    break;

                case "btnTheme":     
                    button.BackColor = Color.DarkGoldenrod;
                    button.ForeColor = Color.White;
                    break;

                case "btnDivide":    
                case "btnMultiply":
                case "btnSubtract":
                case "btnAdd":
                    button.BackColor = Color.FromArgb(80, 80, 90); 
                    button.ForeColor = Color.White;
                    break;
            }
        }

        
        private void ResetSpecialButtonColors(Button button)
        {
            
            switch (button.Name)
            {
                case "btnEquals":    
                    button.BackColor = Color.LightBlue;
                    button.ForeColor = Color.Black;
                    break;

                case "btnClear":     
                    button.BackColor = Color.LightCoral;
                    button.ForeColor = Color.Black;
                    break;

                case "btnClearEntry": 
                    button.BackColor = Color.LightYellow;
                    button.ForeColor = Color.Black;
                    break;

                case "btnSave":      
                    button.BackColor = Color.LightGreen;
                    button.ForeColor = Color.Black;
                    break;

                case "btnLoad":     
                    button.BackColor = Color.LightBlue;
                    button.ForeColor = Color.Black;
                    break;

                case "btnTheme":    
                    button.BackColor = Color.LightGoldenrodYellow;
                    button.ForeColor = Color.Black;
                    break;
            }
        }

        private void btnTheme_Click(object sender, EventArgs e)
        {
            try
            {
                
                isDarkTheme = !isDarkTheme;

                
                ApplyTheme();

               
                SaveThemeSetting();

                
                string themeName = isDarkTheme ? "Темная" : "Светлая";
                MessageBox.Show($"Тема изменена на: {themeName}", "Тема",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при смене темы: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SaveThemeSetting()
        {
            try
            {
                
                string themeSetting = isDarkTheme.ToString();
                File.WriteAllText("calculator_theme.txt", themeSetting);
            }
            catch
            {
                
            }
        }

        
        private void LoadThemeSetting()
        {
            try
            {
                if (File.Exists("calculator_theme.txt"))
                {
                    string themeSetting = File.ReadAllText("calculator_theme.txt");

                    if (bool.TryParse(themeSetting, out bool savedTheme))
                    {
                        isDarkTheme = savedTheme;
                        ApplyTheme();
                    }
                }
            }
            catch
            {
                
            }
        }
    }
}

        


































    

