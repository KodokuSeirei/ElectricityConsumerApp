using System.Collections.Generic;
using System.Data;

namespace ElectricityConsumerApp.Service
{
    public class DataHelper
    {
        public static IEnumerable<object> DataSetReaderOneColumn(DataSet dataSet, string columnName)
        {
            List<object> objects = new List<object>();
            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                objects.Add(row[columnName]);
            }
            return objects;
        }
    }
}
