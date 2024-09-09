using MongoDB.Bson.Serialization.Attributes;

namespace Backend.BankingTranxSystem.SharedServices.Mongo;

public class MongoCounter
{
    [BsonId] public string Id { get; set; }
    public long Sequence { get; set; }
}