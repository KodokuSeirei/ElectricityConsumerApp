using System;

namespace ElectricityConsumerApp.Model
{
    public class ElectricMeterType
    {
        public ElectricMeterType()
        {
            Name = String.Empty;
        }

        public int ID { get; set; }
        public string Name { get; set; }
    }
}
