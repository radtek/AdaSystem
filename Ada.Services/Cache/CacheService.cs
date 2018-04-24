using System;
using Ada.Core;


namespace Ada.Services.Cache
{
    /// <summary>
    /// Provides a per tenant <see cref="ICacheService"/> implementation.
    /// </summary>
    public class CacheService : ICacheService {
        private readonly ICacheStorageProvider _cacheStorageProvider;


        public CacheService(
            ICacheStorageProvider cacheStorageProvider) {
            _cacheStorageProvider = cacheStorageProvider;

        }

        public object GetObject<T>(string key) {
            return _cacheStorageProvider.Get<T>(BuildFullKey(key));
        }

        public void Put<T>(string key, T value) {
            _cacheStorageProvider.Put(BuildFullKey(key), value);
        }

        public void Put<T>(string key, T value, TimeSpan validFor) {
            _cacheStorageProvider.Put(BuildFullKey(key), value, validFor);
        }

        public void Remove(string key) {
            _cacheStorageProvider.Remove(BuildFullKey(key));
        }

        public void Clear() {
            _cacheStorageProvider.Clear();
        }

        private string BuildFullKey(string key) {
            return String.Concat("Ada", ":", key);
        }
    }
}