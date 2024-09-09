using Ardalis.Specification.EntityFrameworkCore;
using Backend.BankingTranxSystem.SharedServices.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Backend.BankingTranxSystem.API.Data.Repositories;

public class EfRepository<T>(BankingTranxSystemContext dbContext) : RepositoryBase<T>(dbContext), IRepository<T> where T : class//, IAggregateRoot
{
    private readonly DbContext _dbContext = dbContext;

    public override async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _dbContext.Set<T>().AddAsync(entity, cancellationToken);

        return entity;
    }

    public override async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        await _dbContext.AddRangeAsync(entities, cancellationToken);

        return entities;
    }

    public override Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        _dbContext.Set<T>().Update(entity);

        return Task.CompletedTask;
    }

    public override Task UpdateRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        _dbContext.Set<T>().UpdateRange(entities);

        return Task.CompletedTask;
    }

    public override Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        _dbContext.Set<T>().Remove(entity);

        return Task.CompletedTask;
    }

    public override Task DeleteRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        _dbContext.Set<T>().RemoveRange(entities);

        return Task.CompletedTask;
    }

    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbContext.Set<T>()
                .AsQueryable()
                .Where(predicate)
                .ToListAsync();
    }

    public IQueryable<T> GetAllAsync()
    {
        return _dbContext.Set<T>()
            .AsQueryable();
    }
}

