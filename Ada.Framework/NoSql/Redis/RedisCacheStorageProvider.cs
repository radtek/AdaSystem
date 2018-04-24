using System;
using Ada.Core;
using Newtonsoft.Json;
using StackExchange.Redis;


namespace Ada.Framework.NoSql.Redis
{
    public class RedisCacheStorageProvider : ICacheStorageProvider
    {
        public const string ConnectionStringKey = "RedisConnectionString";
        private readonly ConnectionMultiplexer _connectionMultiplexer;
        public IDatabase Database => _connectionMultiplexer.GetDatabase();

        public RedisCacheStorageProvider(IRedisConnectionProvider redisConnectionProvider)
        {
            var connectionString = redisConnectionProvider.GetConnectionString(ConnectionStringKey);
            _connectionMultiplexer = redisConnectionProvider.GetConnection(connectionString);
        }
        public void Clear()
        {
            Database.KeyDeleteWithPrefix(GetLocalizedKey("*"));
        }

        public object Get<T>(string key)
        {
            var json = Database.StringGet(GetLocalizedKey(key));
            if (String.IsNullOrEmpty(json))
            {
                return default(T);
            }
            return JsonConvert.DeserializeObject<T>(json);
        }

        public void Remove(string key)
        {
            Database.KeyDelete(GetLocalizedKey(key));
        }

        public void Put<T>(string key, T value)
        {
            var json = JsonConvert.SerializeObject(value);
            Database.StringSet(GetLocalizedKey(key), json, null);
        }

        public void Put<T>(string key, T value, TimeSpan validFor)
        {
            var json = JsonConvert.SerializeObject(value);
            Database.StringSet(GetLocalizedKey(key), json, validFor);
        }

        private string GetLocalizedKey(string key)
        {
            return "Ada:Cache:" + key;
        }
    }
}
