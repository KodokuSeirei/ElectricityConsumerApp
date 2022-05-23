using ElectricityConsumerApp.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace ElectricityConsumerApp.Service
{
    internal class TestimonyHistoryService
    {
        public static Testimony TestimonyReaderFromDataSet(DataSet dataSet)
        {
            Testimony testimony = new Testimony();
            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                testimony = new Testimony()
                {
                    ID = Convert.ToInt32(row["ID"]),
                    ElectricMeterNumber = Convert.ToInt32(row["ElectricMeterNumber"] ?? 0),
                    Value = Convert.ToInt32(row["Value"] ?? 0),
                    Date = Convert.ToDateTime(row["Date"] ?? new DateTime())
                };
            }
            return testimony;
        }

        public static Testimony GetTestimony(int testimonyId)
        {
            string query = $@"SELECT * FROM [TestimonyHistory]
                WHERE [ID] = {testimonyId}";

            return TestimonyReaderFromDataSet(DBService.ExecuteDataSet(query));
        }

        public static DataTable GetTestimonyHistoryTable(int electricMeterNumber)
        {
            string query = $@"SELECT * FROM [TestimonyHistory]
            WHERE [ElectricMeterNumber] = {electricMeterNumber}";

            DataSet dataSet = DBService.ExecuteDataSet(query);

            return dataSet.Tables[0];
        }
        public static void AddTestimony(Testimony testimony)
        {
            string query = "[sp_CreateTestimony]";
            SqlParameter sqlIDParameter = new SqlParameter("@id", 0);
            sqlIDParameter.Direction = ParameterDirection.Output;
            List<SqlParameter> sqlParameters = new List<SqlParameter>() {
                sqlIDParameter,
                new SqlParameter("@electricMeterNumber", testimony.ElectricMeterNumber),
                new SqlParameter("@value", testimony.Value) };
            DBService.ExecuteNonQuery(query, CommandType.StoredProcedure, sqlParameters);
        }

        public static void UpdateTestimony(Testimony testimony)
        {
            string query = "[sp_UpdateTestimony]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>() {
                 new SqlParameter("@id", testimony.ID),
                new SqlParameter("@value", testimony.Value)};

            DBService.ExecuteNonQuery(query, CommandType.StoredProcedure, sqlParameters);
        }

        public static void DeleteTestimony(int testimonyId)
        {
            string query = $"DELETE [TestimonyHistory] WHERE [ID] = {testimonyId}";

            DBService.ExecuteNonQuery(query);
        }

        public static int GetMaxValueByEectricMeterNumber(int electricMeterNumber)
        {
            string query = $@"SELECT MAX([Value]) AS [MaxValue]
            FROM [TestimonyHistory]
            WHERE [ElectricMeterNumber] = {electricMeterNumber}";
            var maxValue = DBService.ExecuteScalar(query, CommandType.Text);
            if (!(maxValue is DBNull))
                return Convert.ToInt32(maxValue);
            return 0;
        }

        public static int GetAllTestimonyCount()
        {
            string query = $"SELECT COUNT(*) FROM [TestimonyHistory]";

            return Convert.ToInt32(DBService.ExecuteScalar(query, CommandType.Text));
        }
    }
}
