using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core
{
    /// <summary>
    /// 缓存接口
    /// </summary>
    public interface ICacheStorageProvider
    {
        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        object Get<T>(string key);
        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void Put<T>(string key, T value);
        /// <summary>
        /// 设置缓存（过期时间）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="validFor"></param>
        void Put<T>(string key, T value, TimeSpan validFor);
        /// <summary>
        /// 清除缓存（KEY）
        /// </summary>
        /// <param name="key"></param>
        void Remove(string key);
        /// <summary>
        /// 清除全部缓存
        /// </summary>
        void Clear();
    }
}
