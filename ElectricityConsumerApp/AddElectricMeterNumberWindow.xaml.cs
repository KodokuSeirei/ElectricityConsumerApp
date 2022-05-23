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
    /// Логика взаимодействия для AddElectricMeterNumberWindow.xaml
    /// </summary>
    public partial class AddElectricMeterNumberWindow : Window
    {
        public IEnumerable<int> ElectricMeterNumberChoices;
        public string ElectricMeterNumbers
        {
            get
            {
                return electricMeterNumberTextBox.Text;
            }
        }
        public int ConsumerID { get; set; } = 0;


        public AddElectricMeterNumberWindow()
        {
            InitializeComponent();
            ElectricMeterNumberChoices = ElectricMeterService.GetElectricMeterNumbers();
        }

        private void addElectricMeterNumber_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
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

        public bool Validation()
        {
            int number = 0;
            bool valid = true;
            StringBuilder errorMessageBuilder = new StringBuilder();

            if (String.IsNullOrEmpty(electricMeterNumberTextBox.Text) || !Int32.TryParse(electricMeterNumberTextBox.Text, out number))
            {
                errorMessageBuilder.AppendLine("Введите действительный номер счётчика.");
                valid = false;
            }

            if (ElectricMeterService.CheckElectricMeterNumber(number))
            {
                errorMessageBuilder.AppendLine("Данный номер счётчика не найден.");
                valid = false;
            }

            if (ConsumerService.CheckConsumerElectricMeter(ConsumerID, number))
            {
                errorMessageBuilder.AppendLine("Данный номер счётчика уже привязан к данному потребителю.");
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
