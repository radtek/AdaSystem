using System;
using Ada.Core;

namespace Ada.Framework.Caching {
    public interface ICacheHolder : ISingleDependency {
        ICache<TKey, TResult> GetCache<TKey, TResult>(Type component);
    }
}
