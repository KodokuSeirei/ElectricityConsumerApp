using System;

namespace ElectricityConsumerApp.Model
{
    public class Consumer
    {
        public Consumer()
        {
            LastName = String.Empty;
            FirstName = String.Empty;
            Patronymic = String.Empty;
            Address = new Address();
            ElectricMeterNumbers = String.Empty;
        }
        public int ID { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Patronymic { get; set; }
        public string FullName
        {
            get
            {
                return LastName.Trim() + " " + FirstName.Trim() + " " + Patronymic.Trim();
            }
        }
        public string ElectricMeterNumbers { get; set; }
        public int AddressID { get; set; }
        public Address Address { get; set; }
    }
}
