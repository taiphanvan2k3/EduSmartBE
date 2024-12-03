using System.Text.Json;
using StackExchange.Redis;

namespace CourseManagementService.Services.Cache
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
        /// Get multiple data from cache by keys (MGET)
        /// <para>Author: TaiPV</para>
        /// <para>Created: 30/11/2024</para>
        /// </summary>
        public Task<List<T>> GetMultipleDataAsync<T>(List<string> keys);

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

        /// <summary>
        /// Remove data from cache by pattern
        /// </summary>
        /// <param name="pattern"></param>
        Task RemoveDataByPattern(string pattern);

        /// <summary>
        /// Remove data from cache by pattern (using lua script)
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        Task RemoveDataByPatternUsingLuaScript(string pattern);

        /// <summary>
        /// Remove data from cache by patterns
        /// </summary>
        /// <param name="patterns"></param>
        /// <returns></returns>
        Task RemoveDataByPatterns(List<string> patterns);
    }

    public class CacheService(IConnectionMultiplexer connectionMultiplexer, ILogger<CacheService> logger) : ICacheService
    {
        private readonly IDatabase _cacheDb = connectionMultiplexer.GetDatabase();
        private readonly ILogger<CacheService> _logger = logger
            ?? throw new ArgumentNullException(nameof(logger));

        public T GetData<T>(string key)
        {
            var value = _cacheDb.StringGet(key);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            if (value.HasValue)
            {
                return JsonSerializer.Deserialize<T>(value, options);
            }
            return default;
        }

        public async Task<List<T>> GetMultipleDataAsync<T>(List<string> keys)
        {
            var redisValues = await _cacheDb.StringGetAsync(keys.Select(x => (RedisKey)x).ToArray());
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            return redisValues
                .Select(value => value.HasValue ? JsonSerializer.Deserialize<T>(value, options) : default)
                .ToList();
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

        public async Task RemoveDataByPattern(string pattern)
        {
            try
            {
                if (_cacheDb == null)
                {
                    return;
                }

                var endpoints = _cacheDb.Multiplexer.GetEndPoints();
                var server = _cacheDb.Multiplexer.GetServer(endpoints[0]);
                var keys = server.Keys(pattern: pattern + "*");
                List<Task> tasks = [];
                foreach (var key in keys)
                {
                    tasks.Add(_cacheDb.KeyDeleteAsync(key));
                }

                await Task.WhenAll(tasks);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error when remove data from cache by pattern: {Pattern}", pattern);
            }
        }

        public async Task RemoveDataByPatternUsingLuaScript(string pattern)
        {
            try
            {
                if (_cacheDb == null) return;

                var endpoints = _cacheDb.Multiplexer.GetEndPoints();
                var server = _cacheDb.Multiplexer.GetServer(endpoints[0]);
                var keys = server.Keys(pattern: pattern + "*").ToArray();

                // Gửi duy nhất 1 lệnh tới Redis Server thay vì gửi nhiều lệnh nhỏ như hàm RemoveDataByPattern
                var script = @"
                    for i, key in ipairs(KEYS) do
                        redis.call('DEL', key)
                    end";

                await _cacheDb.ScriptEvaluateAsync(script, keys);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error when remove data from cache by pattern using lua script: {Pattern}", pattern);
            }
        }

        public async Task RemoveDataByPatterns(List<string> patterns)
        {
            try
            {
                if (patterns == null || patterns.Count == 0)
                {
                    return;
                }

                var endpoints = _cacheDb.Multiplexer.GetEndPoints();
                var server = _cacheDb.Multiplexer.GetServer(endpoints[0]);

                List<Task> tasks = [];
                foreach (var pattern in patterns)
                {
                    var keys = server.Keys(pattern: pattern + "*");
                    foreach (var key in keys)
                    {
                        tasks.Add(_cacheDb.KeyDeleteAsync(key));
                    }
                }

                await Task.WhenAll(tasks);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error when remove data from cache by patterns: {Patterns}", patterns);
            }
        }

        public bool SetData<T>(string key, T value, DateTimeOffset timeEnd)
        {
            try
            {
                var expireTime = timeEnd.DateTime.Subtract(DateTime.Now);

                if (_cacheDb == null)
                {
                    return false;
                }

                // Trả về true nếu set thành công, ngược lại trả về false. Nếu key đã tồn tại thì sẽ bị ghi đè.
                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };
                return _cacheDb.StringSet(key, JsonSerializer.Serialize(value, options), expireTime);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error when set data to cache with key: {Key}", key);
                return false;
            }
        }
    }
}