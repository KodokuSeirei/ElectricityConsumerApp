using ElectricityConsumerApp.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace ElectricityConsumerApp.Service
{
    internal static class ConsumerService
    {
        public static DataSet GetAllConsumersDataSet()
        {
            string query =
                @"SELECT [Consumer].[ID], [LastName], [FirstName], [Patronymic], [ElectricMeterNumber], 
                CONCAT([fs_region].[name], ', ', [fs_city].[name], ', ул.', [Street], ', дом ', [Home], ', кв ', [Fiat]) AS [Address] FROM [Consumer] 
				LEFT JOIN [AddressesDirectory] ON [AddressID] = [AddressesDirectory].[ID]
				LEFT JOIN [fs_city] ON [CityID] = [fs_city].[id]
				LEFT JOIN [fs_region] ON [id_region] = [fs_region].[id]
                CROSS APPLY
				(
					SELECT CONCAT([ElectricMeterNumber], '; ')
					FROM [ConsumerElectricMeterBinding]
					WHERE [ConsumerElectricMeterBinding].[ConsumerID] = [Consumer].[ID]
					FOR XML PATH('')
				) C ([ElectricMeterNumber]);";

            DataSet dataSet = DBService.ExecuteDataSet(query);
            return dataSet;
        }

        public static DataSet GetConsumersDataSetByLastName(string lastName)
        {
            string query =
                $@"SELECT [Consumer].[ID], [LastName], [FirstName], [Patronymic], [ElectricMeterNumber], 
                CONCAT([fs_region].name, ', ', [fs_city].[name], ', ул.', [Street], ', дом ', [Home], ', кв ', [Fiat]) AS [Address] FROM [Consumer] 
				LEFT JOIN [AddressesDirectory] ON [AddressID] = [AddressesDirectory].[ID]
				LEFT JOIN [fs_city] ON [CityID] = [fs_city].[id]
				LEFT JOIN [fs_region] ON [id_region] = [fs_region].[id]
                CROSS APPLY
				(
					SELECT CONCAT([ElectricMeterNumber], '; ')
					FROM [ConsumerElectricMeterBinding]
					WHERE [ConsumerElectricMeterBinding].[ConsumerID] = [Consumer].[ID]
					FOR XML PATH('')
				) C ([ElectricMeterNumber])
                WHERE [LastName] = '{lastName}'";

            DataSet dataSet = DBService.ExecuteDataSet(query);
            return dataSet;
        }

        public static Consumer ConsumerReaderFromDataSet(DataSet dataSet)
        {
            Consumer consumer = new Consumer();
            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                consumer = new Consumer()
                {
                    ID = Convert.ToInt32(row["ID"]),
                    LastName = Convert.ToString(row["LastName"] ?? String.Empty),
                    FirstName = Convert.ToString(row["FirstName"] ?? String.Empty),
                    Patronymic = Convert.ToString(row["Patronymic"] ?? String.Empty),
                    ElectricMeterNumbers = Convert.ToString(row["ElectricMeterNumber"] ?? String.Empty),
                    AddressID = Convert.ToInt32(row["AddressID"] ?? 0),
                };
            }
            if (consumer.AddressID > 0)
                consumer.Address = AddressService.GetAddress(consumer.AddressID);
            return consumer;
        }

        public static Consumer GetConsumer(int consumerID)
        {
            string query = $@"SELECT [Consumer].[ID], [LastName], [FirstName], [Patronymic], [ElectricMeterNumber], [AddressID] FROM [Consumer] 
               CROSS APPLY
				(
					SELECT CONCAT([ElectricMeterNumber], '; ')
					FROM [ConsumerElectricMeterBinding]
					WHERE [ConsumerElectricMeterBinding].[ConsumerID] = [Consumer].[ID]
					FOR XML PATH('')
				) C ([ElectricMeterNumber]) 
                WHERE [Consumer].[ID] = {consumerID}";

            return ConsumerReaderFromDataSet(DBService.ExecuteDataSet(query));
        }

        public static int AddConsumer(Consumer consumer, string electricMeterNumbers)
        {
            string query = "[sp_CreateConsumer]";
            SqlParameter sqlIDParameter = new SqlParameter("@id", 0);
            sqlIDParameter.Direction = ParameterDirection.Output;
            List<SqlParameter> sqlParameters = new List<SqlParameter>() {
                sqlIDParameter,
                new SqlParameter("@lastName", consumer.LastName),
                new SqlParameter("@firstName", consumer.FirstName),
                new SqlParameter("@patronymic", consumer.Patronymic)};
            int consumerId = Convert.ToInt32(DBService.ExecuteScalar(query, CommandType.StoredProcedure, sqlParameters));
            string[] electricMeterNumbersArray = electricMeterNumbers.Trim().Split(";");
            for (int i = 0; i < electricMeterNumbersArray.Length; i++)
            {
                int electricMeterNumber = 0;
                if (Int32.TryParse(electricMeterNumbersArray[i], out electricMeterNumber))
                    AddConsumerElectricMeterBinding(consumerId, electricMeterNumber);
            }

            AddressService.UpdateAddressForConsumer(consumer.Address, consumerId);

            return consumerId;
        }

        public static void UpdateConsumer(Consumer consumer, string electricMeterNumbers)
        {
            string query = "[sp_UpdateConsumer]";
            consumer.AddressID = AddressService.UpdateAddressForConsumer(consumer.Address, consumer.ID);
            List<SqlParameter> sqlParameters = new List<SqlParameter>() {
                new SqlParameter("@id", consumer.ID),
                new SqlParameter("@lastName", consumer.LastName),
                new SqlParameter("@firstName", consumer.FirstName),
                new SqlParameter("@patronymic", consumer.Patronymic)};

            DBService.ExecuteNonQuery(query, CommandType.StoredProcedure, sqlParameters);

            string[] electricMeterNumbersArray = electricMeterNumbers.Trim().Split(";");

            if (electricMeterNumbersArray.Length > 0)
                DeleteConsumerElectricMeterBindings(consumer.ID);

            for (int i = 0; i < electricMeterNumbersArray.Length; i++)
            {
                int electricMeterNumber = 0;
                if (Int32.TryParse(electricMeterNumbersArray[i], out electricMeterNumber))
                    AddConsumerElectricMeterBinding(consumer.ID, electricMeterNumber);
            }
            AddressService.UpdateAddressForConsumer(consumer.Address, consumer.ID);
        }

        public static void AddConsumerElectricMeterBinding(int consumerID, int electricMeterNumber)
        {
            string query = "[sp_CreateConsumerElectricMeterBinding]";
            List<SqlParameter> sqlParameters = new List<SqlParameter>() {
                new SqlParameter("@consumerID", consumerID),
                new SqlParameter("@electricMeterNumber", electricMeterNumber)};
            DBService.ExecuteNonQuery(query, CommandType.StoredProcedure, sqlParameters);
        }

        public static void DeleteConsumerElectricMeterBindings(int consumerID)
        {
            string query = $@"DELETE FROM [ConsumerElectricMeterBinding]
                WHERE [ConsumerID] = {consumerID}";

            DBService.ExecuteNonQuery(query, CommandType.Text);
        }

        public static bool CheckConsumerElectricMeter(int consumerID, int electricMeterNumber)
        {
            string query = $"SELECT [ConsumerID] WHERE [ConsumerID] = {consumerID} AND [ElectricMeterNumber] = {electricMeterNumber}";
            SqlParameter sqlIDParameter = new SqlParameter("@id", 0);
            sqlIDParameter.Direction = ParameterDirection.Output;
            bool exist = Convert.ToInt32(DBService.ExecuteScalar(query, CommandType.Text)) > 0;
            return exist;
        }

        public static void DeleteConsumer(int consumerID)
        {
            string query = $"DELETE [Consumer] WHERE [ID] = {consumerID}";

            DBService.ExecuteNonQuery(query);
        }

        public static int GetAllConsumerCount()
        {
            string query = $"SELECT COUNT(*) FROM [Consumer]";

            return Convert.ToInt32(DBService.ExecuteScalar(query, CommandType.Text));
        }
    }
}
