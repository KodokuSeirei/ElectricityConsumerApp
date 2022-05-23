using ElectricityConsumerApp.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;

namespace ElectricityConsumerApp.Service
{
    internal class ElectricMeterService
    {
        public static IEnumerable<int> GetElectricMeterNumbers()
        {
            IEnumerable<int> numbers = new List<int>();
            try
            {
                string query = @"SELECT [Number] FROM [ElectricMeter]";
                IEnumerable<object> objects = DataHelper.DataSetReaderOneColumn(DBService.ExecuteDataSet(query), "Number");
                numbers = objects.Select(x => (int)x);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"При получении номеров электросчётчиков произошла ошибка \"{ex.Message}\"");
            }
            return numbers;
        }

        public static List<ElectricMeterType> GetElectricMeterTypes()
        {
            List<ElectricMeterType> types = new List<ElectricMeterType>();
            try
            {
                string query = @"SELECT * FROM [ElectricMeterType]";
                var typesTable = DBService.ExecuteDataSet(query).Tables[0];
                foreach (var typeRow in typesTable.Rows)
                {
                    var type = (DataRow)typeRow;
                    types.Add(new ElectricMeterType { ID = (int)type["ID"], Name = type["Name"].ToString() });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"При получении номеров электросчётчиков произошла ошибка \"{ex.Message}\"");
            }
            return types;
        }

        public static DataTable GetAllElectricMetersTable()
        {
            string query = @"SELECT [ElectricMeter].[Number], [ElectricMeterType].[Name] AS [Type], [DateAcceptance], [StateVerificationPeriod] FROM [ElectricMeter]
            LEFT JOIN [ElectricMeterType] ON [ElectricMeter].[TypeID] = [ElectricMeterType].[ID]";

            DataSet dataSet = DBService.ExecuteDataSet(query);

            return dataSet.Tables[0];
        }

        public static ElectricMeter ElectricMeterReaderFromDataSet(DataSet dataSet)
        {
            ElectricMeter electricMeter = new ElectricMeter();
            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                electricMeter = new ElectricMeter()
                {
                    Number = Convert.ToInt32(row["Number"]),
                    Type = Convert.ToString(row["Type"] ?? String.Empty),
                    DateAcceptance = Convert.ToDateTime(row["DateAcceptance"]),
                    StateVerificationPeriod = Convert.ToInt32(row["StateVerificationPeriod"])
                };
            }
            return electricMeter;
        }

        public static ElectricMeter GetElectricMeter(int electricMeterNumber)
        {
            string query = $"SELECT [Number], [ElectricMeterType].[Name] AS [Type], [DateAcceptance], [StateVerificationPeriod] FROM [ElectricMeter]" +
                $"LEFT JOIN [ElectricMeterType] ON [ElectricMeter].[TypeID] = [ElectricMeterType].[ID] " +
                $"WHERE [Number] = {electricMeterNumber}";

            return ElectricMeterReaderFromDataSet(DBService.ExecuteDataSet(query));
        }

        public static void AddElectricMeter(ElectricMeter electricMeter)
        {
            string query = "[sp_CreateElectricMeter]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>() {
                new SqlParameter("@number", electricMeter.Number),
                new SqlParameter("@typeID", electricMeter.TypeID),
                new SqlParameter("@dateAcceptance", Convert.ToDateTime(electricMeter.DateAcceptance)),
                new SqlParameter("@stateVerificationPeriod", electricMeter.StateVerificationPeriod) };
            DBService.ExecuteNonQuery(query, CommandType.StoredProcedure, sqlParameters);
        }

        public static void UpdateElectricMeter(ElectricMeter electricMeter)
        {
            string query = "[sp_UpdateElectricMeter]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>() {
                new SqlParameter("@number", electricMeter.Number),
                new SqlParameter("@typeID", electricMeter.TypeID),
                new SqlParameter("@dateAcceptance", electricMeter.DateAcceptance),
                new SqlParameter("@stateVerificationPeriod", electricMeter.StateVerificationPeriod)};

            DBService.ExecuteNonQuery(query, CommandType.StoredProcedure, sqlParameters);
        }

        public static void DeleteElectricMeter(int electricMeterNumber)
        {
            string query = $"DELETE [ElectricMeter] WHERE [Number] = {electricMeterNumber}";

            DBService.ExecuteNonQuery(query);
        }

        public static bool CheckElectricMeterNumber(int electricMeterNumber)
        {
            string query = $"SELECT COUNT(*) FROM [ElectricMeter] WHERE [Number] = {electricMeterNumber}";
            int count = Convert.ToInt32(DBService.ExecuteScalar(query, CommandType.Text));
            return count > 0;
        }
        public static int GetAllElectricMeterCount()
        {
            string query = $"SELECT COUNT(*) FROM [ElectricMeter]";

            return Convert.ToInt32(DBService.ExecuteScalar(query, CommandType.Text));
        }
    }
}
