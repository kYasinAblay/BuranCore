using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Buran.Core.MvcLibrary.Cache
{
    public static class CacheManager
    {
        public static async Task SetAsync<T>(IDistributedCache cache, string key, T value)
        {
            await cache.SetStringAsync(key, JsonConvert.SerializeObject(value));
        }

        public static async Task<T> GetAsync<T>(IDistributedCache cache, string key)
        {
            var value = await cache.GetStringAsync(key);
            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }

        public static async Task<bool> ExistAsync<T>(IDistributedCache cache, string key)
        {
            var value = await cache.GetStringAsync(key);
            return value == null ? false : true;
        }

        public static async Task RemoveAsync(IDistributedCache cache, string key)
        {
            await cache.RemoveAsync(key);
        }


        public static void Set<T>(IDistributedCache cache, string key, T value)
        {
            cache.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static T Get<T>(IDistributedCache cache, string key)
        {
            var value = cache.GetString(key);
            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }

        public static bool Exist(IDistributedCache cache, string key)
        {
            var value = cache.GetString(key);
            return value == null ? false : true;
        }

        public static void Remove(IDistributedCache cache, string key)
        {
            cache.Remove(key);
        }
    }
}
