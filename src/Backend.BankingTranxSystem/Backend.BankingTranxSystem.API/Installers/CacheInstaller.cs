using Backend.BankingTranxSystem.SharedServices.Cache;
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
        services.AddSingleton<IConnectionMultiplexer>(sp => ConnectionMultiplexer.ConnectAsync(redisCacheSettings.ConnectionString).Result);
        services.AddSingleton<IResponseCacheService, ResponseCacheService>();

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