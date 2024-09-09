using MongoDB.Driver;

namespace Backend.BankingTranxSystem.SharedServices.Mongo;

public interface IMongoDbContext
{
    IMongoDatabase Database { get; }
}