using CQRSLite_Retrosheet.Domain.ReadModel.Repositories.Interfaces;
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

        public JsonRepository_Redis(string ConnectionString, string RepositoryNamespace)
        {
            connectionString = ConnectionString;
            modelType = typeof(T);
            tableName = modelType.Name;
            repositoryNamespace = RepositoryNamespace;
            redisConnection = ConnectionMultiplexer.Connect(connectionString);
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
            var database = redisConnection.GetDatabase();
            await database.StringSetAsync(itemKey, JsonConvert.SerializeObject(item));
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
