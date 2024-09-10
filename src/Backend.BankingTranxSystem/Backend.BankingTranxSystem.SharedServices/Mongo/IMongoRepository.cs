using MongoDB.Driver.Linq;
using System;
using System.Linq.Expressions;

namespace Backend.BankingTranxSystem.SharedServices.Mongo;

public interface IMongoRepository<T> where T : class
{
    int Count(Expression<Func<T, bool>> predicate);
    IMongoQueryable<T> Queryable { get; }
    IMongoQueryable<T> Table { get; }

    IMongoQueryable<T> Fetch(Expression<Func<T, bool>> predicate,
        Func<IMongoQueryable<T>, IOrderedMongoQueryable<T>> orderBy = null, int? page = null, int? pageSize = null);
}