using System;

namespace ElectricityConsumerApp.Model
{
    public class ElectricMeter
    {
        public ElectricMeter()
        {
            Type = String.Empty;
        }

        public DateTime DateAcceptance { get; set; }
        public int Number { get; set; }
        public string Type { get; set; }
        //public string DateAcceptanceString { 
        //    get
        //    {
        //        return DateAcceptance.ToString("d");
        //    } 
        //    set { } 
        //}
        public int StateVerificationPeriod { get; set; }
        public int TypeID { get; set; }
    }
}
