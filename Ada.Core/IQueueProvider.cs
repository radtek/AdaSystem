using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core
{
    public interface IQueueProvider : ISingleDependency
    {
        /// <summary>
        /// 删除指定队列
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void Remove<T>(string key, T value);
        /// <summary>
        /// 获取指定队列
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        List<T> Get<T>(string key);
        /// <summary>
        /// 入队
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void Push<T>(string key, T value);
        /// <summary>
        /// 出对
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        T Pop<T>(string key);
        /// <summary>
        /// 队列长度
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        long Length(string key);
    }
}
