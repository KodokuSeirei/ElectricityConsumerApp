using ElectricityConsumerApp.Model;
using ElectricityConsumerApp.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace ElectricityConsumerApp
{
    /// <summary>
    /// Логика взаимодействия для AddEditConsumerWindow.xaml
    /// </summary>
    public partial class AddEditConsumerWindow : Window
    {
        public int ConsumerID { get; set; }
        public string ElectricMeterNumbers
        {
            get
            {
                return electricMeterNumbersTextBox.Text.ToString();
            }
            set { }
        }

        public List<string> RegionNames { get; set; }
        public List<string> CityNames { get; set; }

        public AddEditConsumerWindow()
        {
            InitializeComponent();
            cityComboBox.Visibility = Visibility.Hidden;
            RegionNames = AddressService.GetAllRegionNames().ToList();
            regionComboBox.ItemsSource = RegionNames;
            regionComboBox.SelectedIndex = 0;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (ConsumerID > 0)
            {
                Consumer consumer = ConsumerService.GetConsumer(ConsumerID);
                lastNameTextBox.Text = consumer.LastName;
                firstNameTextBox.Text = consumer.FirstName;
                patronymicTextBox.Text = consumer.Patronymic;
                electricMeterNumbersTextBox.Text = consumer.ElectricMeterNumbers;
                if (consumer.AddressID > 0)
                    consumer.Address = AddressService.GetAddress(consumer.AddressID);
                regionComboBox.SelectedIndex = RegionNames.FindIndex(x => x == consumer.Address.City.RegionName);
                cityComboBox.SelectedIndex = CityNames.FindIndex(x => x == consumer.Address.City.Name);
                streetTextBox.Text = consumer.Address.Street;
                homeTextBox.Text = consumer.Address.Home;
                fiatTextBox.Text = consumer.Address.Fiat;
            }
        }

        //Событие нажатия на кнопку добавления/изменения потребителя
        private void addEditConsumerButton_Click(object sender, RoutedEventArgs e)
        {

            if (!Validation())
                return;

            Consumer consumer = new Consumer()
            {
                ID = ConsumerID,
                LastName = lastNameTextBox.Text,
                FirstName = firstNameTextBox.Text,
                Patronymic = patronymicTextBox.Text,
                Address = new Address()
                {
                    CityID = AddressService.GetCityIDByCityAndRegionNames(regionComboBox.Text, cityComboBox.Text),
                    Street = streetTextBox.Text,
                    Home = homeTextBox.Text,
                    Fiat = fiatTextBox.Text
                }
            };

            if (consumer.ID == 0)
                ConsumerService.AddConsumer(consumer, electricMeterNumbersTextBox.Text);
            else
                ConsumerService.UpdateConsumer(consumer, electricMeterNumbersTextBox.Text);

            Close();
        }

        //Событие нажатия на кнопку добавления/изменения электросчётчика
        private void addElectricMeterNumber_Click(object sender, RoutedEventArgs e)
        {
            AddElectricMeterNumberWindow addElectricMeterNumberWindow = new AddElectricMeterNumberWindow();
            addElectricMeterNumberWindow.Owner = this;
            addElectricMeterNumberWindow.ConsumerID = ConsumerID;

            if (addElectricMeterNumberWindow.ShowDialog() == true)
            {
                string addedNumber = addElectricMeterNumberWindow.ElectricMeterNumbers;
                if (ElectricMeterNumbers.Contains(' ' + addedNumber) || ElectricMeterNumbers.Contains(addedNumber + ';'))
                    MessageBox.Show("Данный номер счётчика уже добавлен в список");
                else
                {
                    if (electricMeterNumbersTextBox.Text.Length > 1)
                        electricMeterNumbersTextBox.Text += ';';
                    electricMeterNumbersTextBox.Text += ' ' + addedNumber;
                }
            }
        }

        public bool Validation()
        {
            bool valid = true;
            StringBuilder errorMessageBuilder = new StringBuilder();

            if (String.IsNullOrEmpty(lastNameTextBox.Text))
            {
                errorMessageBuilder.AppendLine("Поле \"Фамилия\" должно быть заполнено");
                valid = false;
            }

            if (!ValidationService.IsOnlyLetters(lastNameTextBox.Text))
            {
                errorMessageBuilder.AppendLine("Поле \"Фамилия\" должно состоять только из букв.");
                valid = false;
            }

            if (String.IsNullOrEmpty(firstNameTextBox.Text))
            {
                errorMessageBuilder.AppendLine("Поле \"Имя\" должно быть заполнено");
                valid = false;
            }

            if (!ValidationService.IsOnlyLetters(firstNameTextBox.Text))
            {
                errorMessageBuilder.AppendLine("Поле \"Имя\" должно состоять только из букв.");
                valid = false;
            }

            if (!String.IsNullOrEmpty(patronymicTextBox.Text) && !ValidationService.IsOnlyLetters(patronymicTextBox.Text))
            {
                errorMessageBuilder.AppendLine("Поле \"Отчество\" должно состоять только из букв.");
                patronymicTextBox.Text = Regex.Match(patronymicTextBox.Text, @"[[:word:]\D]").Value;
            }

            if (String.IsNullOrEmpty(streetTextBox.Text))
            {
                errorMessageBuilder.AppendLine("Поле \"Улица\" должно быть заполнено.");
                valid = false;
            }

            if (String.IsNullOrEmpty(homeTextBox.Text))
            {
                errorMessageBuilder.AppendLine("Поле \"Дом\" должно быть заполнено.");
                valid = false;
            }

            if (String.IsNullOrEmpty(fiatTextBox.Text))
            {
                errorMessageBuilder.AppendLine("Поле \"Квартира\" должно быть заполнено");
                valid = false;
            }

            if (String.IsNullOrEmpty(electricMeterNumbersTextBox.Text))
            {
                errorMessageBuilder.AppendLine("Укажите хотябы 1 номер электросчётчика.");
                valid = false;
            }

            if (!String.IsNullOrEmpty(errorMessageBuilder.ToString()))
            {
                MessageBox.Show(errorMessageBuilder.ToString());
            }

            return valid;
        }

        private void regionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CityNames = AddressService.GetCityNamesByRegionName(Convert.ToString(regionComboBox.SelectedValue)).ToList();
            cityComboBox.ItemsSource = CityNames;
            cityComboBox.SelectedIndex = 0;
            cityComboBox.Visibility = Visibility.Visible;
        }
    }
}
