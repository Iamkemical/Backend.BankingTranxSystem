using Ardalis.Specification;
using Backend.BankingTranxSystem.SharedServices.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Backend.BankingTranxSystem.API.Data.Repositories;
public class CachedRepository<T>(EfRepository<T> sourceRepository) : IReadRepository<T> where T : class
{
    public Task<bool> AnyAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
    {
        return sourceRepository.AnyAsync(specification, cancellationToken);
    }

    public Task<bool> AnyAsync(CancellationToken cancellationToken = default)
    {
        return sourceRepository.AnyAsync(cancellationToken);
    }

    public IAsyncEnumerable<T> AsAsyncEnumerable(ISpecification<T> specification)
    {
        return sourceRepository.AsAsyncEnumerable(specification);
    }

    public Task<int> CountAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
    {
        return sourceRepository.CountAsync(specification, cancellationToken);
    }

    public Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return sourceRepository.CountAsync(cancellationToken);
    }

    public Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        return sourceRepository.FindAsync(predicate);
    }

    public Task<T?> FirstOrDefaultAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<TResult?> FirstOrDefaultAsync<TResult>(ISpecification<T, TResult> specification, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public IQueryable<T> GetAllAsync()
    {
        return sourceRepository.GetAllAsync().AsNoTracking();
    }

    public Task<T?> GetByIdAsync<TId>(TId id, CancellationToken cancellationToken = default) where TId : notnull
    {
        throw new NotImplementedException();
    }

    public Task<T?> GetBySpecAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<TResult?> GetBySpecAsync<TResult>(ISpecification<T, TResult> specification, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<List<T>> ListAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<List<T>> ListAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<List<TResult>> ListAsync<TResult>(ISpecification<T, TResult> specification, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<T?> SingleOrDefaultAsync(ISingleResultSpecification<T> specification, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<TResult?> SingleOrDefaultAsync<TResult>(ISingleResultSpecification<T, TResult> specification, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}