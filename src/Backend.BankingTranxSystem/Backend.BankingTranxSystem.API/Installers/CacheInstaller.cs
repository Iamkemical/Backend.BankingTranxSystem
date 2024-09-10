using Backend.BankingTranxSystem.SharedServices.Cache;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;

namespace Backend.BankingTranxSystem.API.Installers;

public class CacheInstaller : IInstaller
{
    public void InstallServices(IServiceCollection services, IConfiguration configuration)
    {
        var redisCacheSettings = new RedisCacheSettings();
        configuration.GetSection(nameof(redisCacheSettings)).Bind(redisCacheSettings);
        services.AddSingleton(redisCacheSettings);

        if (!redisCacheSettings.Enabled)
        {
            return;
        }
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisCacheSettings.ConnectionString;

        });
        services.AddScoped<IConnectionMultiplexer>(sp => ConnectionMultiplexer.ConnectAsync(redisCacheSettings.ConnectionString).Result);
        services.AddScoped<IResponseCacheService, ResponseCacheService>();

        services.AddDistributedMemoryCache();

        ////Cache Headers
        //services.AddHttpCacheHeaders((expirationModelOptions) =>
        //{
        //    expirationModelOptions.MaxAge = 60;
        //    expirationModelOptions.CacheLocation = Marvin.Cache.Headers.CacheLocation.Public;
        //},
        //(validationModelOptions) =>
        //{
        //    validationModelOptions.MustRevalidate = true;
        //    validationModelOptions.Vary = Utility.GetDefaultVaryByHeader();
        //});
    }
}