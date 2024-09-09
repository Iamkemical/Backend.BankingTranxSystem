using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;

namespace Backend.BankingTranxSystem.SharedServices.Mongo;

public static class MongoServiceExtensions
{
    public static IServiceCollection AddMongoContext<TMongoContext>(this IServiceCollection services,
        string connectionString) where TMongoContext : MongoDbContext
    {
        ConventionRegistry.Register("Camel Case",
            new ConventionPack
            {
                new CamelCaseElementNameConvention(),
                new IgnoreExtraElementsConvention(true)
            }, _ => true);
        var config = GetConnectionConfig(connectionString);
        services.AddSingleton<IMongoClient>(s => new MongoClient(config["host"]));
        services.AddScoped(c =>
        {
            var client = c.GetRequiredService<IMongoClient>();
            return client.GetDatabase(config["database"]);
        });
        services.AddScoped<TMongoContext>();
        return services;
    }

    private static Dictionary<string, string> GetConnectionConfig(string connectionString)
    {
        var result = connectionString.Split(";")
            .Select(x => new KeyValuePair<string, string>(x.Split("=")[0], x.Split("=")[1]));
        return new Dictionary<string, string>(result);
    }
}