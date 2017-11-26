using CQRSLite_Retrosheet.Domain.ReadModel.Repositories.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace CQRSLite_Retrosheet.Domain.ReadModel.Repositories
{
    public class JsonRepository_MSSQL<T> : IRepository<T> where T : class
    {
        private string connectionString;
        private Type modelType;
        private string tableName;
        private string repositoryNamespace;
        private ILogger logger;

        public JsonRepository_MSSQL(string ConnectionString, string RepositoryNamespace, ILoggerFactory LoggerFactory)
        {
            connectionString = ConnectionString;
            modelType = typeof(T);
            tableName = modelType.Name;
            repositoryNamespace = RepositoryNamespace;
            logger = LoggerFactory.CreateLogger("JsonRepository");
        }

        public bool Exists(string id)
        {
            string sql = "select rows = count(1) from " + "[" + repositoryNamespace + "]." + tableName + " where [Key] = @Key";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(sql, connection);
                cmd.Parameters.AddWithValue("@Key", id);
                connection.Open();
                int rowcount = (int)cmd.ExecuteScalar();
                return rowcount > 0;
            }
        }

        public T GetById(string id)
        {
            string sql = "select Json from " + "[" + repositoryNamespace + "]." + tableName + " where [Key] = @Key";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(sql, connection);
                cmd.Parameters.AddWithValue("@Key", id);
                connection.Open();
                string json = (string)cmd.ExecuteScalar();

                return json != null ? JsonConvert.DeserializeObject<T>(json) : null;
            }
        }

        public List<T> GetByPartialKey(string keySegment, int segmentStart)
        {
            List<T> items = Activator.CreateInstance<List<T>>();

            string sql = "select Json from " + "[" + repositoryNamespace + "]." + tableName 
                + " where substring([Key], " + segmentStart.ToString() + ", " + keySegment.Length.ToString() + ") = @keySegment";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(sql, connection);
                cmd.Parameters.AddWithValue("@keySegment", keySegment);
                connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        string json = reader.GetString(0);
                        items.Add(JsonConvert.DeserializeObject<T>(json));
                    }
                }
            }
            return items;
        }

        public async Task SaveAsync(T item, Func<T, string> MakeKey)
        {
            var key = MakeKey(item);
            string queryString = "insert into " + "[" + repositoryNamespace + "]." + tableName + " ([key], [json]) values (@key, @json)" + Environment.NewLine;
            string jsonItem = JsonConvert.SerializeObject(item);

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(queryString, connection);
                    command.Parameters.AddWithValue("@key", key);
                    command.Parameters.AddWithValue("@json", jsonItem);
                    connection.Open();
                    await command.ExecuteNonQueryAsync();
                }
            }
            catch(Exception ex)
            {
                string itemType = item.GetType().Name;
                logger.LogError("Save Error ~~~ Error Message: " + ex.Message + " ~~~ Item Type: " + itemType + " ~~~ Item: " + jsonItem);
            }
        }
    }
}
