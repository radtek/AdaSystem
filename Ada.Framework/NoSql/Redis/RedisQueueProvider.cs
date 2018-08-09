
using System.Collections.Generic;
using Ada.Core;
using Ada.Core.Tools;
using StackExchange.Redis;

namespace Ada.Framework.NoSql.Redis
{
    public class RedisQueueProvider : IQueueProvider
    {
        public const string ConnectionStringKey = "RedisConnectionString";
        private readonly ConnectionMultiplexer _connectionMultiplexer;
        public IDatabase Database => _connectionMultiplexer.GetDatabase();

        public RedisQueueProvider(IRedisConnectionProvider redisConnectionProvider)
        {
            var connectionString = redisConnectionProvider.GetConnectionString(ConnectionStringKey);
            _connectionMultiplexer = redisConnectionProvider.GetConnection(connectionString);
        }

        public void Remove<T>(string key, T value)
        {
            Database.ListRemove(GetLocalizedKey(key), SerializeHelper.SerializeToString(value));
        }

        public List<T> Get<T>(string key)
        {
            var result = Database.ListRange(GetLocalizedKey(key));
            List<T> list = new List<T>();
            foreach (var redisValue in result)
            {
                list.Add(SerializeHelper.DeserializeToObject<T>(redisValue));
            }

            return list;
        }

        public void Push<T>(string key, T value)
        {
            Database.ListRightPush(GetLocalizedKey(key), SerializeHelper.SerializeToString(value));
        }

        public T Pop<T>(string key)
        {
            var result = Database.ListLeftPop(GetLocalizedKey(key));
            return SerializeHelper.DeserializeToObject<T>(result);
        }

        public long Length(string key)
        {
            return Database.ListLength(GetLocalizedKey(key));
        }

        private string GetLocalizedKey(string key)
        {
            return "Ada:Queue:" + key;
        }
    }
}
