using ElectricityConsumerApp.Model;
using ElectricityConsumerApp.Service;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ElectricityConsumerApp
{
    /// <summary>
    /// Логика взаимодействия для AddEditTestimonyHistoryWindow.xaml
    /// </summary>
    public partial class AddEditTestimonyWindow : Window
    {
        public int TestimonyID { get; set; }

        public IEnumerable<int> ElectricMeterNumberChoices;
        public string ElectricMeterNumbers
        {
            get
            {
                return electricMeterNumberTextBox.Text;
            }
        }

        public AddEditTestimonyWindow()
        {
            InitializeComponent();
            ElectricMeterNumberChoices = ElectricMeterService.GetElectricMeterNumbers();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (TestimonyID > 0)
            {
                electricMeterNumberTextBox.IsReadOnly = true;
                Testimony testimony = TestimonyHistoryService.GetTestimony(TestimonyID);
                electricMeterNumberTextBox.Text = testimony.ElectricMeterNumber.ToString();
                valueTextBox.Text = testimony.Value.ToString();
            }
        }

        //Событие при нажатии на кнопку клавиатуры при нахождении в electricMeterNumberTextBox
        private void electricMeterNumberTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            string number = (sender as TextBox).Text;
            if (ValidationService.IsOnlyNumerics(number))
            {
                (sender as TextBox).BorderBrush = Brushes.AliceBlue;
            }
            {
                (sender as TextBox).BorderBrush = Brushes.Red;
            }
            bool found = false;
            var border = (resultStack.Parent as ScrollViewer).Parent as Border;
            var data = ElectricMeterNumberChoices;

            if (number.Length == 0)
            {
                border.Visibility = Visibility.Collapsed;
            }
            else
            {
                border.Visibility = Visibility.Visible;
            }

            // Очистка списка
            resultStack.Children.Clear();

            // Добавление результатов автоподбора
            foreach (var obj in data)
            {
                if (obj.ToString().ToLower().StartsWith(number.ToLower()))
                {
                    WindowHelper.AddItemToAutoComplete(obj.ToString(), electricMeterNumberTextBox, resultStack);
                    found = true;
                }
            }

            if (!found)
            {
                resultStack.Children.Add(new TextBlock() { Text = "Результатов не найдено." });
            }

            resultScroll.MaxHeight = ActualHeight - 100;
        }

        private void addEditTestimonyButton_Click(object sender, RoutedEventArgs e)
        {
            if (!Validation())
                return;

            Testimony testimony = new Testimony()
            {
                ID = TestimonyID,
                ElectricMeterNumber = Convert.ToInt32(electricMeterNumberTextBox.Text),
                Value = Convert.ToInt32(valueTextBox.Text)
            };

            if (testimony.ID == 0)
                TestimonyHistoryService.AddTestimony(testimony);
            else
                TestimonyHistoryService.UpdateTestimony(testimony);

            Close();
        }

        public bool Validation()
        {
            int electricMeterNumber = 0;
            int value = 0;
            bool valid = true;
            StringBuilder errorMessageBuilder = new StringBuilder();

            if (String.IsNullOrEmpty(electricMeterNumberTextBox.Text) || !Int32.TryParse(electricMeterNumberTextBox.Text, out electricMeterNumber))
            {
                errorMessageBuilder.AppendLine("Введите действительный номер счётчика.");
                valid = false;
            }

            if (!ElectricMeterService.CheckElectricMeterNumber(electricMeterNumber))
            {
                errorMessageBuilder.AppendLine("Данный номер счётчика не найден.");
                valid = false;
            }

            if (String.IsNullOrEmpty(valueTextBox.Text) || !int.TryParse(valueTextBox.Text, out value) || value < 0)
            {
                errorMessageBuilder.AppendLine("Введите действительное значение счётчика. Значение должно быть целым значением не меньше нуля.");
                valid = false;
            }

            int maxValue = TestimonyHistoryService.GetMaxValueByEectricMeterNumber(electricMeterNumber);

            if (maxValue > 0 && value <= maxValue)
            {
                errorMessageBuilder.AppendLine($"Новое значение счётчика не может быть равным или меньше текущего. На данный момент показание для счётчика: {electricMeterNumber} равно: {maxValue}.");
                valid = false;
            }

            if (!String.IsNullOrEmpty(errorMessageBuilder.ToString()))
            {
                MessageBox.Show(errorMessageBuilder.ToString());
            }

            return valid;
        }
    }
}
