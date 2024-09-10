using MongoDB.Driver;

namespace Backend.BankingTranxSystem.SharedServices.Mongo;

public abstract class MongoDbContext : IMongoDbContext
{
    public IMongoDatabase Database { get; }

    protected MongoDbContext(IMongoDatabase database)
    {
        Database = database;
    }

    private IMongoCollection<MongoCounter> Counters =>
        Database.GetCollection<MongoCounter>("counters");
    static readonly object lockObject = new object();
    public long GetNextSequenceValue<T>()
    {
        var name = typeof(T).Name;
        return NextSequenceValue(name);
    }

    private long NextSequenceValue(string name)
    {
        lock (lockObject)
        {
            var counter = this.Counters.Find(x => x.Id == name).SingleOrDefault();
            if (counter == null)
            {
                counter = new MongoCounter() { Id = name };
                Counters.InsertOne(counter);
            }

            counter.Sequence++;
            Counters.ReplaceOne(x => x.Id == name, counter);
            return counter.Sequence;
        }
    }

}