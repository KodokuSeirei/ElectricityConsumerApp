using System;

namespace ElectricityConsumerApp.Model
{
    public class Address
    {
        public Address()
        {
            City = new City();
            Street = String.Empty;
            Home = String.Empty;
            Fiat = String.Empty;
        }

        public int ID { get; set; }
        public int CityID { get; set; }
        public City City { get; set; }
        public string Street { get; set; }
        public string Home { get; set; }
        public string Fiat { get; set; }




        //public string FullAddress { get { return City + ", " + Value; } private set { } }
    }
}
