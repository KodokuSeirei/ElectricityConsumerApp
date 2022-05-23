using ElectricityConsumerApp.Model;
using ElectricityConsumerApp.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace ElectricityConsumerApp
{
    /// <summary>
    /// Логика взаимодействия для AddEditElectricMeterWindow.xaml
    /// </summary>
    public partial class AddEditElectricMeterWindow : Window
    {
        public int ElectricMeterNumber { get; set; }
        public List<ElectricMeterType> ElectricMeterTypes { get; set; }
        public List<string> ElectricMeterTypesNames { get; set; } = new List<string>();

        public AddEditElectricMeterWindow()
        {
            InitializeComponent();
            ElectricMeterTypes = ElectricMeterService.GetElectricMeterTypes();
            foreach (ElectricMeterType electricMeterType in ElectricMeterTypes)
            {
                ElectricMeterTypesNames.Add(electricMeterType.Name);
            }
            typeComboBox.ItemsSource = ElectricMeterTypesNames;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (ElectricMeterNumber > 0)
            {
                ElectricMeter electricMeter = ElectricMeterService.GetElectricMeter(ElectricMeterNumber);
                numberTextBox.Text = electricMeter.Number.ToString();
                typeComboBox.SelectedIndex = ElectricMeterTypesNames.IndexOf(electricMeter.Type);
                dateAcceptanceDatePicker.SelectedDate = electricMeter.DateAcceptance;
                stateVerificationPeriodTextBox.Text = electricMeter.StateVerificationPeriod.ToString();
            }
        }

        //Событие добаления электросчётчика
        private void addEditElectricMeterButton_Click(object sender, RoutedEventArgs e)
        {

            if (!Validation())
                return;

            int typeID = ElectricMeterTypes.Where(x => x.Name == typeComboBox.Text).First().ID;
            ElectricMeter electricMeter = new ElectricMeter()
            {
                Number = ElectricMeterNumber,
                TypeID = typeID,
                DateAcceptance = dateAcceptanceDatePicker.SelectedDate.Value,
                StateVerificationPeriod = Convert.ToInt32(stateVerificationPeriodTextBox.Text)
            };

            if (electricMeter.Number == 0)
            {
                int number = 0;
                if (!Int32.TryParse(numberTextBox.Text, out number) || number == 0)
                {
                    MessageBox.Show("Введён некорректный номер счётчика");
                    return;
                }
                electricMeter.Number = number;
                ElectricMeterService.AddElectricMeter(electricMeter);
            }
            else
                ElectricMeterService.UpdateElectricMeter(electricMeter);

            Close();
        }

        public bool Validation()
        {
            bool valid = true;
            StringBuilder errorMessageBuilder = new StringBuilder();

            if (String.IsNullOrEmpty(numberTextBox.Text))
            {
                errorMessageBuilder.AppendLine("Поле \"Номер\" должно быть заполнено");
                valid = false;
            }

            if (!ValidationService.IsOnlyNumerics(numberTextBox.Text))
            {
                errorMessageBuilder.AppendLine("Номер электросчётчика должен состоять только из цифр.");
                valid = false;
            }

            if (typeComboBox.SelectedValue == null)
            {
                errorMessageBuilder.AppendLine("Укажите тип электросчётчика.");
                valid = false;
            }

            if (String.IsNullOrEmpty(dateAcceptanceDatePicker.Text))
            {
                errorMessageBuilder.AppendLine("Поле \"Дата приёмки\" должно быть заполнено.");
                valid = false;
            }

            if (dateAcceptanceDatePicker.SelectedDate == null || !ValidationService.IsPastDate(dateAcceptanceDatePicker.SelectedDate.Value))
            {
                errorMessageBuilder.AppendLine("Поле \"Дата приёмки\" должно быть меньше или равной текущей дате.");
                valid = false;
            }

            if (String.IsNullOrEmpty(stateVerificationPeriodTextBox.Text))
            {
                errorMessageBuilder.AppendLine("Поле \"Срок госповерки (лет)\" должно быть заполнено.");
                valid = false;
            }

            if (!ValidationService.IsOnlyNumerics(stateVerificationPeriodTextBox.Text))
            {
                errorMessageBuilder.AppendLine("Поле \"Срок госповерки (лет)\" должно состоять только из цифр.");
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
