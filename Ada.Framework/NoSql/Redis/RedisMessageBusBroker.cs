using System;
using System.Collections.Concurrent;
using System.Linq;
using Ada.Framework.Messaging;
using StackExchange.Redis;

namespace Ada.Framework.NoSql.Redis
{
    public class RedisMessageBusBroker : IMessageBroker
    {
        private readonly IRedisConnectionProvider _redisConnectionProvider;

        public const string ConnectionStringKey = "RedisConnectionString";
        private readonly string _connectionString;
        readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public IDatabase Database => _redisConnectionProvider.GetConnection(_connectionString).GetDatabase();

        private readonly ConcurrentDictionary<string, ConcurrentBag<Action<string, string>>> _handlers = new ConcurrentDictionary<string, ConcurrentBag<Action<string, string>>>();

        public RedisMessageBusBroker( IRedisConnectionProvider redisConnectionProvider)
        {
            _redisConnectionProvider = redisConnectionProvider;
            _connectionString = _redisConnectionProvider.GetConnectionString(ConnectionStringKey);
        }
        public void Publish(string channel, string message)
        {
            Database.Publish(channel, GetHostName() + "/" + message);
        }

        public void Subscribe(string channel, Action<string, string> handler)
        {
            try
            {
                var channelHandlers = _handlers.GetOrAdd(channel, c => new ConcurrentBag<Action<string, string>>());
                channelHandlers.Add(handler);

                var sub = _redisConnectionProvider.GetConnection(_connectionString).GetSubscriber();
                sub.Subscribe(channel, (c, m) => {

                    // the message contains the publisher before the first '/'
                    var messageTokens = m.ToString().Split('/');
                    var publisher = messageTokens.FirstOrDefault();
                    var message = messageTokens.Skip(1).FirstOrDefault();

                    if (String.IsNullOrWhiteSpace(publisher))
                    {
                        return;
                    }

                    // ignore self sent messages
                    if (GetHostName().Equals(publisher, StringComparison.OrdinalIgnoreCase))
                    {
                        return;
                    }

                    _logger.Debug("Processing "+ message);
                    handler(c, message);
                });

            }
            catch (Exception e)
            {
                _logger.Error("An error occurred while subscribing to " + channel,e);
            }
        }
        private string GetHostName()
        {
            // use the current host and the process id as two servers could run on the same machine
            return System.Net.Dns.GetHostName() + ":" + System.Diagnostics.Process.GetCurrentProcess().Id;
        }
    }
}
