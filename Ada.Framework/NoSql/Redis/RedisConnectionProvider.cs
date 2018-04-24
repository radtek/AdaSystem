using System;
using System.Collections.Concurrent;
using System.Configuration;
using StackExchange.Redis;

namespace Ada.Framework.NoSql.Redis
{
    public class RedisConnectionProvider : IRedisConnectionProvider
    {

        private static readonly ConcurrentDictionary<string, Lazy<ConnectionMultiplexer>> ConnectionMultiplexers = new ConcurrentDictionary<string, Lazy<ConnectionMultiplexer>>();
        public ConnectionMultiplexer GetConnection(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentNullException(nameof(connectionString));
            }
            return ConnectionMultiplexers.GetOrAdd(connectionString,
                new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(connectionString))).Value;

        }

        public string GetConnectionString(string service)
        {
            var connetString = ConfigurationManager.AppSettings[service];
            if (string.IsNullOrWhiteSpace(connetString))
            {
                throw new ConfigurationErrorsException("未在WEB.CONFIG配置Redis连接字符串：" + service);
            }
            return connetString;
        }
    }
}
