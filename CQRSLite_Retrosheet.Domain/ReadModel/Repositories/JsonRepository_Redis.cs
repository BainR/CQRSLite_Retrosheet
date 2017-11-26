using CQRSLite_Retrosheet.Domain.ReadModel.Repositories.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace CQRSLite_Retrosheet.Domain.ReadModel.Repositories
{
    public class JsonRepository_Redis<T> : IRepository<T> where T : class
    {
        private string connectionString;
        private Type modelType;
        private string tableName;
        private string repositoryNamespace;
        private IConnectionMultiplexer redisConnection;
        private ILogger logger;

        public JsonRepository_Redis(string ConnectionString, string RepositoryNamespace, ILoggerFactory loggerFactory)
        {
            connectionString = ConnectionString;
            modelType = typeof(T);
            tableName = modelType.Name;
            repositoryNamespace = RepositoryNamespace;
            redisConnection = ConnectionMultiplexer.Connect(connectionString);
            logger = loggerFactory.CreateLogger("RedisRepository");
        }

        public bool Exists(string id)
        {
            var key = MakeKey(id);
            var database = redisConnection.GetDatabase();
            var serializedObject = database.StringGet(key);
            return !serializedObject.IsNullOrEmpty;
        }

        public T GetById(string id)
        {
            var key = MakeKey(id);
            var database = redisConnection.GetDatabase();
            var serializedObject = database.StringGet(key);
            if (serializedObject.IsNullOrEmpty)
            {
                return null;
            }
            return JsonConvert.DeserializeObject<T>(serializedObject.ToString()) as T;
        }

        public List<T> GetByPartialKey(string id, int segmentStart)
        {
            id = new string('?', segmentStart - 1) + id + "*";

            List<T> items = new List<T>();

            EndPoint[] endpoints = redisConnection.GetEndPoints();
            var database = redisConnection.GetDatabase();
            var server = redisConnection.GetServer(endpoints[0]);
            var keys = server.Keys(0, MakeKey(id)); // not recommended for production environments
            foreach(var key in keys)
            {
                var serializedObject = database.StringGet(key);
                items.Add(JsonConvert.DeserializeObject<T>(serializedObject.ToString()) as T);
            }

            return items;
        }

        public async Task SaveAsync(T item, Func<T, string> MakeItemId)
        {
            string itemKey = MakeKey(MakeItemId(item));
            string jsonItem = JsonConvert.SerializeObject(item);

            try
            {
                var database = redisConnection.GetDatabase();
                await database.StringSetAsync(itemKey, jsonItem);
            }
            catch (Exception ex)
            {
                string itemType = item.GetType().Name;
                logger.LogError("Save Error ~~~ Error Message: " + ex.Message + " ~~~ Item Type: " + itemType + " ~~~ Item: " + jsonItem);
            }
        }

        private string MakeKey(string id)
        {
            if (!id.StartsWith(repositoryNamespace + "_" + tableName + ":"))
            {
                return repositoryNamespace + "_" + tableName + ":" + id;
            }
            else
            {
                return id; //Key is already suffixed with namespace
            }
        }
    }
}
