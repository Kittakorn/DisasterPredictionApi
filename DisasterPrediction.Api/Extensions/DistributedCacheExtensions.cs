using Microsoft.Extensions.Caching.Distributed;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DisasterPrediction.Api.Extensions;

public static class DistributedCacheExtensions
{
    private const int cachExpirationTime = 15;

    private static JsonSerializerOptions serializerOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = null,
        WriteIndented = true,
        AllowTrailingCommas = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public static T Get<T>(this IDistributedCache cache, string key)
    {
        var val = cache.Get(key);

        if (val == null)
            return default;

        return JsonSerializer.Deserialize<T>(val, serializerOptions);
    }

    public static Task SetAsync<T>(this IDistributedCache cache, string key, T value)
    {
        var options = new DistributedCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(cachExpirationTime));

        return SetAsync(cache, key, value, options);
    }

    public static Task SetAsync<T>(this IDistributedCache cache, string key, T value, DistributedCacheEntryOptions options)
    {
        var bytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(value, serializerOptions));
        return cache.SetAsync(key, bytes, options);
    }

    public static bool TryGetValue<T>(this IDistributedCache cache, string key, out T? value)
    {
        var val = cache.Get(key);
        value = default;
        if (val == null) return false;
        value = JsonSerializer.Deserialize<T>(val, serializerOptions);
        return true;
    }

    public static async Task<T?> GetOrSetAsync<T>(this IDistributedCache cache, string key, Func<Task<T>> task, DistributedCacheEntryOptions? options = null)
    {
        if (options == null)
            options = new DistributedCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromHours(cachExpirationTime));
        
        if (cache.TryGetValue(key, out T? value) && value is not null)
            return value;

        value = await task();
        if (value is not null)
            await cache.SetAsync<T>(key, value, options);
        
        return value;
    }
}
