using ElectricityConsumerApp.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;

namespace ElectricityConsumerApp.Service
{
    public static class AddressService
    {
        public static Address AddressReaderFromDataSet(DataSet dataSet)
        {
            Address address = new Address();
            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                address = new Address()
                {
                    ID = Convert.ToInt32(row["ID"]),
                    CityID = Convert.ToInt32(row["CityID"]),
                    Street = Convert.ToString(row["Street"] ?? String.Empty),
                    Home = Convert.ToString(row["Home"] ?? String.Empty),
                    Fiat = Convert.ToString(row["Fiat"] ?? String.Empty)
                };
            }
            return address;
        }

        public static City CityReaderFromDataSet(DataSet dataSet)
        {
            City city = new City();
            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                city = new City()
                {
                    ID = Convert.ToInt32(row["ID"]),
                    Name = Convert.ToString(row["CityName"] ?? String.Empty),
                    RegionID = Convert.ToInt32(row["id_region"]),
                    RegionName = Convert.ToString(row["RegionName"] ?? String.Empty),
                    OkrugID = Convert.ToInt32(row["id_okrug"]),
                    OkrugName = Convert.ToString(row["OkrugName"] ?? String.Empty)
                };
            }
            return city;
        }

        public static Address GetAddress(int addressID)
        {
            string query = $@"SELECT * FROM [AddressesDirectory]
            WHERE [ID] = {addressID}";
            Address address = AddressReaderFromDataSet(DBService.ExecuteDataSet(query));
            if (address != null || address.CityID > 0)
                address.City = GetCity(address.CityID);
            return address;
        }

        public static City GetCity(int cityID)
        {
            string query = $@"SELECT [fs_city].[id] AS [ID], [fs_city].[name] AS [CityName], [id_region],
            [fs_region].[name] AS [RegionName], [id_okrug], [fs_okrug].[name] AS [OkrugName] FROM [fs_city]
            LEFT JOIN [fs_region] ON [fs_city].[id_region] = [fs_region].[id]
            LEFT JOIN [fs_okrug] ON [fs_region].[id_okrug] = [fs_okrug].[id]
            WHERE [fs_city].[id] = {cityID}";
            return CityReaderFromDataSet(DBService.ExecuteDataSet(query));
        }

        public static IEnumerable<string> GetAllRegionNames()
        {
            IEnumerable<string> regionNames = new List<string>();
            try
            {
                string query = @"SELECT [name] FROM [fs_region] ORDER BY [name]";
                IEnumerable<object> objects = DataHelper.DataSetReaderOneColumn(DBService.ExecuteDataSet(query), "name");
                regionNames = objects.Select(x => (string)x);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"При получении имён регионов произошла ошибка \"{ex.Message}\"");
            }
            return regionNames;
        }

        public static IEnumerable<string> GetCityNamesByRegionName(string regionName)
        {
            IEnumerable<string> cityNames = new List<string>();
            try
            {
                string query = $@"SELECT [fs_city].[name] FROM [fs_city] 
                LEFT JOIN [fs_region] ON [id_region] = [fs_region].[id] 
                WHERE [fs_region].[name] = '{regionName}'
                ORDER BY [fs_city].[name]";
                IEnumerable<object> objects = DataHelper.DataSetReaderOneColumn(DBService.ExecuteDataSet(query), "name");
                cityNames = objects.Select(x => (string)x);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"При получении имён городов произошла ошибка \"{ex.Message}\"");
            }
            return cityNames;
        }

        private static int AddAddress(Address address)
        {
            string query = "[sp_CreateAddress]";
            SqlParameter sqlIDParameter = new SqlParameter("@ID", 0);
            sqlIDParameter.Direction = ParameterDirection.Output;
            List<SqlParameter> sqlParameters = new List<SqlParameter>() {
                sqlIDParameter,
                new SqlParameter("@cityID", address.CityID),
                new SqlParameter("@street", address.Street),
                new SqlParameter("@home", address.Home),
                new SqlParameter("@fiat", address.Fiat)};

            return Convert.ToInt32(DBService.ExecuteScalar(query, CommandType.StoredProcedure, sqlParameters));
        }

        public static int UpdateAddressForConsumer(Address address, int consumerID)
        {
            int addressID = GetAddressIDByContent(address);

            if (addressID == 0)
                addressID = AddAddress(address);
            string query = $@"UPDATE [Consumer] SET [AddressID] = {addressID} WHERE [ID] = {consumerID}";
            DBService.ExecuteNonQuery(query, CommandType.Text);
            DeleteUnusedAddresses();
            return addressID;
        }

        public static int GetAddressIDByContent(Address address)
        {
            string query = $@"SELECT [ID] FROM [AddressesDirectory] 
                WHERE [CityID] = '{address.CityID}' 
                AND [Street] = '{address.Street}' 
                AND [Home] = '{address.Home}' 
                AND [Fiat] = '{address.Fiat}'";
            int addressID = Convert.ToInt32(DBService.ExecuteScalar(query, CommandType.Text));
            return addressID;
        }

        public static int GetCityIDByCityAndRegionNames(string regionName, string cityName)
        {
            string query = $@"SELECT [fs_city].[id] FROM [fs_city] 
                LEFT JOIN [fs_region] ON [fs_city].[id_region] = [fs_region].[id]
                WHERE [fs_region].[name] = '{regionName}'
                AND [fs_city].[name] = '{cityName}'";
            int cityID = Convert.ToInt32(DBService.ExecuteScalar(query, CommandType.Text));
            return cityID;
        }

        public static void DeleteUnusedAddresses()
        {
            string query = $@"DELETE FROM [AddressesDirectory] 
            WHERE [ID] IN 
                (SELECT [AddressesDirectory].[ID] FROM [AddressesDirectory]  
                LEFT JOIN [Consumer] ON [AddressesDirectory].[ID] = [Consumer].[AddressID] 
                WHERE [Consumer].[ID] IS NULL)";
            DBService.ExecuteNonQuery(query, CommandType.Text);
        }
    }

}
