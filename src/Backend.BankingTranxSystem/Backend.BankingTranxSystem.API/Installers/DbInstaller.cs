using Backend.BankingTranxSystem.API.Data;
using Backend.BankingTranxSystem.API.Data.Repositories;
using Backend.BankingTranxSystem.SharedServices.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.BankingTranxSystem.API.Installers;

public class DbInstaller : IInstaller
{
    public void InstallServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<BankingTranxSystemContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("SqlServerConnection"), options => options.EnableRetryOnFailure());
        }, ServiceLifetime.Scoped);

        //var mongoConnection = configuration.GetConnectionString("MongoConnection");
        //services.AddMongoContext<SharedDbContext>(mongoConnection);

        //Repository
        services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
        services.AddScoped(typeof(EfRepository<>));
        services.AddScoped(typeof(IReadRepository<>), typeof(CachedRepository<>));

        //services.AddScoped(typeof(IMongoRepository<>), typeof(MongoRepository<>));
        //services.AddScoped(typeof(MongoRepository<>));

        //services.AddHangfire(config => config
        //.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
        //.UseSimpleAssemblyNameTypeSerializer()
        //.UseRecommendedSerializerSettings()
        //.UseSqlServerStorage(configuration.GetConnectionString("SqlServerConnection"), new SqlServerStorageOptions
        //{
        //    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
        //    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
        //    QueuePollInterval = TimeSpan.Zero,
        //    UseRecommendedIsolationLevel = true,
        //    DisableGlobalLocks = true,
        //}));

        //services.AddHangfireServer();
    }
}