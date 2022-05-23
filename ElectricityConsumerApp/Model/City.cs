using System;

namespace ElectricityConsumerApp.Model
{
    public class City
    {
        public City()
        {
            Name = String.Empty;
            RegionName = String.Empty;
            OkrugName = String.Empty;
        }

        public int ID { get; set; }
        public string Name { get; set; }
        public int RegionID { get; set; }
        public string RegionName { get; set; }
        public int OkrugID { get; set; }
        public string OkrugName { get; set; }
    }
}
