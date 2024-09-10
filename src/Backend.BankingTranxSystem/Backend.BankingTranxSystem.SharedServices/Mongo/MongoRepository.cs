using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Backend.BankingTranxSystem.SharedServices.Mongo;

public abstract class MongoRepository<T> : IMongoRepository<T> where T : class
{
    private readonly IMongoDbContext _dbContext;

    protected MongoRepository(IMongoDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public virtual int Count(Expression<Func<T, bool>> predicate)
    {
        return Fetch(predicate).Count();
    }

    protected abstract string TableName { get; }

    public IMongoQueryable<T> Queryable =>
        _dbContext.Database.GetCollection<T>(TableName).AsQueryable();

    public IMongoQueryable<T> Table => this.Queryable;
    public virtual IMongoQueryable<T> Fetch(Expression<Func<T, bool>> predicate,
        Func<IMongoQueryable<T>, IOrderedMongoQueryable<T>> orderBy = null, int? page = null, int? pageSize = null)
    {
        IMongoQueryable<T> query = this.Queryable;

        if (orderBy != null)
        {
            query = orderBy(query);
        }
        if (predicate != null)
        {
            query = query.Where(predicate);
        }

        if (page != null && pageSize != null)
        {
            if (page == 0) page = 1;
            if (pageSize == 0) pageSize = 10;
            query = query.Skip((page.Value - 1) * pageSize.Value).Take(pageSize.Value);
        }

        return query;
    }

}