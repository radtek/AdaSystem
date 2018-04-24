using Ada.Core;
using StackExchange.Redis;

namespace Ada.Framework.NoSql.Redis
{
    /// <summary>
    /// 连接Redis接口（单例）
    /// </summary>
    public interface IRedisConnectionProvider : ISingleDependency
    {
        /// <summary>
        /// Redis核心类
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        ConnectionMultiplexer GetConnection(string connectionString);
        /// <summary>
        /// 获取连接字符串
        /// </summary>
        /// <param name="service"></param>
        /// <returns></returns>
        string GetConnectionString(string service);
    }
}
