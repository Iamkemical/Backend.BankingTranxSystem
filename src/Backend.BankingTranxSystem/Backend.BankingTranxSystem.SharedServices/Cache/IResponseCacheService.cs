using Backend.BankingTranxSystem.SharedServices.Helper;
using Microsoft.AspNetCore.Http;

namespace Backend.BankingTranxSystem.SharedServices.Cache;
public interface IResponseCacheService

{

    Task CacheResponseAsync(string cacheKey, object response, TimeSpan timeToLive);

    Task<string> GetCachedResponseAsync(string cacheKey);

    Task<T> GetCachedResponseAsync<T>(string cacheKey);

    Task CacheData<T>(string cacheKey, T data, int t);// where T : new();

    Task<CachePagedList<T>> GetCachedPagedListResponseAsync<T>(string cacheKey, int pageNumber, HttpResponse Response) where T : class;

    Task CachePagedListResponseAsync<T>(string cacheKey, PagedList<T> response, TimeSpan timeToLive) where T : class;

    Task RemoveDataFromCache(string cacheKey);

    /// <summary>

    /// GetOrCreateAsync

    /// </summary>

    /// <typeparam name="TResult"></typeparam>

    /// <param name="cachKey"></param>

    /// <param name="func"></param>

    /// <param name="ttl">Time to live in seconds</param>

    /// <returns></returns>

    Task<TResult> GetOrCreateAsync<TResult>(string cachKey, Func<Task<TResult>> func, int ttl = 60);

    Task<bool> TakeLockAsync(string cacheKey, int durationInMins = 1);

    Task ReleaseLockAsync(string cacheKey);

}