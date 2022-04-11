using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Common.Caching.Interfaces;
using Common.Settings;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Common.Caching;
public class Cache : ICache
{
    private readonly IDistributedCache _cache;
    private readonly ILogger<Cache> _logger;
    private readonly CacheSettings _cacheSettings;

    public Cache(
        IDistributedCache cache,
        ILogger<Cache> logger,
        IOptions<CacheSettings> cacheSettings)
    {
        _cache = cache;
        _logger = logger;
        _cacheSettings = cacheSettings.Value;
    }

    public async Task<T> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            var json = await _cache.GetAsync(key, cancellationToken);

            return json != null
                ? JsonSerializer.Deserialize<T>(json)
                : default;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex,
                "Cache read error ({@ConnectionString}). Key: {@Key}",
                _cacheSettings.ConnectionString,
                key);

            return default;
        }
    }
    public async Task SetAsync<T>(string key, T value, CancellationToken cancellationToken = default) =>
        await SetAsync(key, value, null, null, cancellationToken);

    public async Task SetAsync<T>(string key,
        T value,
        TimeSpan? absoluteExpiration,
        TimeSpan? slidingExpiration,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var json = JsonSerializer.Serialize(value);

            await _cache.SetStringAsync(key,
                json,
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow =
                        absoluteExpiration ?? TimeSpan.FromSeconds(_cacheSettings.AbsoluteExpirationInSeconds),
                    SlidingExpiration =
                        slidingExpiration ?? TimeSpan.FromSeconds(_cacheSettings.SlidingExpirationInSeconds)
                },
                cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex,
                "Cache write error ({@ConnectionString}). Key: {@Key}. ValueType: {@ValueType}",
                _cacheSettings.ConnectionString,
                key,
                value.GetType());
        }
    }
}