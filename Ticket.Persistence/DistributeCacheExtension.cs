using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace Ticket.Persistence
{
    public interface IDistributedCacheService
    {
        /// <summary>
        /// Lấy dữ liệu cache
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        bool TryGetValue<T>(string key, out T value);

        /// <summary>
        /// Thêm dữ liệu cache
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        Task SetAsync<T>(string key, T value);

        /// <summary>
        /// Thêm dữ liệu cache
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        Task SetAsync<T>(string key, T value, DistributedCacheEntryOptions options);

        /// <summary>
        /// Xóa dữ liệu cache
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task RemoveAsync(string key);
    }

    public class DistributedCacheService : IDistributedCacheService
    {
        private readonly IDistributedCache _cache;
        public DistributedCacheService(IDistributedCache cache)
        {
            _cache = cache;
        }

        public bool TryGetValue<T>(string key, out T value)
        {
            var val = _cache.Get(key);
            value = default;
            if (val == null) return false;
            value = JsonSerializer.Deserialize<T>(val, GetJsonSerializerOptions());
            return true;
        }

        public Task SetAsync<T>(string key, T value)
        {
            return SetAsync(key, value, new DistributedCacheEntryOptions());
        }

        public Task SetAsync<T>(string key, T value, DistributedCacheEntryOptions options)
        {
            var bytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(value, GetJsonSerializerOptions()));
            return _cache.SetAsync(key, bytes, options);
        }

        public Task RemoveAsync(string key)
        {
            return _cache.RemoveAsync(key);
        }

        private static JsonSerializerOptions GetJsonSerializerOptions()
        {
            return new JsonSerializerOptions()
            {
                PropertyNamingPolicy = null,
                WriteIndented = true,
                AllowTrailingCommas = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            };
        }
    }
}
