using System.Text.Json;
using StackExchange.Redis;

namespace AuthService.Services.Cache
{
    public interface ICacheService
    {
        /// <summary>
        /// Get data from cache by key
        /// <para>Author: TaiPV</para>
        /// <para>Created: 08/10/2024</para>
        /// </summary>
        /// <typeparam name="T">type of data</typeparam>
        /// <param name="key">key of data</param>
        /// <returns></returns>
        T GetData<T>(string key);

        /// <summary>
        /// Set data to cache by key
        /// <para>Author: TaiPV</para>
        /// <para>Created: 08/10/2024</para>
        /// </summary>
        /// <typeparam name="T">type of data</typeparam>
        /// <param name="key">key of data</param>
        /// <param name="value">value of data</param>
        /// <param name="timeEnd">the time when the cache will be expired</param>
        /// <returns></returns>
        bool SetData<T>(string key, T value, DateTimeOffset timeEnd);

        /// <summary>
        /// Remove data from cache by key
        /// <para>Author: TaiPV</para>
        /// <para>Created: 08/10/2024</para>
        /// </summary>
        /// <param name="key">key of data</param>
        /// <returns></returns>
        object RemoveData(string key);
    }

    public class CacheService : ICacheService
    {
        private IDatabase _cacheDb;

        public CacheService(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("RedisConnection");
            ConnectToRedis(connectionString);
        }

        public T GetData<T>(string key)
        {
            var value = _cacheDb.StringGet(key);
            if (value.HasValue)
            {
                return JsonSerializer.Deserialize<T>(value);
            }
            return default;
        }

        public object RemoveData(string key)
        {
            if (_cacheDb == null)
            {
                return null;
            }

            var isExist = _cacheDb.KeyExists(key);
            if (isExist)
            {
                return _cacheDb.KeyDelete(key);
            }

            return null;
        }

        public bool SetData<T>(string key, T value, DateTimeOffset timeEnd)
        {
            var expireTime = timeEnd.DateTime.Subtract(DateTime.Now);

            if (_cacheDb == null)
            {
                return false;
            }

            // Trả về true nếu set thành công, ngược lại trả về false. Nếu key đã tồn tại thì sẽ bị ghi đè.
            return _cacheDb.StringSet(key, JsonSerializer.Serialize(value), expireTime);
        }

        private void ConnectToRedis(string connectionString)
        {
            try
            {
                var options = ConfigurationOptions.Parse(connectionString);
                options.AbortOnConnectFail = false; // Cho phép thực hiện retry nếu kết nối thất bại
                options.ConnectRetry = 3;
                var redis = ConnectionMultiplexer.Connect(options);
                _cacheDb = redis.GetDatabase();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error when connect to redis: " + e.Message);
                throw;
            }
        }
    }
}