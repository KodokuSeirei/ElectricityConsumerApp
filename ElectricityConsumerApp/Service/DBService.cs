using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows;

namespace ElectricityConsumerApp.Service
{
    internal static class DBService
    {
        private static string connectionString = ConfigurationManager.ConnectionStrings["ElectricityConsumer"].ConnectionString;

        public static DataSet ExecuteDataSet(string query)
        {
            DataSet ds = new DataSet();
            try
            {

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);

                    adapter.Fill(ds);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return ds;
        }

        public static object ExecuteScalar(string query, CommandType commandType, List<SqlParameter> sqlParameters = null)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    SqlCommand command = new SqlCommand(query, connection);
                    command.CommandType = commandType;

                    if (sqlParameters != null)
                        foreach (SqlParameter parameter in sqlParameters)
                        {
                            command.Parameters.Add(parameter);
                        }

                    return command.ExecuteScalar();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return 0;
        }

        public static void ExecuteNonQuery(string query, CommandType commandType, List<SqlParameter> sqlParameters = null)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    SqlCommand command = new SqlCommand(query, connection);
                    command.CommandType = commandType;

                    if (sqlParameters != null)
                        foreach (SqlParameter parameter in sqlParameters)
                        {
                            command.Parameters.Add(parameter);
                        }

                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public static void ExecuteNonQuery(string query)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    SqlCommand command = new SqlCommand(query, connection);

                    command.CommandType = CommandType.Text;

                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
