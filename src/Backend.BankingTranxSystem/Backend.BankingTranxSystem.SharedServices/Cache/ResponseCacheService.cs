using Backend.BankingTranxSystem.SharedServices.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Threading.Tasks;
using System;
using System.Text.Json;

namespace Backend.BankingTranxSystem.SharedServices.Cache;

public class ResponseCacheService(IDistributedCache distributedCache, IConnectionMultiplexer connectionMultiplexer, ILogger<ResponseCacheService> logger) : IResponseCacheService
{
    private readonly IDistributedCache _distributedCache = distributedCache;
    private readonly IConnectionMultiplexer _connectionMultiplexer = connectionMultiplexer;
    private readonly ILogger<ResponseCacheService> _logger = logger;
    private readonly string _cacheKeySuffix = "_" + Utility.GetEnvironmentName();

    public async Task CacheResponseAsync(string cacheKey, object response, TimeSpan timeToLive)
    {
        cacheKey += cacheKey.Contains(_cacheKeySuffix) ? "" : _cacheKeySuffix;

        if (response == null)
        {
            return;
        }

        var serailizedResponse = JsonSerializer.Serialize(response);

        await _distributedCache.SetStringAsync(cacheKey, serailizedResponse, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = timeToLive,
            SlidingExpiration = null
        });
    }

    public async Task<string> GetCachedResponseAsync(string cacheKey)
    {
        cacheKey += cacheKey.Contains(_cacheKeySuffix) ? "" : _cacheKeySuffix;
        var cachedResponse = await _distributedCache.GetStringAsync(cacheKey);

        return string.IsNullOrEmpty(cachedResponse) ? null : cachedResponse;
    }

    public async Task<T> GetCachedResponseAsync<T>(string cacheKey)
    {
        cacheKey += cacheKey.Contains(_cacheKeySuffix) ? "" : _cacheKeySuffix;
        try
        {
            var jsonData = await _distributedCache.GetStringAsync(cacheKey);

            if (jsonData is null)
            {
                return default(T);
            }

            return JsonSerializer.Deserialize<T>(jsonData);
        }
        catch (Exception)
        {

            return default(T);
        }
    }
    /// <summary>
    /// Cache data, but remove key if null
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="cacheKey"></param>
    /// <param name="data"></param>
    /// <param name="t"></param>
    /// <returns></returns>
    public async Task CacheData<T>(string cacheKey, T data, int t) //where T : new()
    {
        cacheKey += cacheKey.Contains(_cacheKeySuffix) ? "" : _cacheKeySuffix;
        try
        {
            var ttl = TimeSpan.FromSeconds(t);

            if (data is null)
            {
                _ = _distributedCache.RemoveAsync(cacheKey);
            }
            await CacheResponseAsync(cacheKey, data, ttl);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "CacheData - {msg}", ex.Message);
        }
    }

    public async Task<TResult> GetOrCreateAsync<TResult>(string cacheKey, Func<Task<TResult>> func, int ttl)
    {
        var r = await GetCachedResponseAsync<TResult>(cacheKey);

        if (r is null)
        {
            r = await func();

            await CacheData(cacheKey, r, ttl);
        }

        return r;
    }

    public async Task<CachePagedList<T>> GetCachedPagedListResponseAsync<T>(string cacheKey, int pageNumber, HttpResponse Response) where T : class
    {
        cacheKey += cacheKey.Contains(_cacheKeySuffix) ? "" : _cacheKeySuffix;
        try
        {
            var jsonData = await _distributedCache.GetStringAsync(cacheKey);

            if (jsonData is null)
            {
                return default(CachePagedList<T>);
            }

            var data = JsonSerializer.Deserialize<CachePagedList<T>>(jsonData);

            if (data is not null)
            {
                var paginationData = new
                {
                    totalCount = data.TotalCount,
                    pageSize = data.PageSize,
                    currentPage = pageNumber,
                    totalPages = data.TotalPages
                };
                var paginationString = JsonSerializer.Serialize(paginationData);
                Response.Headers.Append("X-Pagination", paginationString);
            }

            return data;
        }
        catch (Exception)
        {

            return default(CachePagedList<T>);
        }
    }

    public async Task CachePagedListResponseAsyn<T>(string cacheKey, PagedList<T> response, TimeSpan timeToLive) where T : class
    {
        if (response == null)
        {
            return;
        }

        cacheKey += cacheKey.Contains(_cacheKeySuffix) ? "" : _cacheKeySuffix;

        var dataToCache = new CachePagedList<T>
        {
            CurrentPage = response.CurrentPage,
            Items = response,
            PageSize = response.PageSize,
            TotalCount = response.TotalCount,
            TotalPages = response.TotalPages,
        };

        var serailizedResponse = JsonSerializer.Serialize(dataToCache);

        await _distributedCache.SetStringAsync(cacheKey, serailizedResponse, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = timeToLive,
            SlidingExpiration = null

        });
    }

    public async Task RemoveDataFromCache(string cacheKey)
    {
        cacheKey += cacheKey.Contains(_cacheKeySuffix) ? "" : _cacheKeySuffix;
        await _distributedCache.RemoveAsync(cacheKey);
    }

    public async Task<bool> TakeLockAsync(string cacheKey, int durationInSecs = 15)
    {
        var db = _connectionMultiplexer.GetDatabase();

        return await db.LockTakeAsync(cacheKey, cacheKey, TimeSpan.FromSeconds(durationInSecs));
    }

    public async Task ReleaseLockAsync(string cacheKey)
    {
        var db = _connectionMultiplexer.GetDatabase();

        await db.LockReleaseAsync(cacheKey, cacheKey);
    }
}