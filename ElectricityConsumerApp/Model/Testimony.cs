using System;

namespace ElectricityConsumerApp.Model
{
    public class Testimony
    {
        public int ID { get; set; }
        public int ElectricMeterNumber { get; set; }
        public int Value { get; set; }
        public DateTime Date { get; set; }
    }
}
