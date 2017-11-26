using CQRSLite_Retrosheet.Domain.ReadModel.Repositories.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace CQRSLite_Retrosheet.Domain.ReadModel.Repositories
{
    // This is much faster for saves than anything I was able to accomplish using Entity Framework

    public class FlatRepository_MSSQL<T> : IRepository<T> where T : class
    {
        private string insertStatement;
        private string connectionString;
        private Type modelType;
        private string tableName;
        private string repositoryNamespace;
        private ILogger logger;

        public FlatRepository_MSSQL(string ConnectionString, string RepositoryNamespace, ILoggerFactory loggerFactory)
        {
            connectionString = ConnectionString;
            modelType = typeof(T);
            tableName = modelType.Name;
            repositoryNamespace = RepositoryNamespace;
            insertStatement = BuildSqlInsert();
            logger = loggerFactory.CreateLogger("FlatRepository");
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
            T item = Activator.CreateInstance<T>();

            string sql = "select * from " + "[" + repositoryNamespace + "]." + tableName + " where [Key] = @Key";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(sql, connection);
                cmd.Parameters.AddWithValue("@Key", id);
                connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    reader.Read();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        if (reader.GetName(i) != "Key")
                        {
                            SetValue(item, reader.GetName(i), reader.GetValue(i));
                        }
                    }
                }
            }
            return item;
        }

        public List<T> GetByPartialKey(string keySegment, int segmentStart)
        {
            List<T> items = Activator.CreateInstance<List<T>>();

            string sql = "select * from " + "[" + repositoryNamespace + "]." + tableName + " where substring([Key], " + segmentStart.ToString() + ", " + keySegment.Length.ToString() + ") = @keySegment";
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
                        T item = Activator.CreateInstance<T>();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            if (reader.GetName(i) != "Key")
                            {
                                SetValue(item, reader.GetName(i), reader.GetValue(i));
                            }
                        }
                        items.Add(item);
                    }
                }
            }
            return items;
        }

        public async Task SaveAsync(T item, Func<T, string> MakeKey)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(insertStatement, connection);

                    command.Parameters.AddWithValue("@Key", MakeKey(item));

                    foreach (var property in modelType.GetProperties())
                    {
                        var value = property.GetValue(item);
                        command.Parameters.AddWithValue("@" + property.Name, value ?? DBNull.Value);
                    }

                    connection.Open();
                    await command.ExecuteNonQueryAsync();
                }
            }
            catch(Exception ex)
            {
                string jsonItem = JsonConvert.SerializeObject(item);
                string itemType = item.GetType().Name;
                logger.LogError("Save Error ~~~ Error Message: " + ex.Message + " ~~~ Item Type: " + itemType + " ~~~ Item: " + jsonItem);
            }
        }

        private string BuildSqlInsert()
        {
            StringBuilder sbColumns = new StringBuilder("[Key]");
            StringBuilder sbParameters = new StringBuilder("@Key");

            foreach (var property in modelType.GetProperties())
            {
                sbColumns.Append(", [" + property.Name + "]");
                sbParameters.Append(", @" + property.Name);
            }

            return "insert into " + "[" + repositoryNamespace + "]." + tableName + " (" + sbColumns.ToString() + ") values (" + sbParameters.ToString() + ")";
        }

        // https://stackoverflow.com/questions/13270183/type-conversion-issue-when-setting-property-through-reflection
        public void SetValue(object inputObject, string propertyName, object propertyVal)
        {
            if (propertyVal.GetType() == typeof(System.DBNull))
            {
                return;
            }

            //find out the type
            Type type = inputObject.GetType();

            //get the property information based on the type
            System.Reflection.PropertyInfo propertyInfo = type.GetProperty(propertyName);

            //find the property type
            Type propertyType = propertyInfo.PropertyType;

            //Convert.ChangeType does not handle conversion to nullable types
            //if the property type is nullable, we need to get the underlying type of the property
            var targetType = IsNullableType(propertyInfo.PropertyType) ? Nullable.GetUnderlyingType(propertyInfo.PropertyType) : propertyInfo.PropertyType;

            //Returns an System.Object with the specified System.Type and whose value is
            //equivalent to the specified object.
            propertyVal = Convert.ChangeType(propertyVal, targetType);

            //Set the value of the property
            propertyInfo.SetValue(inputObject, propertyVal, null);

        }

        private bool IsNullableType(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>));
        }
    }
}
